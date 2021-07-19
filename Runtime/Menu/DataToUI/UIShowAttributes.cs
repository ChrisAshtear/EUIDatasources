using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIShowAttributes : UIDataController
{
    public SourceProps props;
    private DataSource data;
    // Use this for initialization
    void Start()
    {
        
        Debug.Log(props);
        if (props.db != null)
        {
            props.db.onDataReady += RefreshFromSource;
            props.db.dataChanged += RefreshFromSource;
        }
        data = props.db.getTable(props.tableName);
        RefreshFromSource();
    }

    public void RefreshFromSource()
    {
        data = props.db.getTable(props.tableName);
        if(data!=null)
        {
            RefreshData(data.GetAllAttributes());
        }
    }

    private void OnDestroy()
    {
        if(data!=null)
        {
            data.selectionChanged -= RefreshFromSource;
        }
    }
}
