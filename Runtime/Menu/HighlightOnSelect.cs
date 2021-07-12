using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class HighlightOnSelect : MonoBehaviour
{
    // Use this for initialization
    private float fieldValue;//value to look for in stats.
    public string fieldName;
    public Image hlImage;
    public Color highlightedColor;
    public Color disabledColor;

    public void Init(IDataLibrary data,float fieldVal)
    {
        fieldValue = (float)fieldVal;
        data.AddListener(fieldName, UpdateHighlight);
        UpdateHighlight(data.GetValue(fieldName));
    }

    public void UpdateHighlight(IData stat)
    {
        if(Convert.ToBoolean(stat.Data))
        {
            hlImage.color = highlightedColor;
        }
        else
        {
            hlImage.color = disabledColor;
        }
    }
}
