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
public class ListProps : DataProps
{
    public listFunction onSelect = listFunction.Form;
    public listObjProps displayObj;
}
[System.Serializable]
public class SourceProps
{
    public DatabaseSource db;
    public string tableName;
    public string ID;
}

public class populateList : populateData, I_ItemMenu
{

    public new ListProps props;

    private Dictionary<string,object> preservedData = new Dictionary<string, object>();

    public override void Populate()
    {
        
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
        UIDisplayCodeController dc = props.displayObj.GetComponent<UIDisplayCodeController>();
        if (dc != null)
        {
            dc.displayCode = displayCode;
        }

        foreach (string key in keys)
        {
            bool filtered = false;
            foreach (SourceFilter filter in allFilters)
            {
                if(key == "0" && showEmptyItem){ continue; }
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
            UIButtonListItem listItem = obj.GetComponent<UIButtonListItem>();
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
