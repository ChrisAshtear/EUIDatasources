using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ConfigObject : INotifyPropertyChanged, INotifyCollectionChanged
{
    public delegate void ValueChanged(object value);

    public event PropertyChangedEventHandler PropertyChanged;
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    [NonSerialized]
    static Dictionary<string, ValueChanged> callbackList = new Dictionary<string, ValueChanged>();

    [NonSerialized]
    public string typeName;

    //protected IJsonData databank;

    public delegate void OnBotComponentChange(GameObject newObj, GameObject oldObj, object wheelCfg);

    public event OnBotComponentChange onCompTypeChange;

    public ConfigObject()
    {
        typeName = this.GetType().Name;
    }

    public void AddChangeListener(OnBotComponentChange evt)
    {
        if(onCompTypeChange == null)
        {
            onCompTypeChange += evt;
        }
            
    }

    public void SetChanged(GameObject oldObj, GameObject newObj, ConfigObject conf)
    {
        onCompTypeChange.Invoke(oldObj, newObj, conf);
    }
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    protected void SetPropertyField<T>(ref T field, T newValue, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
        string fullname = $"{typeName}.{propertyName}";
        if (!EqualityComparer<T>.Default.Equals(field, newValue))
        {
            field = newValue;
            if (callbackList.ContainsKey(fullname))
            {
                callbackList[fullname].Invoke(newValue);
            }
            OnPropertyChanged(new PropertyChangedEventArgs(fullname));
        }
    }

    public static void AddValueChangeCallback(string field, ValueChanged callback)
    {
        //this needs to pass the full field name.
        if (!callbackList.ContainsKey(field))
        {
            callbackList.Add(field, callback);
        }
        else
        {
            callbackList[field] += callback;
        }
    }

    /*public void SetDatabank(IJsonData databank)
    {
        this.databank = databank;
    }*/
}