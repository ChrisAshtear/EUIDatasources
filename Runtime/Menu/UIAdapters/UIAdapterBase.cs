using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class UIAdapterBase : MonoBehaviour, IUIAdapter
{
    protected DataLibrary data;
    //protected GameMode mode;
    protected GameObject gamemodeObj;
    public UnityEvent<IDataLibrary> onDataLoad;

    float lastTime;
    public Action<Dictionary<string, DataObject>> OnValueChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void AddListener(string valueName, Action<IData> callback)
    {
        data.AddListener(valueName, callback);
    }

    public DataObject GetValue(string valueName)
    {
        return (DataObject)data.GetValue(valueName);
    }

    public DataLibrary GetData()
    {
        return data;
    }

    public void RemoveListener(string valueName, Action<IData> callback)
    {
        data.RemoveListener(valueName, callback);
    }

    public void SetValue(string valueName, object value)
    {
        data.SetValue(valueName, value);
    }

    // Use this for initialization
    void Start()
    {
        data = new DataLibrary();
        //this.mode = (GameMode)ServiceLocator.GetService<IGameModeService>();
        Ready();
    }

    private void OnDestroy()
    {
    }

    protected virtual void Ready()
    {

    }

    public void PopulateData()
    {
    }

    /*public virtual void UpdateStats(GameMode mode)
    {
        Debug.Log("WTF");
    }*/

}
