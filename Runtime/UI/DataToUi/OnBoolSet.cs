using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnBoolSet : MonoBehaviour, IDataReceiver
{
    public UnityEvent<bool> onToggle;
    public bool invertBool;
    private bool curVal;
    // Start is called before the first frame update
    public void SetData(object data)
    {
        curVal = (bool)data;
        if (invertBool) { curVal = !curVal; }
        onToggle.Invoke(curVal);
    }

    public void toggle()
    {
        curVal= !curVal;
        onToggle.Invoke(curVal);
    }
}
