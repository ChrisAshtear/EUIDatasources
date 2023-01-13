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

    //have to have a new data item added to a callback list in a datasource.


}
//alternately manually define vars in a seperate class for each table.
//cant serialize dictionaries, but can serialize lists.