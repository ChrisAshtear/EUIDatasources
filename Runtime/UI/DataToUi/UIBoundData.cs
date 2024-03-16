using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Animations;
using System.ComponentModel;

//handler for array type



public class UIBoundData : MonoBehaviour
{
    System.Object data;

    private static string[] deprecatedProps = { "audio", "rigidbody2D", "rigidbody", "particleSystem", "collider", "collider2D", "renderer", "constantForce", "light", "animation", "camera", "hingeJoint","networkView" };

    public delegate object ValueChanger(object initialValue, GameObject callingObj);
    public delegate IList GetListData(string fieldName);
    UnityEvent<GameObject,System.Object> dataChanged = new UnityEvent<GameObject, object> ();
    Dictionary<string,System.Object> dataList = new Dictionary<string, object>();
    Dictionary<string, PropertyInfo> props = new Dictionary<string, PropertyInfo>();
    
    System.Object parentData;

    static Dictionary<string, ValueChanger> dataTypeChangeHandlers = new Dictionary<string, ValueChanger>();
    static Dictionary<string, GetListData> dataListHandlers = new Dictionary<string, GetListData>();

    private UIDataTag[] tags;

    //check for other instances of uibounddata in children. then do not bind data.
    public bool checkForOtherBinds = false;

    private object parentDataRef;
    private Transform detailsWindow;
    [HideInInspector]
    public UID_DataList listObj;

    public static void AddHandler(string name, ValueChanger handler)
    {
        if(!dataTypeChangeHandlers.ContainsKey(name))
        {
            dataTypeChangeHandlers.Add(name, handler);
        }
        else
        {
            dataTypeChangeHandlers[name] = handler;
        }
    }
    public static void AddListHandler(string name, GetListData handler)
    {
        if (!dataListHandlers.ContainsKey(name))
        {
            dataListHandlers.Add(name, handler);
        }
        else
        {
            dataListHandlers[name] = handler;
        }
    }

    private void OnEnable()
    {
        /*if(this.parentData != null)
        {
            SetData(data);
        }
       */
    }

    //last 2 args are for UIDlist
    public void SetData(System.Object data, UnityAction<GameObject,System.Object> callback=null, int listIndex = -1, object parentDataRef = null, Transform detailsWindow = null)
    {
        //if (data == parentData) { return; }
        Debug.Log("Setting Data for " + gameObject.name);
        parentData = data;
        dataChanged = new UnityEvent<GameObject, object>();
        if(callback != null) { dataChanged.AddListener(callback); }
        this.detailsWindow = detailsWindow;
        if (parentDataRef!= null)
        {
            this.parentDataRef = parentDataRef;
        }
        
        dataList.Clear();
        Dictionary<string, PropertyInfo> props = data.GetType().GetProperties( ).ToDictionary(p => p.Name);
        this.props = props;
        foreach(KeyValuePair<string, PropertyInfo> pair in props)
        {
            if(pair.Key == "Item" || deprecatedProps.Contains(pair.Key))
            {
                continue;//Skipping data container in List, deprecated rigidbody
            }
            dataList.Add(pair.Key.ToLower(), pair.Value.GetValue(data));
        }
        dataList.Add("this", parentData);
        if(parentData is IList || this.parentDataRef is IList)
        {
            IList list;
            if (parentData is IList)
            {
                list = (IList)parentData;
            }
            else
            {
                list = (IList)this.parentDataRef;
            }
            dataList.TryAdd("count", (object)list.Count);
            dataList.TryAdd("index", (object)listIndex);
            dataList.TryAdd("new", (object)1);
            dataList.TryAdd("delete", (object)1);
            if(detailsWindow)
            {
                dataList.TryAdd("select", (object)1);
            }
            Debug.Log("index set to " + listIndex);
        }


        //getcomponents in children(bounddata)- if any data tag is a child of this bounddata, dont try to bind.(isChildOf)

        tags = GetComponentsInChildren<UIDataTag>(true);
        foreach (UIDataTag tag in tags)
        {
            Debug.Log("setting tag" + tag.Tag);
            if (checkForOtherBinds && tag.GetComponentInParent<UIBoundData>(true) != this && tag.GetComponent<UID_DataList>() == null)
            { 
                continue; 
            }
            //if(tag.GetComponentInParent<UIBoundData>() != this) { continue; } // this cant seem to find the current bounddata/
            if(dataList.ContainsKey(tag.Tag.ToLower()))
            {
                BindField(tag.gameObject, tag.Tag.ToLower());
            }
        }
        //InvokeRepeating("UpdateFields", 1, 1);
    }

