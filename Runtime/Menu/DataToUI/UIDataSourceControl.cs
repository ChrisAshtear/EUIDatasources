using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIDataSourceControl : UIDataController
{
    public DataSource data;
    private DataLibrary lib;
    // Use this for initialization
    void Start()
    {
        data.selectionChanged += RefreshFromSource;
        RefreshFromSource();
    }

    public void RefreshFromSource()
    {
        Dictionary<string, object> database = data.getSelected();

        lib = new DataLibrary(database);

        RefreshData(lib);
    }

    private void OnDestroy()
    {
        data.selectionChanged -= RefreshFromSource;
    }
}
