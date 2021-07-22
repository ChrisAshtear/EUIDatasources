using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIForm : MonoBehaviour
{
    public JSONDatabaseSource source;
    protected List<UIDataTag> uiObjects;

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
                string txt = obj.GetComponent<TMPro.TextMeshProUGUI>().text;
                formFields.Add( tag.fieldName, txt );
                break;

            }
        }
        foreach (KeyValuePair<string, string> field in formFields) 
        {
            source.SetArgument( field.Key, field.Value, false );
        }
        source.DoLoadData( false );
    }
}
