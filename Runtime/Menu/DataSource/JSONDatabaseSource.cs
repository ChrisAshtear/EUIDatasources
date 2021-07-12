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
    public string URL; // load from here if loadFromURL true
    public Dictionary<string, string> remoteArguments = new Dictionary<string, string>();
    public bool loadFromURL = false;
    HttpWebRequest webRequest;
    public List<FieldPair> defaultRemoteArguments;
    //Props
    public int remoteSetIncremenAmt = 1;
    public string remotePageFieldName;

    public void SetupArguments()
    {
        remoteArguments = new Dictionary<string, string>();
        foreach(FieldPair pair in defaultRemoteArguments)
        {
            SetArgument(pair.fieldName, pair.fieldValue, false);
        }
    }

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
                DataSource table = new DataSource(obj.Name, primaryKey);
                JArray arr = (JArray)obj.Value;
                if(tables.ContainsKey(obj.Name))
                {
                    tables.Remove(obj.Name);
                }
                tables.Add(obj.Name, table);
                foreach (JToken t in arr.Children())
                {
                    Dictionary<string, object> row = JsonConvert.DeserializeObject<Dictionary<string, object>>(t.ToString());
                    table.data.Add(row[primaryKey].ToString(), row);
                }
                table.setReady();
            }

        }

        if (tables.Count > 0)
        {
            dataReady = true;
        }
        doOnDataReady();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public override void LoadData()
    {
        if (remoteArguments == null) { SetupArguments(); }
        //SetArgument(arg1Name, arg1Val, false);
        tables = new Dictionary<string, DataSource>();
        displayCodes = new Dictionary<string, string>();
        dataReady = false;

        Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>();

        try
        {
            if (loadFromURL)
            {
                string getDataUrl = "https://us-central1-warscrap-c63.cloudfunctions.net/api/" + URL + NetUtil.DictionaryToGetString(remoteArguments) ;
                NetUtil.DoWebRequest(getDataUrl, LoadFromString);
            }
            else
            {
                LoadFromString(JSON_file.text);
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
        curPage += remoteSetIncremenAmt * increment;
        SetArgument(remotePageFieldName, curPage.ToString());
    }

    public override void RequestNextSet()
    {
        RequestNewData(1);
    }

    public override void RequestPrevSet()
    {
        RequestNewData(-1);
    }
}
