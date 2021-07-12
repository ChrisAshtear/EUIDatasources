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
            s.LoadData();
        }
        
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void setupSettings()
    {
        ProjectSettings.data = Resources.LoadAll<ProjectData>("")[0];
        Debug.Log("pdata" + ProjectSettings.data);
    }
}