using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity;
using UnityEngine.Events;
using UnityEditor;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
public enum listFunction { showDetails, Form, CustomPlusDetails, CustomDataLib };
public enum filterType { text, number, boolean }
public enum filterMatchType { matches, notmatching, greaterOrEqual, LessThanOrEqual }

[Serializable]
public class SourceFilter
{
    public string filterVar;//filter variable to be searched
    public string filterValue;//filter text - comma separated values.
    public filterType type;
    public filterMatchType match;

    public bool MatchesFilter(string value)
    {
        switch (type)
        {
            case filterType.number:

                int.TryParse(value, out int testingNum);
                int.TryParse(filterValue, out int filterNum);
                switch (match)
                {
                    case filterMatchType.greaterOrEqual:
                        if (testingNum >= filterNum) { return true; }
                        else { return false; }
                        break;
                    case filterMatchType.LessThanOrEqual:
                        if (testingNum <= filterNum) { return true; }
                        else { return false; }
                        break;
                }
                break;

            case filterType.text:
                bool doesMatch = filterValue.Contains(value);
                switch (match)
                {
                    case filterMatchType.matches:
                        return doesMatch;
                        break;
                    case filterMatchType.notmatching:
                        return !doesMatch;
                        break;
                }
                break;
            case filterType.boolean:
                bool matches = value.Contains("True");
                return matches;
        }
        return false;
    }
}
/*
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SourceFilter))]
public class FilterDrawer : PropertyDrawer
{
    private float xOffset = 0;
    private float yHeight = 20;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        xOffset = position.x - 100;

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var varRect = getRect(100, position);
        var valueRect = getRect(100, position);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(varRect, property.FindPropertyRelative("filterVar"),GUIContent.none);
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("filterValue"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public Rect getRect(int size, Rect position)
    {
        Rect newR = new Rect(xOffset, position.y, size, yHeight);
        xOffset += size;
        return newR;
    }
}
#endif
*/
[Serializable]
public class DataProps : BaseProps
{
    public UnityEvent ev;
    public UnityEvent<IDataLibrary> evData;

    public string tableName;
    [HideInInspector]
    public DataSource data { get { return dataS.db.getTable(dataS.tableName); } }
    public SourceProps dataS;
    public GameObject detailsContainer;
}

public class populateData : populateBase, I_ItemMenu
{
    public DataProps props;
    public bool alwaysVisible = false;//for individual item - should infocard pop up, or should data be placed onto an always visible ui element.

    public bool showEmptyItem = false; //show 'empty' item.
    public bool selectDefaultItem = true;

    [TextArea(3, 10)]
    public string displayCode;
    public List<SourceFilter> filters;
    [Tooltip("Filter always present")]
    public List<SourceFilter> permanentFilter;
    private IEnumerable<SourceFilter> allFiltersA;

    protected int defaultSelection = -1;
    private Dictionary<string, object> preservedData = new Dictionary<string, object>();
    void Start()
    {
        if (populateOnStart)
        {
            Debug.Log(props);
            if(props.dataS != null)
            {
               props.dataS.db.onDataReady += DoPopulate;
               props.dataS.db.dataChanged += DoPopulate;
            }

            DoPopulate();
        }
        //TODO: add subscriptions for ondatachanged and ondataready

    }
    public void OnClick(UIButtonListItem listItem)
    {
        props.evData.Invoke(listItem.GetData());
    }

    public void SetFilter(string newFilter)
    {
        if (filters.Count > 0)
        {
            filters[0].filterValue = newFilter;
        }
        Populate();
    }
    //TODO:replace definition id with a variable
    public void RefreshFilterByData(IDataLibrary data)
    {
        preservedData = data.GetPreserved();
        if (filters.Count > 0)
        {
            if (filters.Count > 0)
            {
                filters[0].filterValue = data.GetValue(filters[0].filterVar).ToString();
            }
            IData id = data.GetValue("DefinitionID");
            if (id.Data != null)
            {
                defaultSelection = (int)(uint)id.Data;
            }
            Populate();
        }
    }

    public override void PrePopulate()
    {
        allFiltersA = filters.Concat(permanentFilter);
    }

    public bool IsFiltered(string key,string value)
    {
        bool filtered = false;
        foreach (SourceFilter filter in allFiltersA)
        {
            if (key == "0" && showEmptyItem) { continue; }
            if (!filter.MatchesFilter(value))
            {
                filtered = true;
            }
        }
        if (filtered) { return true; }
        else { return false; }
    }
    
    public void setData(DatabaseSource d)
    {
        props.dataS.db = d;
        Clear();
        Invoke("Populate", 0.2f);
    }

    public void setTable(string t)
    {
        props.tableName = t;
        Clear();
        Invoke("Populate", 0.2f);
    }

    
}
