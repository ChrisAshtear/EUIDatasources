using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class RobotComponentDatabank : MonoBehaviour, IJsonData
{
    public TextAsset[] jsonFiles;
    private static Dictionary<string, JToken> parsedConfigs = new Dictionary<string, JToken>();
    public static RobotComponentDatabank ins;
    // Start is called before the first frame update
    void Start()
    {
        ParseFiles();
        ins = this;
    }

    void ParseFiles()
    {
        parsedConfigs.Clear();
        foreach(TextAsset file in jsonFiles)
        {
            parsedConfigs.Add( file.name, JToken.Parse(file.ToString()));
            UIBoundData.AddListHandler(file.name + ".brand", (data) => GetData(file.name + ".brand"));
        }
    }

    IList GetData(string type)
    {
        string baseType = type.Split('.')[0];
        JToken selectedConfig = parsedConfigs[baseType];
        List<string> objs = new List<string>();
        if (selectedConfig != null)
        {
            for(int i = 0; i < selectedConfig["brands"].Children().Count(); i++)
            {
                objs.Add(selectedConfig["brands"][i]["name"].Value<string>());
            }
            //redo this to add all possible properties.
            
        }
        return objs;
    }

    public static GameObject GetWheelData(int index)
    {
        JToken selectedConfig = parsedConfigs["RobotWheel"];
        Dictionary<string,object> data = new Dictionary<string,object>();

        if (selectedConfig != null)
        {
            string prefabName = (selectedConfig["brands"][index]["uuid"].Value<string>());
            GameObject prefab = Resources.Load("RobotParts/RobotWheel/" + prefabName) as GameObject;
            //redo this to add all possible properties.
            return prefab;
        }
        return null;
    }

    public GameObject GetPrefab(string type, int index, bool collider = false)
    {
        JToken selectedConfig = parsedConfigs[type];
        Dictionary<string, object> data = new Dictionary<string, object>();

        if (selectedConfig != null)
        {
            string collidername = "";
            if (collider) { collidername = ".collider"; }
            string prefabName = (selectedConfig["brands"][index]["uuid"].Value<string>());
            GameObject prefab = Resources.Load($"RobotParts/{type}/{prefabName+collidername}") as GameObject;
            //redo this to add all possible properties.
            return prefab;
        }
        return null;
    }

    public static GameObject GetPrefabObj(string type, int index, bool collider = false)
    {
        return ins.GetPrefab(type, index, collider);
    }

    public Dictionary<string, object> GetData(string type, int index, string fields)
    {
        JToken selectedConfig = parsedConfigs[type];
        Dictionary<string, object> data = new Dictionary<string, object>();
        /*
        if (selectedConfig != null)
        {
            string prefabName = (selectedConfig["brands"][index]["uuid"].Value<string>());
            GameObject prefab = Resources.Load($"RobotParts/{type}/{prefabName}") as GameObject;
            //redo this to add all possible properties.
            return prefab;
        }*/
        return null;
    }
}
