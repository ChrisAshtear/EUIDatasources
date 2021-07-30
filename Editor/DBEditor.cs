using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DBLoader), true)]
public class DBEditor : Editor
{

    DBLoader dbsource;

    public void OnEnable()
    {
        dbsource = (DBLoader)target;
    }

    public override void OnInspectorGUI()
    {

        serializedObject.Update();
        
        List<string> tables = dbsource.getTables();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("isRemote"));
        
        if (!dbsource.isRemote)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("local_file"));
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rootURL"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("URL"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("webRequestType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultRemoteArguments"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("remoteSetIncremenAmt"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("remotePageFieldName"));
        }
        
        string tableList = "";
        int numTables = tables?.Count ?? 0;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("#Tables", numTables.ToString());
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Status: ", dbsource.loadStatus);
        if (tables != null)
        {
            foreach (string t in tables)
            {
                tableList += t + ",";
            }
        }

        tableList = tableList.TrimEnd(',');
        EditorGUILayout.LabelField("Tables:", tableList);

        if (GUILayout.Button("Load"))
        {
            dbsource.DoLoadData(true);
        }
        //DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(dbsource);
    }

}
