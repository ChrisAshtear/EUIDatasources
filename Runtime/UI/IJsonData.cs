using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IJsonData
{
    private static Dictionary<string, JToken> parsedConfigs = new Dictionary<string, JToken>();
    public GameObject GetPrefab(string type, int index, bool collider = false);

    public Dictionary<string, object> GetData(string type, int index, string fields);


}