﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIDataSourceControl : UIDataController
{
    public SourceProps data;
    private DataLibrary lib;
    private DataSource source;
    // Use this for initialization
    void Start()
    {
        source = data.db.getTable(data.tableName);
        source.selectionChanged += RefreshFromSource;
        RefreshFromSource();
    }

    public void RefreshFromSource()
    {                 
        Dictionary<string, object> database = source.getSelected();

        lib = new DataLibrary(database);

        RefreshData(lib);
    }

    public void SelectNext()
    {
        source.selectNext();
    }

    public void SelectPrev()
    {
        source.selectPrev();
    }

    private void OnDestroy()
    {
        source.selectionChanged -= RefreshFromSource;
    }
}
