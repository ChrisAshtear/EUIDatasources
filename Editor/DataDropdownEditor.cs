using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR

[CustomEditor(typeof(DataDropdown), true)]
public class DataDropDownEditor : Editor
{

    DataDropdown dropdown;
    DataSource table;
    string field="";
    bool showAllProps = false;
    UnityEvent<IDataLibrary> eventd;
    public void OnEnable()
    {
        dropdown = (DataDropdown)target;
    }

    void OnTableSelected(object table)
    {
        this.table = (DataSource)table;
        dropdown.data = this.table;
        dropdown.chosenTable = this.table.name.ToLower();
    }

    void OnFieldSelected(object field)
    {
        this.field = (string)field;
        dropdown.chosenField = this.field;
    }

    public void RestoreTable()
    {
        if(dropdown.db != null && dropdown.chosenTable != "")
        {
            this.table = dropdown.data = dropdown.db.getTable(dropdown.chosenTable);
            this.field = dropdown.chosenField;
        }
    }

    public override void OnInspectorGUI()
    {
        RestoreTable();
        //detect when db or table changes and wipe list of fields/tables
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("DatabaseSource");
        dropdown.db = (DatabaseSource)EditorGUILayout.ObjectField(dropdown.db, typeof(DatabaseSource), false);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Table");
        //EditorGUILayout.DropdownButton("Button");
        string tableButtonLabel = "Select Table";
        if(dropdown.db != null)
        {
            if (table != null) { tableButtonLabel = table.name; }
            if (GUILayout.Button(tableButtonLabel))
            {
                // create the menu and add items to it
                GenericMenu menu = new GenericMenu();
                List<DataSource> datas = dropdown.db.tables.Values.ToList();
                foreach (DataSource d in datas)
                {
                    menu.AddItem(new GUIContent(d.name), d.name == table?.name, OnTableSelected, d);
                }

                // display the menu
                menu.ShowAsContext();
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Field");
        if (table!=null)
        {
            string label = "Select Field";
            if(field != "") { label = field; }
            if (GUILayout.Button(label))
            {
                // create the menu and add items to it
                GenericMenu menu = new GenericMenu();
                List<string> fields = table.getFieldSimple();
                foreach (string d in fields)
                {
                    menu.AddItem(new GUIContent(d), d== field, OnFieldSelected, d);
                }

                // display the menu
                menu.ShowAsContext();
            }
        }

        EditorGUILayout.EndHorizontal();

        showAllProps = EditorGUILayout.Toggle("Show Advanced", showAllProps);
        if(showAllProps)
        {
            EditorGUILayout.LabelField("Dropdown Props:");
            DrawDefaultInspector();
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onSelectDataEvent"), new GUIContent("On Selection"));
            EditorGUILayout.Space();
        }
        

        EditorUtility.SetDirty(dropdown);
    }

}
#endif