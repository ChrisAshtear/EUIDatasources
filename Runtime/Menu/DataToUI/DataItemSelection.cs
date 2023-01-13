using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataItemSelection
{
    public DatabaseSource db;
    public string tableName;
    public string ID;
    public string itemUUID;
    public DataSource source { get { return db.getTable(tableName); } }
}