    //temporary function to update text.
    void UpdateFields()
    {
        object data = parentData;
        if(this.parentDataRef != null) { data = this.parentDataRef; }
        foreach (KeyValuePair<string, PropertyInfo> pair in props)
        {
            if (pair.Key == "Item")
            {
                continue;//Skipping data container in List.
            }
            if(dataList.ContainsKey(pair.Key.ToLower()))
            {
                dataList[pair.Key.ToLower()] = pair.Value.GetValue(data);
            }
            else
            {
                dataList.Add(pair.Key.ToLower(), pair.Value.GetValue(data));
            }
        }

        foreach (UIDataTag tag in tags)
        {
            if (checkForOtherBinds && tag.GetComponentInParent<UIBoundData>(true) != this && tag.GetComponent<UID_DataList>() == null)
            {
                continue;
            }
            //if(tag.GetComponentInParent<UIBoundData>() != this) { continue; } // this cant seem to find the current bounddata/
            if (dataList.ContainsKey(tag.Tag.ToLower()))
            {
                BindField(tag.gameObject, tag.Tag.ToLower());
            }
        }
    }

    void BindField(GameObject obj, string tag, bool textOnly = false)
    {
        //Need to add support for Color, Radio Buttons.
        //Should also programmattically list all objs with DataTag in inspector.

        string proptype = "unknown";
        bool isEnum = false;
        string propkey = getPropVal(tag);
        if (props.ContainsKey(propkey)) 
        {
            proptype = props[propkey].PropertyType.Name;
            isEnum = props[propkey].GetType().IsEnum;
        }
        else
        {
            proptype = dataList[tag].GetType().ToString();
        }

        TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = dataList[tag].ToString();
        }

        if(textOnly)
        {
            return;
        }

        TMP_InputField field = obj.GetComponent<TMP_InputField>();
        bool handlerFound = false;


        if (field != null)
        {
            field.text = dataList[tag].ToString() ;
            if (field.contentType == TMP_InputField.ContentType.Standard) { field.onValueChanged.AddListener((data) => ValueChangedStr(tag, data)); }
            else if (field.contentType == TMP_InputField.ContentType.DecimalNumber) { field.onValueChanged.AddListener((data) => ValueChangedNumber(tag, data)); }
            else if( field.contentType == TMP_InputField.ContentType.IntegerNumber) { field.onValueChanged.AddListener((data) => ValueChangedNumber(tag, data, true)); }
            handlerFound = true;
        }
        Button button = obj.GetComponent<Button>();
        if(button && !handlerFound)
        {
            if (proptype == "Boolean")
            {
                Debug.Log("button onclick" + button.GetHashCode());
                button.onClick.AddListener(() => ValueChangedBool(tag));
            }
            else if (dataTypeChangeHandlers.ContainsKey(proptype))
            {
                button.onClick.AddListener(() => dataTypeChangeHandlers[proptype].Invoke(dataList[tag],button.gameObject));
            }
            //parentData is object sent, parentDataRef is ITS parent sent from datalist. rename these.
            else if( parentData is IList || this.parentDataRef is IList)
            {
                IList list;
                if(parentData is IList)
                {
                    list = (IList)parentData;
                }
                else
                {
                    list = (IList)parentDataRef;
                }
                // IList list = (IList)parentData;
                Type type = list.GetType().GetGenericArguments().Single();

                UnityAction addList = () => {
                    object newObj = Activator.CreateInstance(type); list.Add(newObj); if (listObj) { listObj.UpdateData(); }
                };
                UnityAction removeList = () =>
                {
                    Debug.Log("removing " + (int)dataList["index"] + "-bound to " + gameObject.name);
                    list.RemoveAt((int)dataList["index"]);
                    if (listObj) { listObj.UpdateData(); }
                };
                //need to get onClick listeners that were assigned IN EDITOR, and retain them, while clearing the ones added in program.
                button.onClick.RemoveAllListeners();

                switch(tag)
                {
                    case "new":
                        button.onClick.AddListener(addList);
                        break;
                    case "delete":
                        button.onClick.AddListener(removeList);
                        break;
                    case "select":
                        button.onClick.AddListener(() => {
                                UIBoundData d = this.detailsWindow.GetComponent<UIBoundData>();
                                d.SetData(this.parentData);
                            }) ;
                        break;
                }
            }
            handlerFound = true;
        }
        TMP_Dropdown dropdown = obj.GetComponent<TMP_Dropdown>();
        if (dropdown && !handlerFound)
        {
            // I will get all values and iterate through them    
            if(isEnum)
            {
                Type enumType = dataList[tag].GetType();
                Array enumValues = enumType.GetEnumValues();
                Dictionary<string, object> enumValuesStr = new Dictionary<string, object>();

                foreach (var value in enumValues)
                {
                    // with our Type object we can get the information about    
                    // the members of it    
                    MemberInfo memberInfo =
                        enumType.GetMember(value.ToString()).First();

                    enumValuesStr.Add(value.ToString(), value);

                }
                dropdown.ClearOptions();
                dropdown.AddOptions(enumValuesStr.Keys.ToList());
                dropdown.value = (int)dataList[tag];
                dropdown.onValueChanged.AddListener((data) => ValueChangedEnum(tag, data, enumType));
            }
            else
            {
                string parentProp = dataList["this"].GetType().ToString();
                string fullProp = $"{parentProp}.{tag}";
                if ( (dataListHandlers.ContainsKey(fullProp)) )
                {
                    IList datalist = dataListHandlers[fullProp].Invoke(fullProp);
                    Dictionary<string, object> valuesStr = new Dictionary<string, object>();
                    List<string> options = new List<string>();
                    foreach (var value in datalist)
                    {
                        options.Add(value.ToString());
                    }
                    dropdown.ClearOptions();
                    dropdown.AddOptions(options);
                    dropdown.value = (int)dataList[tag];
                    dropdown.onValueChanged.AddListener((data) => ValueChangedNumber(tag, data.ToString(), true));
                }
                
            }
            

            handlerFound = true;
        }
        UID_DataList dataObject = obj.GetComponent<UID_DataList>();
        if(dataObject)
        {
            dataObject.SetData(dataList[tag]);
            if(parentData is ConfigObject)
            {
                ConfigObject dat = (ConfigObject)parentData;
                ConfigObject.AddValueChangeCallback(dat.typeName + "." + tag, (obj) => {

                    dataObject.SetData(obj);
                });
            }
        }
        /*UIBoundData childBoundData = obj.GetComponent<UIBoundData>();
        if (childBoundData)
        {
            childBoundData.SetData(dataList[tag]);
        }*/

