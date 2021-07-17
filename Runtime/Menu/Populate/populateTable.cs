using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity;
using UnityEngine.Events;
using UnityEditor;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

//TODO: this should be sortable. the cell containing each header column should have a button added to it to sort the contents.
public class populateTable : populateData, I_ItemMenu
{
    
    [Tooltip("Alternate color for odd and even rows")]
    public bool striping = true;
    public bool sortable = true;
    public bool showHeader = true;
    public Color stripeColorOdd;
    public Color stripeColorEven;
    public Color headerColor;
    [Tooltip("Comma seperated list of fields to display, in that order.")]
    public string fieldList;
    private int[] fieldWidth;

    private Dictionary<string, object> preservedData = new Dictionary<string, object>();

    public override void Populate()
    {
        Debug.Log(props);
        bool selected = false;

        Clear();

        DataSource d;
        string primaryKey = "";
        {
            primaryKey = props?.data?.primaryKey;
            d = props?.data;
        }

        if (d == null) { return; }

        bool selectedAnItem = false;
        List<string> keys = d.getFieldFromAllItems(primaryKey);

        IEnumerable<SourceFilter> allFilters = filters.Concat(permanentFilter);

        if (d.displayCode != null)
        {
            displayCode = d.displayCode;
        }
        if(props.detailsContainer != null)
        {
            UIDisplayCodeController dc = props.detailsContainer.GetComponent<UIDisplayCodeController>();
            if (dc != null)
            {
                dc.displayCode = displayCode;
            }
        }

        bool odd = true;

        string[] fieldArr = fieldList.Split(',');
        //if (fieldWidth == null)
        {
            
            fieldWidth = new int[fieldArr.Length];

            DataLibrary sample = new DataLibrary(props.data.getObjFromItemID(keys[0]));
            for (int i = 0; i < fieldArr.Length; i++)
            {
                string val = sample.GetTxtValue(fieldArr[i]);
                fieldWidth[i] = val.Length;
                if (fieldWidth[i] < 4) { fieldWidth[i] = 4; }
            }
        }
        

        DataLibrary headerData = new DataLibrary();
        if (showHeader)
        {
            foreach (string field in fieldArr)
            {
                headerData.SetValue(field, field.ToUpper());
            }
            keys.Insert(0, "*header*");
        }

        foreach (string key in keys)
        {
            bool filtered = false;
            foreach (SourceFilter filter in allFilters)
            {
                if (key == "0" && showEmptyItem) { continue; }
                string value = d.getFieldFromItemID(key, filter.filterVar);
                if (!filter.MatchesFilter(value))
                {
                    filtered = true;
                }
            }
            if (filtered) { continue; }

            GameObject obj = Instantiate(prefab, layoutGroup.transform);
            obj.transform.parent = layoutGroup.transform;
            obj.name = key;
            UIRowListItem listItem = obj.GetComponent<UIRowListItem>();
            d.AddListener(key, listItem.SourceUpdate);
            DataLibrary dat;

            if(key == "*header*") { dat = headerData; }
            else { dat = new DataLibrary(props.data.getObjFromItemID(key)); }
            

            if (odd || !striping) { dat.SetValue("bgColor", stripeColorOdd); }
            else { dat.SetValue("bgColor", stripeColorEven); }

            foreach (KeyValuePair<string, object> de in preservedData)
            {
                dat.SetValue(de.Key, de.Value);
            }
            listItem.SetDataCustomFields(dat,fieldList,fieldWidth);
            listItem.Click = OnClick;
            listItem.source = props.data;
            if (!selectedAnItem)
            {
                if (selectDefaultItem && (defaultSelection == -1 || key == defaultSelection.ToString()))
                {
                    listItem.Click?.Invoke(listItem);
                    selectedAnItem = true;
                }
            }
            odd = !odd;
        }

    }

    public void NextPage()
    {
        props.dataS.db.RequestNextSet();
    }

    public void PrevPage()
    {
        props.dataS.db.RequestPrevSet();
    }
}