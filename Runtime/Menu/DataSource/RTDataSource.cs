using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;
/*
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RealTimeDataSource", order = 1)]
[System.Serializable]
public class RTDataSource : DataSource
{
    public string elementToLoad = "";
    public float updateTime = 5;
    public bool useListIndexAsKey = true;
    //in seconds
    private object sourceData;

    private System.Timers.Timer updateTimer = new Timer();

    public RTDataSource()
    {
        //WorldFile w = new WorldFile(XDocument.Load(f.FullName).Element("WorldFile"), f.Directory.FullName);
        //condition = maxCond;
    }

    public void SetData(object data, bool update = false)
    {
        dataReady = false;
        if (!update)
        {
            selectedKey = "NA";
        }

        if (data != null)
        {
            sourceData = data;
            this.data = new Dictionary<string, Dictionary<string, object>>();
            //XDocument doc = XDocument.Load(Application.dataPath + "/Resources/" + sourceName);
            if (data is IDictionary)
            {
                foreach (DictionaryEntry entry in (IDictionary)data)
                {
                    Dictionary<string, object> fields;
                    fields = entry.Value.GetType().GetFields().ToDictionary(prop => prop.Name, prop => prop.GetValue(entry.Value));
                    this.data.Add(entry.Key.ToString(), fields);
                }
            }
            if (data is IList)
            {
                IList genList = (IList)data;
                for (int i = 0; i < genList.Count; i++)
                {
                    Dictionary<string, object> fields;

                    //IF NAME ISNT DEFINED, THIS WILL CAUSE EXCEPTION
                    fields = genList[i].GetType().GetFields().ToDictionary(prop => prop.Name, prop => prop.GetValue(genList[i]));
                    if (useListIndexAsKey)
                    {
                        this.data.Add(i.ToString(), fields);
                    }
                    else
                    {
                        this.data.Add(fields[primaryKey].ToString(), fields);
                    }
                }
            }
            if (!this.data.ContainsKey(selectedKey))
            {
                selectedKey = "NA";
            }
            dataReady = true;
        }
    }

    public override string setFieldFromItemID(string id, string field, string value)
    {
        if (dataReady/* && id > 0 && id < data.Count*//*)
        {
            Dictionary<string, object> dict = data[id];

            string val;
            if (dict.ContainsKey(field))
            {
                dict[field] = value;
                return "success";
            }
            else
            {
                return "field not found";
            }
        }
        return "Data Not Ready";
        //this will require a save operation, as any changed data will be lost on exit.
    }

    public void updateData()
    {
        if (sourceData != null)
        {
            SetData(sourceData, true);
            changedData();
        }
    }

    public object getRawData()
    {
        return sourceData;
    }

    private void Awake()
    {
    }

    private void OnDisable()
    {
    }

    private void OnEnable()
    {
    }
}*/