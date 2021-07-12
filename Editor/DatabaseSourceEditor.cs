using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(DatabaseSource), true)]
public class DatabaseSourceEditor : Editor
{

    DatabaseSource dbsource;

    public void OnEnable()
    {
        dbsource = (DatabaseSource)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        List<string> tables = dbsource.getTables();
        string tableList = "";
        int numTables = tables?.Count ?? 0;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("#Tables", numTables.ToString());
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Status: ",dbsource.loadStatus);
        if(tables!=null)
        {
            foreach (string t in tables)
            {
                tableList += t + ",";
            }
        }
        
        tableList = tableList.TrimEnd(',');
        EditorGUILayout.LabelField("Tables:" ,tableList);

        if (GUILayout.Button("Load"))
        {
            dbsource.LoadData();
        }
        EditorUtility.SetDirty(dbsource);
    }

}
#endif