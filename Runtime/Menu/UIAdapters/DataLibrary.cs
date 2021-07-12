using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum DataLibSupportedTypes { text, number, sprite };

public class DataObject : IData
{

    object dataObj;
    string name;
    public bool preserveData = false;
    DataLibSupportedTypes type;

    public string Name { get { return name; } set { this.name = value; } }
    public object Data { get { return dataObj; } set { this.dataObj = value; } }
    public string DisplayValue { get { return dataObj?.ToString() ?? ""; } }
    public DataLibSupportedTypes Type { get { return type; } }

    public DataObject(string name, object obj,bool preserve=false)
    {
        this.name = name;
        this.dataObj = obj;
        this.preserveData = preserve;
        this.type = DataLibSupportedTypes.text;
    }

    public override string ToString() { return DisplayValue; }

}

public class DataLibrary : IDataLibrary,IDataLibraryReadOnly, IEnumerator<IData>,IEnumerable
{
    private int position = -1;

    Dictionary<string, DataObject> Values;
    Dictionary<string, Action<DataObject>> callbackList;
    public Action<Dictionary<string, IData>> OnValueChanged { get; set; }
    public string LibraryName { get { return libName; } set { libName = value; } }

    public bool IsPopulated { get { return Values.Count > 0; } }

    public IData Current { get { return (IData)Values.Values.GetEnumerator().Current; } }

    object IEnumerator.Current { get { return Values.Values.GetEnumerator().Current; } }

    private string libName = "Null";

    public DataLibrary()
    {
        Values = new Dictionary<string, DataObject>();
        callbackList = new Dictionary<string, Action<DataObject>>();
    }

    public Dictionary<string,object> GetPreserved()
    {
        Dictionary<string, object> pData = new Dictionary<string, object>();
        foreach (DataObject d in Values.Values)
        {
            if (d.preserveData)
            {
                pData.Add(d.Name, d.Data);
            }

        }
        return pData;
    }

    //TODO: get rid of dataobject in general, kind of useless.
    public DataLibrary(Dictionary<string,object> data)
    {
        Values = new Dictionary<string, DataObject>();
        foreach (KeyValuePair<string, object> de in data)
        {
            Values.Add(de.Key.ToLower(), new DataObject(de.Key, de.Value));
        }
        callbackList = new Dictionary<string, Action<DataObject>>();
    }

    public void AddListener(string valueName, Action<IData> callback)
    {
        if (callbackList.ContainsKey(valueName.ToLower()))
        {
            callbackList[valueName.ToLower()] += callback;
        }
        else
        {
            callbackList.Add(valueName.ToLower(), callback);
        }
    }

    public IData GetValue(string valueName)
    {
        Values.TryGetValue(valueName.ToLower(), out DataObject val);
        return (IData)val ?? (IData)new DataObject("none",null);
    }

    public void RemoveListener(string valueName, Action<IData> callback)
    {
        if (callbackList.ContainsKey(valueName))
        {
            callbackList[valueName.ToLower()] -= callback;
        }
    }

    public void SetValue(string valueName, object value,bool preserve=false)
    {
        bool foundValue = Values.TryGetValue(valueName.ToLower(), out DataObject val);
        if (!foundValue)
        {
            val = new DataObject(valueName, value,preserve);
            Values.Add(valueName.ToLower(), val);
        }
        else
        {
            Values[valueName.ToLower()].Data = value;
            Values[valueName.ToLower()].preserveData = preserve;
        }
        if (callbackList.ContainsKey(valueName.ToLower()))
        {
            callbackList[valueName]?.Invoke(val);
        }
    }

    public string GetTxtValue(string valueName)
    {
        return GetValue(valueName.ToLower()).DisplayValue;
    }

    public bool MoveNext()
    {
        return Values.Values.GetEnumerator().MoveNext();
    }

    public void Reset()
    {
        Values.Values.GetEnumerator().MoveNext();//??
    }

    public void Dispose()
    {
        Values.Values.GetEnumerator().Dispose();
    }

    public IEnumerator GetEnumerator()
    {
        return Values.Values.GetEnumerator();
    }
}