using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;
[RequireComponent(typeof(UIDataController))]
public class UIRowListItem : UIButtonListItem
{

    [Tooltip("Container Object for created Text objects")]
    public GameObject containerObject;
    public GameObject RowCellPrefab;
    public string fieldList;

    public void SetDataCustomFields(IDataLibrary data, string fields, bool isHeader = false)
    {
        this.fieldList = fields;
        SetData(data);
    }
    //TODO: need to change bgcolor + txtcolor automatically 
    public override void PreDataUpdate(IDataLibrary data)
    {
        string[] fields = fieldList.Split(',');
        foreach(string field in fields)
        {
            IData dat = data.GetValue(field);
            if (dat.Data != null)
            {
                switch(dat.Type)
                {
                    case DataLibSupportedTypes.text:
                        GameObject g = Instantiate(RowCellPrefab, containerObject.transform, false);
                        TextMeshProUGUI text = g.GetComponent<TextMeshProUGUI>();
                        text.text = dat.DisplayValue;
                        break;
                }
            }
        }
        //create components.
    }

}
