using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public enum DataInputType { Asset, Moddable, Remote }

[System.Serializable]
public class DatabaseSource : ScriptableObject
{

    protected bool dataReady = false;
    [HideInInspector]
    public DataType type;

    public bool lockType = false;//If false type is not forced. if true, the DBsource object cannot be changed to a new inputtype.

    public delegate void DataReadyHandler();
    public event DataReadyHandler onDataReady;

    public Dictionary<string, DataSource> tables;
    public DataSource RootTable { get { return getTable("root"); } }

    public Dictionary<string, string> displayCodes;

    public string primaryKey = "UUID";

    protected string selectedKey = "NA";

    public delegate void SelectionChangedHandler();
    public event SelectionChangedHandler selectionChanged;

    public delegate void DataChangedHandler();
    public event DataChangedHandler dataChanged;

    private List<string> tableList;
    [HideInInspector]
    public string loadStatus = "unloaded";

    public DataInputType inputType;

    //Props
    public DatabaseSource()
    {
        tables = new Dictionary<string, DataSource>();
    }

    public void UI_SelectEntry(selectListItem e)
    {
        //selectItem(e.index,e.fill.data);
        Debug.Log(e.ToString());
    }

    private void Awake()
    {
        DoLoadData(true);
    }

    private void OnEnable()
    {
        //if(tables.Count ==0)
        {
            DoLoadData(true);
        }
    }

    private void OnDisable()
    {
        dataReady = false;
    }

    public void addSelectCallback(string tableName,Action<DataSource> action)
    {
        List<string> tableNames = new List<string>(tableName.Split(','));
        foreach (string table in tableNames)
        {
            DataSource d = getTable(table);
            if(d != null)
            {
                d.selectChanged += action;
            }
        }
    }

    public void SetDataReadyCallback(DataReadyHandler callback)
    {
        if (dataReady) { callback.Invoke(); }
        else { onDataReady += callback; }
    }

    public void addChangedCallback(string tableName, Action<DataSource> action)
    {
        List<string> tableNames = new List<string>(tableName.Split(','));
        foreach (string table in tableNames)
        {
            DataSource d = getTable(table);
            if (d != null)
            {
                d.datChanged += action;
            }
        }
    }

    public void dropTable(string tableName)
    {
        DataSource table = getTable(tableName);
        if (table.name != "NotFound")
        {
            tables.Remove(tableName);
            changedData();
        } 
    }

    public void clearTable(string tableName)
    {
        DataSource table = getTable(tableName);
        if(table.name != "NotFound")
        {
            table.attributes = new DataLibrary();
            table.data.Clear();
            changedData();
        }
    }

    public virtual List<string> getTables()
    {
        return tableList;
    }

    public virtual void addTable(string tableName, DataSource table)
    {
        tableName = tableName.ToLower();
        if (!tables.ContainsKey(tableName))
        {
            tables.Add(tableName, table);
        }
    }

    public virtual DataSource newTable(string tableName)
    {
        tableName = tableName.ToLower();
        if (!tables.ContainsKey(tableName))
        {
            DataSource table = new DataSource();
            table.name = tableName;
            tables.Add(tableName, table);
            //table.parentData = this;
            return table;
        }
        else
        {
            return getTable(tableName);
        }
    }

    public virtual bool containsTable(string id)
    {
        return tables.ContainsKey(id);
    }

    public virtual DataSource getTable(string tableName)
    {
        tableName = tableName.ToLower();
        DataSource table = new DataSource();
        table.name = "NotFound";
        tables?.TryGetValue(tableName, out table);
        return table;
    }

    public bool isDataReady()
    {
        if (tables == null || tables.Count == 0)
        {
            return false;
        }
        return dataReady;
    }

    public void DoLoadData(bool clearTables = false)
    {
        if (clearTables) { tables = new Dictionary<string, DataSource>(); }
        bool loaded = LoadData();
        
    }

    protected virtual bool LoadData()
    {
        return true;
    }

    protected void doOnDataReady()
    {
        loadStatus = "loaded.";
        if (onDataReady != null)
        {
            onDataReady();
        }
        changedData();
        tableList = new List<string>();
        foreach (DataSource source in tables.Values)
        {
            if (!tableList.Contains(source.name))
            {
                tableList.Add(source.name);
            }
        }
    }

    protected void doOnDataChanged()
    {
        if (dataChanged != null) { dataChanged(); }
    }

    public virtual DataSource getSelected()
    {
        if (dataReady && tables != null && tables.ContainsKey(selectedKey))
        {
            DataSource entry = tables[selectedKey];
            return entry;
        }
        else
        {
            return null;
        }
    }

    public void changedData()
    {
        if (dataChanged != null)
        {
            dataChanged();
        }
    }

    public virtual void selectNextItemInTable(string table)
    {
        getTable(table)?.selectNext();
    }

    public virtual void selectPrevItemInTable(string table)
    {
        getTable(table)?.selectPrev();
    }

    public virtual string getSelectedKey()
    {
        return selectedKey;
    }

    public virtual void selectItem(string key,DataSource table)
    {
        selectedKey = key;
        table?.selectItem(key);
        if (selectionChanged != null)
        {
            selectionChanged();
        }
    }

    //request next data set
    public virtual int RequestNextSet(string tableName, int amtOfRecords, int recordOffset)
    {
        return 0;
    }

}
