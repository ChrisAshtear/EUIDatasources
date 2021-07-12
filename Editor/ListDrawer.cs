using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ListProps))]
public class ListDrawer : PropertyDrawer
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

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty p = property.FindPropertyRelative("onSelect");

        if (p.intValue == 2 || p.intValue == 3)//custom prop
        {
            string evName = "evData";
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(evName)) + expandedHeight;
        }
        else
        {
            return yHeight;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);


        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        // Calculate rects
        xOffset = position.x;
        position.xMax += 100;
        var fieldsRect = new Rect(position.x, position.y, position.width, yHeight);
        SerializedProperty p = property.FindPropertyRelative("onSelect");
        EditorRowProps fields = new EditorRowProps(fieldsRect);
        fields.AddProp(property, "dataS");
        fields.AddProp(property, "onSelect", "Dropdown Type");
        if (p.intValue == 0 || p.intValue == 2)
        {
            fields.AddProp(property, "displayObj", "Display Object");
        }
        else { fields.AddProp(property, "none"); }
        fields.Draw();
        if (p.intValue == 2 || p.intValue == 3)//custom prop
        {
            EditorGUI.indentLevel = 0;
            var eventRect = new Rect(position.x, position.y + yHeight, position.width, position.height - yHeight);
            position.height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("evData"));

            eventRect.x -= 100;
            var labelRect = new Rect(position.x - 100, position.y + yHeight, position.width, position.height - yHeight);
            EditorGUI.PrefixLabel(labelRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("OnPress : "));

            eventRect.y += 16;
            EditorGUI.PropertyField(eventRect, property.FindPropertyRelative("evData"), GUIContent.none);

        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

}

#endif