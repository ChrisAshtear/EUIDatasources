using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Unity.VectorGraphics; //uncomment if you use vectors
using UnityEngine;
using UnityEngine.UI;
public class UIDataController : MonoBehaviour
{
    protected List<UIDataTag> uiObjects;
    public GameObject trackingObject;
    public string statBarSuffix;
    private void Start()
    {
        uiObjects = GetComponentsInChildren<UIDataTag>().ToList();
    }

    public virtual void RefreshData(IDataLibrary data)
    {
        uiObjects = GetComponentsInChildren<UIDataTag>(true).ToList();
        foreach (UIDataTag tag in uiObjects)
        {
            IData dat = data.GetValue(tag.fieldName);
            GameObject obj = tag.gameObject;
            IData subType = null;
            //Debug.Log("UITag:" + tag.name);
            //Debug.Log("UITagType:" + tag.dataType.ToString());
            if (tag.dataType == UIDataType.DisableButtonIfTrue && dat.Data==null) { dat.Data = ""; }
            if (tag.dataType == UIDataType.ShowIfTrue && dat.Data==null) { dat.Data = "false"; }
            if (dat.Data == null) { continue; }
            
            switch (tag.dataType)
            {
                case UIDataType.Value:
                    obj.GetComponent<TMPro.TextMeshProUGUI>().text = dat.ToString();
                    break;
                case UIDataType.Sprite:
                    Vector3 oldScale = obj.transform.localScale;
                    Image i = obj.GetComponent<Image>();
                    if(i!=null)
                    {
                        i.sprite = (Sprite)dat.Data;
                        obj.transform.localScale = oldScale;
                    }
                    /*
                    else //Uncomment this block if you use SVG in your project.
                    {
                        SVGImage svg = obj.GetComponent<SVGImage>();
                        if(svg!=null)
                        {
                            svg.sprite = (Sprite)dat.Data;
                        }
                    }*/
                    break;
                case UIDataType.Bar:
                    /*IStats stats = trackingObject.GetComponent<IStats>();
                    UIStatBar bar = obj.GetComponent<UIStatBar>();
                    bar.fieldName = tag.fieldName;
                    if(subType != null) { bar.fieldName += "_" + subType.Data.ToString(); }
                    bar.trackingObject = trackingObject;
                    bar.enabled = true;
                    bar.UpdateData((IStat)stats.GetStatData(bar.fieldName));*///initialization
                    break;
                case UIDataType.List:
                    TMPro.TMP_Dropdown drop = obj.GetComponent<TMPro.TMP_Dropdown>();
                    drop.ClearOptions();
                    List<TMPro.TMP_Dropdown.OptionData> optionList = new List<TMPro.TMP_Dropdown.OptionData>();
                    List<object> options = ((IEnumerable)dat.Data).Cast<object>().ToList();
                    foreach (object o in options)
                    {
                        optionList.Add(new TMPro.TMP_Dropdown.OptionData(o.ToString()));
                    }
                    drop.AddOptions(optionList);
                    break;
                case UIDataType.Highlight:
                    Image img = obj.GetComponent<Image>();
                    HighlightOnSelect highlight = obj.GetComponent<HighlightOnSelect>();
                    highlight.Init(data,Convert.ToSingle(dat.Data));
                    //data tag needs to feed hash to highlightOnSelectX
                    //feed stats to highlightOnSelectX
                    //HLOS will contain statToListenForX
                    //hl add listener to 'selectedWeapon'X
                    //if hash matches currentVal of selectedWeapon, highlight image.
                    //some kind of basic stat that stores an int or str value without dealing with maximums?
                    //
                    break;
                case UIDataType.Color:
                    Image imgToColor = obj.GetComponent<Image>();
                    if (imgToColor != null) { imgToColor.color = (Color)dat.Data; }
                    TMPro.TextMeshProUGUI text = obj.GetComponent<TMPro.TextMeshProUGUI>();
                    if(text != null) { text.color = (Color)dat.Data; }
                    if (Application.isEditor) { imgToColor.SetAllDirty(); }
                    break;

                case UIDataType.DataButton:
                    DataButton button = obj.GetComponent<DataButton>();
                    button.Data = dat.Data;
                    break;

                case UIDataType.DisableButtonIfTrue:
                    Button button2 = obj.GetComponent<Button>();
                    button2.interactable = !Convert.ToBoolean(dat.Data);
                    if (tag.invert) { button2.interactable = !button2.interactable; }
                    break;

                case UIDataType.ShowIfTrue:
                    if(dat.Data == "false" && tag.fieldName.Contains("&"))
                    {
                        bool alltrue = true;
                        string[] fields = tag.fieldName.Split('&');
                        foreach(string s in fields)
                        {
                            IData datObj = data.GetValue(s);
                            if(!Convert.ToBoolean(datObj.Data))
                            {
                                alltrue = false;
                            }
                        }
                        dat.Data = alltrue;
                    }
                    obj.SetActive(Convert.ToBoolean(dat.Data));
                    if (tag.invert) { obj.SetActive(!obj.activeSelf); }
                    break;

                case UIDataType.ShowIfEqual:
                    /*
                    bool show = (dat.Data.ToString() == subType.ToString());
                    obj.SetActive(show);
                    if (tag.invert) { obj.SetActive(!show); }*/
                    break;
            }

        }
        
    }
}