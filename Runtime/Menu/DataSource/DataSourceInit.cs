using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[DefaultExecutionOrder(200)]
public static class DataSourceInit
{
    
    private static DatabaseSource[] sources;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void setupSources()
    {
        sources = Resources.LoadAll<DatabaseSource>("");
        foreach(DatabaseSource s in sources)
        {
            s.DoLoadData(true);
        }
        
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void setupSettings()
    {
        ProjectSettings.data = ProjectSettings.GetData();
        Debug.Log("pdata" + ProjectSettings.data);
    }
}