using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UpdateData(Dictionary<string, object> data);
//DataPointer for inspector, extends dataItem?
[System.Serializable]
public class DataItem
{
    public string Name;
    public string Type;
    [System.NonSerialized]
    Dictionary<string, object> vars = new Dictionary<string, object>();

    public DataItem(string name, string type, Dictionary<string, object> vars)
    {
        Name = name;
        Type = type;
        this.vars = new Dictionary<string, object>(vars);
    }

    public DataItem()
    {
        Name = "none";
        Type = "none";
        this.vars = new Dictionary<string, object>(vars);
    }

    protected void SetVals(Dictionary<string, object> vars)
    {
        this.vars = new Dictionary<string, object>(vars);
    }
    //have to have a new data item added to a callback list in a datasource.

    public Dictionary<string,object> GetAllData()
    {
        //TODO: remove all users of this. this shouldnt be used.
        return new Dictionary<string, object>(vars);
    }

    public object GetValue(string varName)
    {
        bool exists = vars.ContainsKey(varName);
        bool existsL = vars.ContainsKey(varName.ToLower());
        if (exists) { return vars[varName]; }
        else if(existsL) { return vars[varName.ToLower()]; }
        else { return null; }
    }

    public void SetValue(string varName, object value)
    {
        bool exists = vars.ContainsKey(varName);
        if (exists) { vars[varName] = value; }
        else { vars.Add(varName,value); }
    }

    public List<string> GetFields()
    {
        List<string> fields = new List<string>();
        foreach(KeyValuePair<string, object> item in vars)
        {
            fields.Add(item.Key); 
        }
        return fields;
    }
}

[System.Serializable]
public class DataItemContent : DataItem, ISerializationCallbackReceiver
{
    [SerializeField] public DatabaseSource db;
    [SerializeField] public string tableName;
    [SerializeField] public string ID;
    [SerializeField] public string itemUUID;

    bool loaded = false;

    DataItemContent(DatabaseSource db, string tableName, string iD, string itemUUID)
    {
        this.db = db;
        this.tableName = tableName;
        ID = iD;
        this.itemUUID = itemUUID;
        Load();
        
    }

    public bool Load()
    {
        if (db == null) return false;
        DataItem item = db.getTable(tableName).getObjFromItemID(itemUUID);
        if (item == null) { return false; }
        Name = item.Name;
        Type = item.Type;
        SetVals(item.GetAllData());
        return true;
    }

    public void OnAfterDeserialize()
    {
        Load();
        //throw new NotImplementedException();
    }

    public void OnBeforeSerialize()
    {
        //Load();
        //throw new NotImplementedException();
    }
}
//alternately manually define vars in a seperate class for each table.
//cant serialize dictionaries, but can serialize lists.