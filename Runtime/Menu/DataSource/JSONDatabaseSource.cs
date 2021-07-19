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
public interface IRemoteSource
{
    public void SetArgument(string field, string value,bool reload);


}

[Serializable]
public class FieldPair
{
    public string fieldName;
    public string fieldValue;
}

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DatabaseSource/JSONDatabaseSource", order = 1)]
public class JSONDatabaseSource : DatabaseSource,IRemoteSource
{
    //if we can use a text asset here & also support an assetbundle/web request that would be ideal.
    public TextAsset JSON_file; // load from here if loadFromURL false
    public string rootURL;// the root web address.
    public string URL; // the file to load - load from here if loadFromURL true
    public Dictionary<string, string> remoteArguments = new Dictionary<string, string>();
    public bool loadFromURL = false;
    public NetRequestType webRequestType = NetRequestType.GET;

    HttpWebRequest webRequest;
    public List<FieldPair> defaultRemoteArguments;
    //Props
    public int remoteSetIncremenAmt = 1;
    public string remotePageFieldName;

    private bool appendLoad = false;

    public void SetupArguments()
    {
        remoteArguments = new Dictionary<string, string>();
        foreach(FieldPair pair in defaultRemoteArguments)
        {
            SetArgument(pair.fieldName, pair.fieldValue, false);
        }
    }

    //so the datasource should request new data if it gets a request to fill X entries that it doesnt have in its cache.
    //additionally, should the datasource cache data it already requested, and at what point should it refresh?
    //we need 1 table that requests 8 rows & another that requests 20. These logically should use the same datasource.

    private void OnEnable()
    {
        SetupArguments();
    }

    private void Awake()
    {
        
        //remoteArguments = new Dictionary<string, string>();
        type = DataType.JSON;
    }

    public void LoadFromString(string data)
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
                    table.data.Add(row[primaryKey].ToString(), row);
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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    protected override bool LoadData()
    {
        if (remoteArguments == null) { SetupArguments(); }
        if (!appendLoad) 
        { 
            tables = new Dictionary<string, DataSource>(); 
            DataSource table = new DataSource("root", primaryKey);
            tables.Add("root", table);
        }
        
        displayCodes = new Dictionary<string, string>();
        dataReady = false;

        Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>();

        try
        {
            if (loadFromURL)
            {
                appendLoad = true;
                string getDataUrl = rootURL + URL;
                NetUtil.DoWebRequest(getDataUrl, remoteArguments,webRequestType , LoadFromString);
                return false;
            }
            else
            {
                LoadFromString(JSON_file.text);
                return true;
            }
            
        }
        catch (Exception e)
        {
            if (e.Message.Contains("dictionary"))
            {
                loadStatus = "key not found";
            }
            else
            {
                loadStatus = e.Message;
            }
            return false;
        }
    }

    public void SetArgument(string field, string value, bool reload = true)
    {
        if(remoteArguments.ContainsKey(field))
        {
            remoteArguments[field] = value;
        }
        else
        {
            remoteArguments.Add(field, value);
        }
        if(reload)
        {
            LoadData();
        }
    }

    public void RequestNewData(int increment)
    {
        remoteArguments.TryGetValue(remotePageFieldName, out string pageVal);
        int curPage = int.Parse(pageVal);
        curPage += increment;
        SetArgument(remotePageFieldName, curPage.ToString());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="amtOfRecords"></param>
    /// <param name="recordOffset"></param>
    /// <returns>if the data that was requested is currently cached.</returns>
    public override int RequestNextSet(string tableName,int amtOfRecords, int recordOffset)
    {
        DataSource d = getTable(tableName);
        int numEntries = d.data.Values.Count;

        if(recordOffset+amtOfRecords > numEntries)
        {
            int newRecordsNeeded = (recordOffset + amtOfRecords) - numEntries;
            RequestNewData(newRecordsNeeded);
            return newRecordsNeeded;
        }
        else
        {
            return 0;
        }
    }
    /*
    public override void RequestPrevSet(int amtOfRecords, int recordOffset)
    {
        RequestNewData(-recordOffset);
    }*/
}
