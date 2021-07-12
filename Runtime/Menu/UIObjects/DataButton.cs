using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;

#if UNITY_EDITOR
[CustomEditor(typeof(DataButton))]
public class DataButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DataButton targetMenuButton = (DataButton)target;

        //targetMenuButton.acceptsPointerInput = EditorGUILayout.Toggle("Accepts pointer input", targetMenuButton.acceptsPointerInput);
        base.OnInspectorGUI();
        // Show default inspector property editor
        //DrawDefaultInspector();
    }
}
#endif
public class DataButton : UnityEngine.UI.Button
{
    public object Data { get; set; }

    public UnityEvent<object> clickEvent;
    public UnityEvent<int> clickEventInt;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if(interactable)
        {
            base.OnPointerClick(eventData);
            clickEvent?.Invoke(Data);
            int evData = Convert.ToInt32(Data);
            clickEventInt?.Invoke(evData);
        }
    }

    public void ForcePress()
    {
        clickEvent?.Invoke(Data);
        int evData = Convert.ToInt32(Data);
        clickEventInt?.Invoke(evData);
    }

}
