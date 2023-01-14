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
    public DataItem item { get { return db.getTable(tableName).getObjFromItemID(itemUUID); } }
}