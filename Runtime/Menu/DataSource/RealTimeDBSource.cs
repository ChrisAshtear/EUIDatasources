using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Xml.Linq;
/*
[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DatabaseSource/Real-Time DatabaseSource", order = 1)]
public class RealTimeDBSource : DatabaseSource
{

    //Props
    public RealTimeDBSource()
    {
        //condition = maxCond;
    }

    private void Awake()
    {
        type = DataType.Realtime;
        LoadData();
    }

    private void OnEnable()
    {
        type = DataType.Realtime;
        LoadData();
    }

    public override void LoadData()
    {
        tables = new Dictionary<string, DataSource>();
        displayCodes = new Dictionary<string, string>();
        dataReady = true;

    }

    public void updateData()
    {
        foreach (RTDataSource data in tables.Values.ToList())
        {
            data.updateData();
        }
        //doOnDataChanged();
    }

    public override DataSource newTable(string tableName)
    {
        tableName = tableName.ToLower();
        if (!tables.ContainsKey(tableName))
        {
            RTDataSource table = new RTDataSource();
            table.name = tableName;
            tables.Add(tableName, table);
            //table.parentData = this;
            doOnDataChanged();

            return table;
        }
        else
        {
            return getTable(tableName);
        }
    }

    public void SetData(string tableName,object data)
    {
        tableName = tableName.ToLower();
        RTDataSource d = newTable(tableName) as RTDataSource;
        if(data == null || d == null)
        {
            return;
        }
        d.SetData(data);
        d.primaryKey = "hash";
        dataReady = true;
        doOnDataChanged();
    }
}



    */