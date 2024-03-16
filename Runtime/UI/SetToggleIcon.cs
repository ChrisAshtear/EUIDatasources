using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetToggleIcon : MonoBehaviour,IDataReceiver
{
    public TextMeshProUGUI toggleIconText;
    private bool enable = true;
    public bool invertBool = true;
    // Start is called before the first frame update
    public void onClick(bool value)
    {
        Debug.Log("icon " + value.ToString());
        if (value)
        {
            toggleIconText.text = "\uF204";
        }
        else
        {
            toggleIconText.text = "\uF205";
        }
    }

    public void SetData(object data)
    {
        bool curVal = (bool)data;
        if(invertBool) { curVal = !curVal; }
        onClick(curVal);
    }

    public void toggleIcon()
    {
        enable = !enable;
        onClick(enable);
    }


}