        IDataReceiver[] receivers = obj.GetComponentsInChildren<IDataReceiver>();
        foreach(IDataReceiver receiver in receivers)
        {
            receiver.SetData(dataList[tag]);
        }
        
    }

    void AddListItem(object item, IList list, Type type)
    {
        object newObj = Activator.CreateInstance(type); 
        list.Add(newObj); 
        if (listObj)
        { 
            listObj.UpdateData(); 
        }
    }

    void ValueChangedStr(string key, string value)
    {
        //this will update the data in the parent object. No need for a callback at the moment.
        string propkey = getPropVal(key);
        if (propkey == "") { return; }
        props[propkey].SetValue(parentData, value);
        dataList[key] = value;
    }

    void ValueChangedEnum(string key, object value, Type enumType)
    {
        string propkey = getPropVal(key);
        if (propkey == "") { return; }
        props[propkey].SetValue(parentData, value);
        dataList[key] = value;
    }

    void ValueChangedNumber(string key, string value, bool integer=false)
    {
        bool parseOK = false;
        string propkey = getPropVal(key);
        if (propkey == "") { return; }
        if (integer)
        {
            parseOK = int.TryParse(value, out int val);
            if (!parseOK) { Debug.Log($"parse failed on {key}-{value}"); }
            props[propkey].SetValue(parentData, val);
            dataList[key] = val;
        }
        else
        {
            parseOK = float.TryParse(value, out float val);
            if (!parseOK) { Debug.Log($"parse failed on {key}-{value}"); }
            props[propkey].SetValue(parentData, val);
            dataList[key] = val;
        }

        //this will update the data in the parent object. No need for a callback at the moment.
       
        
    }

    string getPropVal(string name)
    {
        foreach(string key in props.Keys)
        {
            if(name == key.ToLower())
            {
                return key;
            }
        }
        return "";
    }

    void ValueChangedBool(string key)
    {
        string propkey = getPropVal(key);
        if (propkey == "") { return; }
        bool parseOK = bool.TryParse(props[propkey].GetValue(parentData).ToString(), out bool val);
        if (!parseOK) { Debug.Log($"parse failed on {key}-{props[propkey].GetValue(parentData).ToString()}"); }
        //this will update the data in the parent object. No need for a callback at the moment.
        props[propkey].SetValue(parentData, !val);

        Debug.Log("setting bool"+val);
        dataList[key] = !val;
    }
}
