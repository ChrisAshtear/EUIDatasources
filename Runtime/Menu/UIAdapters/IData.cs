using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDataLibrary
{
    IData GetValue(string valueName);

    string GetTxtValue(string valueName);
    void SetValue(string valueName, object value,bool preserve=false);

    void AddListener(string valueName, Action<IData> callback);
    void RemoveListener(string valueName, Action<IData> callback);

    public IEnumerator GetEnumerator();

    string LibraryName{ get; set; }
    Action<Dictionary<string, IData>> OnValueChanged { get; set; }
    Dictionary<string, object> GetPreserved();
    bool IsPopulated { get; }
}

public interface IDataLibraryReadOnly
{
    IData GetValue(string valueName);
}

public interface IData
{
    object Data { get; set; }
    string DisplayValue { get; }
    string Name { get; }

    DataLibSupportedTypes Type { get; }
}