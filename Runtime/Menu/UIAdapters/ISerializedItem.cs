using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializedItem
{
    public void ReadFromSource(DataItem data);
    public void WriteToSource(DataItem data,DataSource dataSource);
}
