using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UIDataController))]
public class UIRowListItem : UIButtonListItem
{

    [Tooltip("Container Object for created Text objects")]
    public GameObject containerObject;
    public GameObject RowCellPrefab;
    public string fieldList;
    private float width;
    private float[] fieldSizePct;
    private float lastWidth;

    private bool fieldsCreated = false;
    private Dictionary<string, GameObject> columns = new Dictionary<string, GameObject>();

    public void SetDataCustomFields(IDataLibrary data, string fields, int[] fieldSize, bool isHeader = false)
    {
        this.fieldList = fields;

        int totalsize = 0;
        foreach (int f in fieldSize) { totalsize += f; }
        fieldSizePct = new float[fieldSize.Length];
        for(int i = 0; i < fieldSizePct.Length;i++)
        {
            fieldSizePct[i] = ((float)fieldSize[i] / totalsize);
        }


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
        float xOffset = 0;
        if(columns.Count> 0) { fieldsCreated = true; }
        for (int i = 0;i<fields.Length; i++)
        {
            float widthPer = (r.rect.width * fieldSizePct[i]);
            float widthPerOld = r.rect.width / (fields.Length - 1);
            IData dat = data.GetValue(fields[i]);
            if (dat.Data != null&& !fieldsCreated)
            {
                switch(dat.Type)
                {
                    case DataLibSupportedTypes.text:
                        GameObject g = Instantiate(RowCellPrefab, containerObject.transform, false);
                        TextMeshProUGUI text = g.GetComponent<TextMeshProUGUI>();
                        text.text = dat.DisplayValue;
                        RectTransform rect = g.GetComponent<RectTransform>();
                        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,xOffset,widthPer);
                        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top,0, r.rect.height);
                        columns.Add(fields[i], g);
                        break;
                }
            }
            else
            {
                switch (dat.Type)
                {
                    case DataLibSupportedTypes.text:
                        columns.TryGetValue(fields[i], out GameObject g);
                        if (g != null)
                        {
                            TextMeshProUGUI text = g.GetComponent<TextMeshProUGUI>();
                            text.text = dat.DisplayValue;
                        }
                        break;
                }
            }
            xOffset += widthPer;
        }
        //create components.
    }

    void Update()
    {
        RectTransform r = GetComponent<RectTransform>();
        //Debug.Log(r.rect.width.ToString());
    }
    
}
