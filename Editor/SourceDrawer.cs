using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SourceProps))]
public class SourceDrawer : PropertyDrawer
{

    private float xOffset = 0;
    private float yHeight = 32;
    private float expandedHeight = 50;//extra space for event control +/- buttons
    // Draw the property inside the given rect
    int _choiceIndex;
    int _tableChoiceIndex;

    List<string> allFields = new List<string>();
    List<string> allTables = new List<string>();
    string currentData = "null";
    public int index = 0;
    public string[] options = new string[] { "Cube", "Sphere", "Plane" };

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return yHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        xOffset = position.x;

        SerializedProperty dataSource = property.FindPropertyRelative("db");
        DatabaseSource obj = (DatabaseSource)dataSource.objectReferenceValue;

        SerializedProperty table = property.FindPropertyRelative("tableName");
        // Calculate rects
        var labelRect = getRect(100, position);
        var tableRect = getRect(100, position);

        GUIutil.doPrefixLabel(ref labelRect, "Source");
        labelRect.height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("db"));
        EditorGUI.PropertyField(labelRect, property.FindPropertyRelative("db"), GUIContent.none);
        GUIutil.doPrefixLabel(ref tableRect, "Table");

        if (obj != null)
        {
            string[] tables = obj.getTables().ToArray();
            index = EditorGUI.Popup(tableRect, index, tables.ToArray());
            if (index >= tables.Length)
            {
                index = 0;
            }
            property.FindPropertyRelative("tableName").stringValue = tables[index];
            property.FindPropertyRelative("ID").stringValue = obj.primaryKey;
        }

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