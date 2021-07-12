using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class listObjProps : MonoBehaviour
{
    public string[] valsList; // level vals
    public string[] saveValsList;
    public GameObject prefabName;
    public GameObject prefabDesc;
    public GameObject prefabHeader;
    public GameObject prefabCustom;
    public GameObject prefabSep;
    public GameObject prefabListObj;
    public GameObject horizRows;
    public GameObject vertRows;
    public Texture2D iconSheet;
    public Color fontColor = Color.white;
    public string Name;

    void Start()
    {
        Invoke("list", 2);
    }

    public void list()
    {
        Animator anim = GetComponent<Animator>();
        //createText("Name", Name);

        
        //also display best times.
    }

    public void resetVals()
    {
        Transform t = transform;
        List<GameObject> objs = new List<GameObject>();
        foreach (Transform child in t)
        {
            objs.Add(child.gameObject);
        }
        foreach (GameObject c in objs)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(c);
            }
            else
            {
                Destroy(c);
            }
        }
        list();
    }

    public void createText(string text, string value="",Transform newParent=null)
    {
        GameObject obj = (GameObject)Instantiate(prefabName,transform.position, Quaternion.identity,transform);
        if(newParent != null)
        {
            obj.transform.parent = newParent;
        }
        setText(obj.transform.Find("Name").gameObject, text + ": ");
        setText(obj.transform.Find("Value").gameObject, value);
    }

    public void createDescText(string text)
    {
        GameObject obj = (GameObject)Instantiate(prefabDesc, transform.position, Quaternion.identity,transform);
        obj.transform.parent = transform;
        setText(obj, text);
    }

    public void setText(GameObject obj, string text)
    {
        Text t = obj.GetComponentInChildren<Text>();
        if (t != null)
        {
            t.text = text;
            t.color = fontColor;
        }
        TMPro.TextMeshProUGUI tm = obj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tm != null)
        {
            tm.text = text;
            tm.color = fontColor;
        }
    }

    public void createCustom(GenericDataHandler data)
    {
        GameObject obj = (GameObject)Instantiate(prefabCustom, transform.position, Quaternion.identity, transform);

        UIDataTag[] fields = obj.GetComponentsInChildren<UIDataTag>();

        foreach (UIDataTag i in fields)
        {
            switch(i.dataType)
            {
                case UIDataType.Text:
                    setText(i.gameObject, data.getData(i.fieldName));
                    break;
                case UIDataType.Sprite:
                    Image img = i.gameObject.GetComponentInChildren<Image>();
                    img.sprite = (Sprite)data.getDataObj(i.fieldName);//error control here please
                    break;
            }
        }

    }

    public void createListObjs(GenericDataHandler data, string fieldName,Transform parent=null)
    {
        object val = data.getDataObj(fieldName);
        
        Sprite[] sprites = Resources.LoadAll<Sprite>("GUIicons/" + iconSheet.name);
        if (val is IDictionary)
        {
            IDictionary dict = (IDictionary)val;
            ICollection keys = dict.Keys;
            foreach(var key in keys)
            {
                object indivItem = dict[key];

                GameObject obj = (GameObject)Instantiate(prefabListObj, transform.position, Quaternion.identity, transform);
                if(parent != null)
                {
                    obj.transform.parent = parent;
                }
                UIDataTag[] fields = obj.GetComponentsInChildren<UIDataTag>();
                GenericDataHandler dat = obj.AddComponent<GenericDataHandler>();
                //dat.setData(indivItem);
                foreach (UIDataTag i in fields)
                {
                    string text = dat.getData(i.fieldName);
                    setText(i.gameObject, text);
                }
            }

        }

    }

    public Transform createHorizGroup()
    {
        GameObject obj = (GameObject)Instantiate(horizRows, transform.position, Quaternion.identity, transform);
        return obj.transform;
    }

    public Transform createVertGroup(string text,Transform parent=null)
    {
        GameObject obj = (GameObject)Instantiate(vertRows, transform.position, Quaternion.identity, transform);
        obj.transform.parent = parent;
        createText(text,"",obj.transform);
        return obj.transform;
    }

    public void createHeader(int iconIdx,string text)
    {
        GameObject obj = (GameObject)Instantiate(prefabHeader, transform.position, Quaternion.identity, transform);
        Sprite[] sprites = Resources.LoadAll<Sprite>("GUIicons/" + iconSheet.name); 
        setText(obj, text);
        Image img = obj.GetComponentInChildren<Image>();
        img.sprite = sprites[iconIdx];
    }

    public void createSeperator()
    {
        GameObject obj = (GameObject)Instantiate(prefabSep, transform.position, Quaternion.identity, transform);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
