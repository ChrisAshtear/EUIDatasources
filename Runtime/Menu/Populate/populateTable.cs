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
//TODO: utility to clear all populated items when saving scene
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
    private int resultCount = 0;

    private List<UIRowListItem> allRows = new List<UIRowListItem>();

    private Dictionary<string, object> preservedData = new Dictionary<string, object>();

    public override void Populate()
    {
        Debug.Log(props);
        bool selected = false;

        bool createPrefabs = true;

        if(allRows.Count < 1 || allRows.Count != props.results+1)
        {
            Clear();
            allRows = new List<UIRowListItem>();
        }
        else { createPrefabs = false; }

        DataSource d;
        string primaryKey = "";
        {
            primaryKey = props?.data?.primaryKey;
            d = props?.data;
        }

        if (d == null) { return; }

        bool selectedAnItem = false;
        List<string> keys = d.GetFieldFromItems(primaryKey,props.results,resultCount);
        if(keys.Count == 0) { return; }
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

        int row = 0;
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

            GameObject obj;
            UIRowListItem listItem;
            DataLibrary dat;

            if (key == "*header*") { dat = headerData; }
            else { dat = new DataLibrary(props.data.getObjFromItemID(key)); }


            if (createPrefabs) 
            { 
                obj = Instantiate(prefab, layoutGroup.transform);
                obj.transform.parent = layoutGroup.transform;
                
                listItem = obj.GetComponent<UIRowListItem>();
                allRows.Add(listItem);
                if (odd || !striping) { dat.SetValue("bgColor", stripeColorOdd); }
                else { dat.SetValue("bgColor", stripeColorEven); }
            }
            else 
            { 
                obj = allRows[row].gameObject;
                listItem = allRows[row];
                
            }

            listItem.SetDataCustomFields(dat, fieldList, fieldWidth);
            listItem.Click = OnClick;
            listItem.source = props.data;


            obj.name = key;
            d.AddListener(key, listItem.SourceUpdate);
            

            

            foreach (KeyValuePair<string, object> de in preservedData)
            {
                dat.SetValue(de.Key, de.Value);
            }
            
            if (!selectedAnItem)
            {
                if (selectDefaultItem && (defaultSelection == -1 || key == defaultSelection.ToString()))
                {
                    listItem.Click?.Invoke(listItem);
                    selectedAnItem = true;
                }
            }
            odd = !odd;
            row++;
        }

    }

    public void NextPage()
    {
        resultCount += props.results;
        if(props.dataS.db.inputType != DataInputType.Remote)
        {
            Populate();
        }
        else
        {
            int newRecordsNeeded = props.dataS.db.RequestNextSet(props.dataS.tableName, props.results, resultCount);
            if (newRecordsNeeded == 0) { Populate(); }
            else { /*resultCount -= newRecordsNeeded; */}
        }
        
        //Remote - if not cached
        
    }

    public void PrevPage()
    {
        resultCount -= props.results;
        if(resultCount < 0) { resultCount = 0; }
        Populate();
    }
}