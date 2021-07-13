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
    private float width;

    public void SetDataCustomFields(IDataLibrary data, string fields, bool isHeader = false)
    {
        this.fieldList = fields;
        SetData(data);
    }
    //TODO: need to change bgcolor + txtcolor automatically 
    public override void PreDataUpdate(IDataLibrary data)
    {
        RectTransform r = GetComponent<RectTransform>();
        width = r.rect.width;
        RectTransform parRect = transform.GetComponentInParent<RectTransform>();
        width = parRect.rect.width;
        
        string[] fields = fieldList.Split(',');
        float widthPer = r.rect.width / (fields.Length-1);
        int i = 0;
        foreach (string field in fields)
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
                        RectTransform rect = g.GetComponent<RectTransform>();
                        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,i*widthPer,widthPer);
                        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top,0, r.rect.height /2);
                        break;
                }
            }
            i++;
        }
        //create components.
    }

}
