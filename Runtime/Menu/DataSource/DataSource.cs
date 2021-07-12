using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public enum DataType {  XML, SQL,JSON,Choice, Web, Realtime}; // we need custom handlers for all these.

public enum fieldType { str, integer, flt };
[System.Serializable]
public class DataSource
{
    //public string name;
    public string sourceName;
    public string addressOfData;
    protected bool dataReady = false;
    //public DataType type;

    public delegate void DataReadyHandler();
    public event DataReadyHandler onDataReady;

    //protected List<Dictionary<string, string>> data;
    public Dictionary<string,Dictionary<string, object>> data; //(primaryKey,(obj[fieldname/fieldval])
    public string name;
    public string primaryKey = "";
    [TextArea(3, 10)]
    public string displayCode = ""; // used for displaying an item with displayObj

    protected string selectedKey = "NA";

    public delegate void SelectionChangedHandler();
    public event SelectionChangedHandler selectionChanged;

    public delegate void DataChangedHandler();
    public event DataChangedHandler dataChanged;

    public Action<DataSource> datChanged;
    public Dictionary<string,Action<string,object>> fieldChanged = new Dictionary<string, Action<string, object>>();
    public Action<DataSource> selectChanged;
    public Sprite spritesheet;

    public DataSource()
    {
        //condition = maxCond;
    }

    public DataSource(string name,string key)
    {
        primaryKey = key;
        this.name = name;
        data = new Dictionary<string, Dictionary<string, object>>();
    }

    private void OnDestroy()
    {
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
       bool found = data.TryGetValue(key, out Dictionary<string, object> obj);
       if (!found) { return; }
       if(obj.ContainsKey(fieldName))
        {
            obj[fieldName] = value;
        }
       else
        {
            obj.Add(fieldName, value);
        }
        fieldChanged.TryGetValue(key, out Action<string,object> callback);
        callback?.Invoke(fieldName,value);
    }
    private void OnEnable()
    {

    }

    public bool isDataReady()
    {
        if(data == null)
        {
            return false;
        }
        return dataReady;
    }

    public virtual void LoadData()
    {
    }

    protected void doOnDataReady()
    {
        if(onDataReady != null)
        {
            onDataReady();
        }
        
    }

    public IReadOnlyList<Dictionary<string, object>> GetAllSortedByField(string fieldName, bool descending = false)//ordered list. 
    {
        List<Dictionary<string, object>> sorted = new List<Dictionary<string, object>>();
        sorted = data.Values.OrderBy(o => o[fieldName]).ToList();
        return sorted;
    }

    public void setReady()
    {
        dataReady = true;
    }

    public virtual string getRandomFieldVal(string fieldName)
    {
        if (dataReady)
        {
            List<Dictionary<string, object>> vals = data.Values.ToList();
            Dictionary<string, object> entry = vals[UnityEngine.Random.Range(0, vals.Count)];

            object val;
            entry.TryGetValue(fieldName, out val);

            return val.ToString() ?? "none";
        }
        else
        {
            return "not ready";
        }
    }

    public virtual string getSelectedVal(string fieldName)
    {
        Dictionary<string, object> entry = getSelected();
        if(entry != null)
        {
            if (entry.ContainsKey(fieldName))
            {
                return entry[fieldName].ToString();
            }
        }
        return "Not Found";
    }

    public virtual Dictionary<string, object> getSelected()
    {
        if(selectedKey == "NA" && dataReady && data.Count > 0)
        {
            selectedKey = data.Keys.ToList()[0];
        }
        if (dataReady && data != null && data.ContainsKey(selectedKey))
        {
            Dictionary<string, object> entry = data[selectedKey];
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
            selectChanged?.Invoke(this);
            if (selectionChanged != null)
            {
                selectionChanged();
            }
        }
    }

    protected void fireSelectionChanged()
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

    public virtual Dictionary<string,fieldType> getFields()
    {
        return new Dictionary<string, fieldType>();
    }


    public virtual List<string> getFieldSimple()
    {
        if (data != null)
        {
            List<string> fields = new List<string>();
            foreach (Dictionary<string, object> dict in data.Values)
            {
                foreach (KeyValuePair<string, object> entry in dict)
                {
                    if (!fields.Contains(entry.Key))
                    {
                        fields.Add(entry.Key);
                    }
                    // do something with entry.Value or entry.Key
                }
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
            foreach (Dictionary<string, object> dict in data.Values)
            {
                object val = "none";
                dict.TryGetValue(field, out val);

                allVals.Add(val.ToString());
                // do something with entry.Value or entry.Key
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



            foreach (Dictionary<string, object> dict in data.Values)
            {
                object val = "none";
                object key = "none";
                dict.TryGetValue(field, out val);
                dict.TryGetValue(primaryKey, out key);

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



            foreach (Dictionary<string, object> dict in data.Values)
            {
                object val = "none";
                object key = "none";
                dict.TryGetValue(field, out val);
                dict.TryGetValue(primaryKey, out key);

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
            data.TryGetValue(id, out Dictionary<string, object> dict);

            object val;
            dict.TryGetValue(field, out val);

            return val ?? "none";
        }
        return "";
    }

    public virtual Dictionary<string,object> getObjFromItemID(string id)
    {
        if (dataReady/* && id > 0 && id < data.Count*/)
        {
            data.TryGetValue(id, out Dictionary<string, object> dict);
            if (dict == null) { dict = new Dictionary<string, object>(); }
            return dict;
        }
        return new Dictionary<string, object>();
    }

    public virtual string setFieldFromItemID(string id, string field,string value)
    {
        return "";
    }

    public virtual float GetMaxValueFromField(string field)
    {
        //TODO:store these values so it doesnt get them every time.
        float highestVal = 0;
        foreach(Dictionary<string,object> entry in data.Values)
        {
            entry.TryGetValue(field, out object fieldValue);
            if(fieldValue == null) { continue; }
            float val = (float)Convert.ToDouble(fieldValue);
            if(val>highestVal)
            {
                highestVal = val;
            }
        }
        return highestVal;
    }

    public virtual Dictionary<string,object> getFieldObjsFromItemID(string id)
    {
        if (dataReady/* && id > 0 && id < data.Count*/)
        {

            Dictionary<string, object> dict;
            data.TryGetValue(id,out dict);
            if (dict != null)
            {
                return dict;
            }

        }
        return new Dictionary<string, object>();
    }

    public virtual Dictionary<string, object> getFieldsFromItemID(string id)
    {
        data.TryGetValue(id, out Dictionary<string, object> fields);
        return fields;
    }
}
