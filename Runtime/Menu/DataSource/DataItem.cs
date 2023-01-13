using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UpdateData(Dictionary<string, object> data);

[Serializable]
public class DataItem
{
    public string Name;
    public string Type;
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

    //have to have a new data item added to a callback list in a datasource.

    public Dictionary<string,object> GetAllData()
    {
        //TODO: remove all users of this. this shouldnt be used.
        return new Dictionary<string, object>(vars);
    }

    public object GetValue(string varName)
    {
        bool exists = vars.ContainsKey(varName);
        if (exists) { return vars[varName]; }
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
//alternately manually define vars in a seperate class for each table.
//cant serialize dictionaries, but can serialize lists.