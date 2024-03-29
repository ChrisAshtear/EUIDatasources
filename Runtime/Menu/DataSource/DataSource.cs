﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using static UnityEditor.Progress;

public enum DataType {  XML, SQL,JSON,Choice, Web, Realtime}; // we need custom handlers for all these.

[System.Serializable]
public class DataSource
{
    protected bool dataReady = false;

    public delegate void DataReadyHandler();
    public event DataReadyHandler onDataReady;

    public Dictionary<string,DataItem> data; //(primaryKey,(obj[fieldname/fieldval])
    public string name;
    public string primaryKey = "";
    public string displayCode = ""; // used for displaying an item with displayObj

    protected string selectedKey = "NA";

    public event DataChangedHandler selectionChanged;

    public delegate void DataChangedHandler();
    public event DataChangedHandler dataChanged;

    public Action<DataSource> datChanged;
    public Dictionary<string,Action<string,object>> fieldChanged = new Dictionary<string, Action<string, object>>();
    public Action<DataSource> selectChanged;
    public Sprite spritesheet;

    public DataLibrary attributes;

    private List<string> entriesIndex;

    public DataSource(string name = "noname",string key = "none")
    {
        primaryKey = key;
        this.name = name;
        data = new Dictionary<string, DataItem>();
        attributes = new DataLibrary(); 
    }

    public IData GetAttribute(string fieldName)
    {
        return attributes.GetValue(fieldName);
    }

    public void SetAttribute(string fieldName, object value)
    {
        attributes.SetValue(fieldName, value);
    }

    public DataLibrary GetAllAttributes()
    {
        return attributes;
    }

    public void AddListener(string key, Action<string,object> callback)
    {
        if(fieldChanged.ContainsKey(key))
        {
            fieldChanged[key] += callback;
        }
        else
        {
            fieldChanged.Add(key, callback);
        }
    }

    public void RemoveListener(string key, Action<string, object> callback)
    {
        if (key == null) { return; }
        if (fieldChanged.ContainsKey(key))
        {
            fieldChanged[key] -= callback;
        }
    }

    public void setField(string key, string fieldName,object value)
    {
        //check if source is read only here.
       bool found = data.TryGetValue(key, out DataItem obj);
       if (!found) { return; }
       obj.SetValue(fieldName,value);

        fieldChanged.TryGetValue(key, out Action<string,object> callback);
        callback?.Invoke(fieldName,value);
    }

    public bool isDataReady()
    {
        if(data == null)
        {
            return false;
        }
        return dataReady;
    }

    protected void doOnDataReady()
    {
        entriesIndex = getFieldFromAllItems(primaryKey);

        if (onDataReady != null)
        {
            onDataReady();
        }
        
    }

    public IReadOnlyList<Dictionary<string, object>> GetAllSortedByField(string fieldName, bool descending = false)//ordered list. 
    {
        List<Dictionary<string, object>> sorted = new List<Dictionary<string, object>>();
        //TODO fix sorting
        //sorted = data.Values.OrderBy(o => o[fieldName]).ToList();
        return sorted;
    }

    public void setReady()
    {
        dataReady = true;
        doOnDataReady();
    }

    public virtual string getRandomFieldVal(string fieldName)
    {
        if (dataReady)
        {
            List<DataItem> vals = data.Values.ToList();
            DataItem entry = vals[UnityEngine.Random.Range(0, vals.Count)];

            object val = entry.GetValue(fieldName);

            return val.ToString() ?? "none";
        }
        else
        {
            return "not ready";
        }
    }

    public virtual string getSelectedVal(string fieldName)
    {
        DataItem entry = getSelected();
        if(entry != null)
        {
            object val = entry.GetValue(fieldName);
            if (val!=null)
            {
                return val.ToString();
            }
        }
        return "Not Found";
    }

    public virtual DataItem getSelected()
    {
        if(selectedKey == "NA" && dataReady && data.Count > 0)
        {
            selectedKey = data.Keys.ToList()[0];
        }
        if (dataReady && data != null && data.ContainsKey(selectedKey))
        {
            DataItem entry = data[selectedKey];
            return entry;
        }
        else
        {
            return null;
        }
    }

    public virtual object getSelectedAsObject()
    {
        if (selectedKey == "NA" && dataReady && data.Count > 0)
        {
            selectedKey = data.Keys.ToList()[0];
        }
        if (dataReady && data != null && data.ContainsKey(selectedKey))
        {
            return data[selectedKey];
        }
        return null;
    }

    public virtual void changeSelection(int amt,bool doEvent = true)
    {
        
        List<string> keys = data.Keys.AsEnumerable().ToList();
        if(selectedKey == "NA")
        {
            selectedKey = keys[0];
            return;
        }
        int curIdx = keys.IndexOf(selectedKey);
        curIdx += amt;
        if(curIdx >= keys.Count)
        {
            curIdx = 0;
        }
        if(curIdx < 0)
        {
            curIdx = keys.Count-1;
        }
        selectedKey = keys[curIdx];
        if(doEvent)
        {
            fireSelectionChanged();
        }
    }

    public void fireSelectionChanged()
    {
        selectChanged?.Invoke(this);
        if (selectionChanged != null)
        {
            selectionChanged();
        }
    }

    public void changedData()
    {
        datChanged?.Invoke(this);
        if(dataChanged != null && selectedKey != "NA")
        {
            dataChanged();
        }
    }

    public virtual void selectPrev()
    {
        changeSelection(-1);
    }
    public virtual void selectNext()
    {
        changeSelection(1);
    }

