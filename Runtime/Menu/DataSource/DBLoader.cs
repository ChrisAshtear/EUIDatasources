using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

public class DBLoader : DatabaseSource
{
    public TextAsset local_file; // load from here if loadFromURL false
    public string rootURL;// the root web address.
    public string URL; // the file to load - load from here if loadFromURL true
    public Dictionary<string, string> remoteArguments = new Dictionary<string, string>();
    public bool loadFromURL = false;
    public NetRequestType webRequestType = NetRequestType.GET;

    HttpWebRequest webRequest;
    public List<FieldPair> defaultRemoteArguments = new List<FieldPair>();
    //Props
    public int remoteSetIncremenAmt = 1;
    public string remotePageFieldName;

    protected bool appendLoad = false;

    public string filePath = "";

    public void SetupArguments()
    {
        remoteArguments = new Dictionary<string, string>();
        foreach (FieldPair pair in defaultRemoteArguments)
        {
            SetArgument(pair.fieldName, pair.fieldValue, false);
        }
    }

    //need to support loading from local path. streaming assets.

    //so the datasource should request new data if it gets a request to fill X entries that it doesnt have in its cache.
    //additionally, should the datasource cache data it already requested, and at what point should it refresh?
    //we need 1 table that requests 8 rows & another that requests 20. These logically should use the same datasource.

    private void OnEnable()
    {
        SetupArguments();
    }


    public void SetArgument(string field, string value, bool reload = true)
    {
        if (remoteArguments.ContainsKey(field))
        {
            remoteArguments[field] = value;
        }
        else
        {
            remoteArguments.Add(field, value);
        }
        if (reload)
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
    public override int RequestNextSet(string tableName, int amtOfRecords, int recordOffset)
    {
        DataSource d = getTable(tableName);
        int numEntries = d.data.Values.Count;

        if (recordOffset + amtOfRecords > numEntries)
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

    protected virtual void LoadFromString(string data)
    {

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
            switch (inputType)
            {
                case DataInputType.Asset:
                    LoadFromString(local_file.text);
                    return true;
                    break;

                case DataInputType.Moddable:
                    //We can stream and this can be user modifiable.
                    string path = Application.streamingAssetsPath;
                    int index = Application.streamingAssetsPath.LastIndexOf("/");
                    if (index >= 0) { path = path.Substring(0, index + 1); }
                    index = filePath.IndexOf("StreamingAssets/");
                    path += filePath.Substring(index);

                    string filedata = File.ReadAllText(path);
                    LoadFromString(filedata);
                    return true;
                    break;

                case DataInputType.Remote:
                    appendLoad = true;
                    string getDataUrl = rootURL + URL;
                    NetUtil.DoWebRequest(getDataUrl, remoteArguments, webRequestType, LoadFromString);
                    return false;
                    break;
            }
            return false;

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
}

