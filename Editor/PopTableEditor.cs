using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(populateTable), true)]
public class TableEditor : Editor
{

    populateTable table;

    public void OnEnable()
    {
        table = (populateTable)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("props"), new GUIContent("Data Source Options"));
        EditorGUILayout.Space();
        EditorStyles.label.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Table Options");
        EditorStyles.label.fontStyle = FontStyle.Normal;
        GUIutil.DrawUILine(Color.white);
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.ObjectField(serializedObject.FindProperty("prefab"), new GUIContent("Table Row Prefab"));
        EditorGUILayout.PrefixLabel("Striping | Show Header");
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("striping"), new GUIContent(""));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showHeader"), new GUIContent(""));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("Stripe Color 1 | Stripe Color 2 | Header Color");
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stripeColorOdd"), new GUIContent(""));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stripeColorEven"), new GUIContent(""));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("headerColor"), new GUIContent(""));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fieldList"));
        EditorGUILayout.EndVertical();
        GUIutil.DrawUILine(Color.white);
        //not supported yet EditorGUILayout.PropertyField(serializedObject.FindProperty("sortable"));
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(table);
    }

}
