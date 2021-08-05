using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UIForm : MonoBehaviour
{
    public DBLoader source;
    protected List<UIDataTag> uiObjects;
    public UnityEvent OnResponse;

    private void Awake()
    {
        
    }

    public void DoOnResponse()
    {
        if (OnResponse != null) { OnResponse.Invoke(); }
        source.dataChanged -= DoOnResponse;//remove the event as we only need this to be refreshed once.
    }

    public void SubmitForm()
    {
        uiObjects = GetComponentsInChildren<UIDataTag>( true ).ToList();
        Dictionary<string, string> formFields = new Dictionary<string, string>();
        
        foreach (UIDataTag tag in uiObjects) 
        {
            GameObject obj = tag.gameObject;
            switch (tag.dataType) 
            {
                case UIDataType.Value:
                TMPro.TextMeshProUGUI textObj = obj.GetComponent<TMPro.TextMeshProUGUI>();
                string txt = textObj.text;
                formFields.Add( tag.fieldName, txt );
                textObj.text = "";//doesnt work? need to control tmp text input
                break;

            }
        }
        foreach (KeyValuePair<string, string> field in formFields) 
        {
            source.SetArgument( field.Key, field.Value, false );
        }
        source.dataChanged += DoOnResponse;
        source.DoLoadData( false );
    }
}
