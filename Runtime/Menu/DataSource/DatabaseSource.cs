using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[System.Serializable]
public class DatabaseSource : ScriptableObject
{
    [HideInInspector]
    public string sourceName;
    [HideInInspector]
    public string addressOfData;
    protected bool dataReady = false;
    [HideInInspector]
    public DataType type;

    public delegate void DataReadyHandler();
    public event DataReadyHandler onDataReady;

    public Dictionary<string, DataSource> tables;

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

    public bool isRemote = false;

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
    }

    private void OnEnable()
    {
        //if(tables.Count ==0)
        {
            LoadData();
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
        tableName = tableName.ToLower();
        if (tables.ContainsKey(tableName))
        {
            tables.Remove(tableName);
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

    public virtual void LoadData()
    {
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
        if (dataChanged != null && selectedKey != "NA")
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

    //TODO:get all fields.
    public virtual Dictionary<string, fieldType> getFields()
    {
        return new Dictionary<string, fieldType>();
    }

    //request next data set
    public virtual void RequestNextSet()
    {

    }

    public virtual void RequestPrevSet()
    {

    }
}