    public virtual string getSelectedKey()
    {
        if (selectedKey == "NA" && dataReady && data.Count > 0)
        {
            selectedKey = data.Keys.ToList()[0];
        }
        return selectedKey;
    }

    public virtual void selectItem(string key)
    {
        selectedKey = key;
        selectChanged?.Invoke(this);
        if (selectionChanged != null)
        {
            selectionChanged();
        }
    }

    public virtual List<string> getFieldSimple()
    {
        if (data != null)
        {
            //
            List<string> fields = new List<string>();
            foreach (DataItem item in data.Values)
            {
                List<string> fieldList = item.GetFields();
                fields = fields.Union(fieldList).ToList();
            }
            return fields;
        }
        return new List<string>();
    }

    public virtual List<string> getFieldFromAllItems(string field)
    {
        if (dataReady && data != null)
        {
            List<string> allVals = new List<string>();
            foreach (DataItem item in data.Values)
            {
                object val = item.GetValue(field);
                if(val!=null)
                { allVals.Add(val.ToString()); }
                else { Debug.LogError("DataSource-GetFieldFromAllItems failed: "+field+" not found." + name + "-" + GetHashCode());}
                // do something with entry.Value or entry.Key
            }
            return allVals;
        }
        return new List<string>();
    }

    /// <summary>
    /// Gets a list of values from a field from items in the datasource.
    /// </summary>
    /// <param name="field">field name</param>
    /// <param name="results">#of results. -1 is all</param>
    /// <param name="offset">how many results to skip</param>
    /// <returns>a string list of values</returns>
    public virtual List<string> GetFieldFromItems(string field,int results=-1,int offset=0)
    {
        //primary keys should be in a list.
        if (dataReady && data != null)
        {
            List<string> allVals = new List<string>();
            //TODO: if remote DB, check if we can get more results
            if (results == -1 || data.Values.Count < results) { results = data.Values.Count; }
            else { results += offset; }
            for(int i = offset;i<results;i++)
            {
                string key = entriesIndex[i];
                if(field == primaryKey) { allVals.Add(entriesIndex[i]); }
                else
                {
                    object val = data[key].GetValue(field);
                    if (val != null)
                    { allVals.Add(val.ToString()); }
                }
            }
            return allVals;
        }
        return new List<string>();
    }

    public virtual Dictionary<string, string> getFieldFromAllItemsKeyed(string field, bool uniqueOnly = false, bool sort = false)
    {
        if (dataReady)
        {
            Dictionary<string, string> allVals = new Dictionary<string, string>();

            foreach (DataItem item in data.Values)
            {
                object val = item.GetValue(field) ?? "none";
                object key = item.GetValue(primaryKey) ?? "none";

                if (uniqueOnly && allVals.ContainsKey(val.ToString()))
                {
                    continue;
                }
                allVals.Add(val.ToString(), key.ToString());
                // do something with entry.Value or entry.Key
            }

            if (sort)
            {
                //this seems the easiest way to sort.
                var items = from pair in allVals
                            orderby pair.Key ascending
                            select pair;

                return items.ToDictionary(t => t.Key, t => t.Value);
            }
            else
            {
                return allVals;
            }

        }
        return new Dictionary<string, string>();
    }

    public virtual Dictionary<string, string> getFieldFromAllItemsKeyedR(string field, bool uniqueOnly = false, bool sort = false) // what is this for?
    {
        if (dataReady)
        {
            Dictionary<string, string> allVals = new Dictionary<string, string>();



            foreach (DataItem item in data.Values)
            {
                object val = item.GetValue(field) ?? "none";
                object key = item.GetValue(primaryKey) ?? "none";

                if (uniqueOnly && allVals.ContainsKey(val.ToString()))
                {
                    continue;
                }
                allVals.Add(key.ToString(), val.ToString());
                // do something with entry.Value or entry.Key
            }

            if (sort)
            {
                //this seems the easiest way to sort.
                var items = from pair in allVals
                            orderby pair.Key ascending
                            select pair;

                return items.ToDictionary(t => t.Key, t => t.Value);
            }
            else
            {
                return allVals;
            }

        }
        return new Dictionary<string, string>();
    }
    public virtual string getFieldFromItemID(string id, string field)
    {
        return getFieldObjFromItemID(id, field).ToString();
    }

    public virtual bool containsID(string id)
    {
        return data.ContainsKey(id);
    }

    public virtual object getFieldObjFromItemID(string id, string field)
    {
        if (dataReady/* && id > 0 && id < data.Count*/)
        {
            data.TryGetValue(id, out DataItem item);

            object val = item.GetValue(field);

            return val ?? "none";
        }
        return "";
    }

    public virtual DataItem getObjFromItemID(string id)
    {
        if (dataReady/* && id > 0 && id < data.Count*/)
        {
            data.TryGetValue(id, out DataItem item);
            if (item != null) { return item; }
        }
        return new DataItem();
    }

    public virtual float GetMaxValueFromField(string field)
    {
        //TODO:store these values so it doesnt get them every time.
        float highestVal = 0;
        foreach(DataItem item in data.Values)
        {
            object fieldValue = item.GetValue(field);
            if(fieldValue == null) { continue; }
            float val = (float)Convert.ToDouble(fieldValue);
            if(val>highestVal)
            {
                highestVal = val;
            }
        }
        return highestVal;
    }

    public virtual DataItem getFieldObjsFromItemID(string id)
    {
        if (dataReady)
        {

            DataItem item;

            data.TryGetValue(id,out item);
            if (item != null)
            {
                return item;
            }

        }
        return new DataItem();
    }

    public virtual DataItem getFieldsFromItemID(string id)
    {
        data.TryGetValue(id, out DataItem item);
        return item;
    }

}
