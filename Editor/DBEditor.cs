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
        if (dbsource.lockType)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("inputType"));
            EditorGUILayout.Space();
        }
        
        EditorGUILayout.LabelField("----------Source Options----------");
        switch (dbsource.inputType)
        {
            case DataInputType.Asset:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("local_file"));
                break;

            case DataInputType.Moddable:
                if (GUILayout.Button("Select streaming assets file"))
                {
                    dbsource.filePath = EditorUtility.OpenFilePanel("Select Data file", Application.streamingAssetsPath, "");
                    return;
                }
                string filename = "";
                int index = dbsource.filePath.LastIndexOf("/");
                if (index >= 0) { filename = dbsource.filePath.Substring(index+1); }

                EditorGUILayout.LabelField("File", filename);
                EditorGUILayout.LabelField("Path", dbsource.filePath);
                break;

            case DataInputType.Remote:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rootURL"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("URL"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("webRequestType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultRemoteArguments"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("remoteSetIncremenAmt"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("remotePageFieldName"));
                break;
        }
       
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("----------Data Info----------");
        string tableList = "";
        int numTables = tables?.Count ?? 0;
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
