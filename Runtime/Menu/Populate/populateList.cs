using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity;
using UnityEngine.Events;
using UnityEditor;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


public interface I_ItemMenu
{
    void SetSelected(GameObject obj);
    GameObject GetSelected();
}

[System.Serializable]
public class OnSelectListEvent : UnityEvent<selectListItem>
{
}


public class selectListItem
{
    //public FillFromSource fill;//reference to selected obj
    public string index;//index of selected item
}



// Custom serializable class
//[Serializable]

[System.Serializable]
public class SourceProps
{
    public DatabaseSource db;
    public string tableName;
    public string ID;
    public DataSource source { get { return db.getTable(tableName); } }
}

public class populateList : populateData, I_ItemMenu
{
    public listFunction onSelect = listFunction.Form;
    public GameObject displayObj;

    private Dictionary<string,object> preservedData = new Dictionary<string, object>();

    public override void Populate()
    {
        
        bool selected = false;

        Clear();

        //if(displayObj == null) { displayObj = gameObject;}

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

        if (displayObj != null)
        {
            UIDisplayCodeController dc = displayObj.GetComponent<UIDisplayCodeController>();
            if (dc != null)
            {
                dc.displayCode = displayCode;
            }
        }
        

        foreach (string key in keys)
        {
            bool filtered = false;
            if ((key == "0" || key =="none") && !showEmptyItem) { continue; }
            foreach (SourceFilter filter in allFilters)
            {
                
                string value = d.getFieldFromItemID(key, filter.filterVar);
                if(!filter.MatchesFilter(value))
                {
                    filtered = true;
                }
            }
            if (filtered) { continue; }

            GameObject obj = Instantiate(prefab, layoutGroup.transform);
            
            obj.transform.parent = layoutGroup.transform;
            obj.name = key;
            if (!obj.TryGetComponent<UIButtonListItem>(out var listItem))
            {
                Debug.LogError("No UI button list item found on " + obj.name);
                continue;
            }
            //UIButtonListItem listItem = obj.GetComponent<UIButtonListItem>();
            d.AddListener(key, listItem.SourceUpdate);
            DataLibrary dat = new DataLibrary(props.data.getObjFromItemID(key));

            foreach (KeyValuePair<string, object> de in preservedData)
            {
                dat.SetValue(de.Key, de.Value);
            }
            listItem.SetData(dat);
            listItem.Click = OnClick;
            listItem.source = props.data;
            if(!selectedAnItem)
            {
                if(selectDefaultItem &&(  defaultSelection == -1 || key == defaultSelection.ToString()))
                {
                    listItem.Click?.Invoke(listItem);
                    selectedAnItem = true;
                }
            }
            
        }

    }

    public void OnClick(UIButtonListItem listItem)
    {
        props.evData.Invoke(listItem.GetData());
    }

}
