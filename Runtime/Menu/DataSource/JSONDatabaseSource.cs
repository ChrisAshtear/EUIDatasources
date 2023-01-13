using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Xml.Linq;
using System;
using UnityEngine.Networking;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
//TODO: there should be a small add on script that extends the db source - so you can adjust a JSON/XML loader to adjust for how your API works. maybe an intermediary that handles requests for new information.
[Serializable]
public class FieldPair
{
    public string fieldName;
    public string fieldValue;
}

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DatabaseSource/JSONDatabaseSource", order = 1)]
public class JSONDatabaseSource : DBLoader
{

    //so the datasource should request new data if it gets a request to fill X entries that it doesnt have in its cache.
    //additionally, should the datasource cache data it already requested, and at what point should it refresh?
    //we need 1 table that requests 8 rows & another that requests 20. These logically should use the same datasource.

    private void Awake()
    {
        type = DataType.JSON;
    }

    protected override void LoadFromString(string data)
    {
        JObject dat = JObject.Parse(data);

        foreach (JProperty obj in dat.Properties())
        {
            if (obj.Value.Type == JTokenType.Array)
            {
                JArray arr = (JArray)obj.Value;
                
                DataSource table = new DataSource(obj.Name, primaryKey);
                
                if (tables.ContainsKey(obj.Name))
                {
                    if(appendLoad)
                    {
                        table = tables[obj.Name];
                    }
                    else
                    {
                        tables.Remove(obj.Name);
                        tables.Add(obj.Name, table);
                    }
                }
                else { tables.Add(obj.Name, table); }
                
                foreach (JToken t in arr.Children())
                {
                    Dictionary<string, object> row = JsonConvert.DeserializeObject<Dictionary<string, object>>(t.ToString());
                    //TODO:the dict shouldnt be accessed directly.
                    //TODO: get name and type of json 
                    string name = "noname";
                    string type = "notype";
                    DataItem item = new DataItem(name, type, row);
                    table.data.Add(row[primaryKey].ToString(), item);
                }
                table.setReady();
            }
            else
            {
                tables.TryGetValue("root", out DataSource rootTable);
                if (rootTable == null)
                {
                    DataSource table = new DataSource("root", primaryKey);
                    tables.Add("root", table);
                    rootTable = table;
                }
                if(obj.Value.Type == JTokenType.Object)
                {
                    Dictionary<string, object> jobj = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj.Value.ToString());
                    rootTable.SetAttribute(obj.Name, jobj);
                }
                else
                {
                    rootTable.SetAttribute(obj.Name, obj.Value);
                }
                
                         

            }
        }

        if (tables.Count > 0)
        {
            dataReady = true;
        }
        doOnDataReady();
     }

}
