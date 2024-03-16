using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemListConfigHandler : MonoBehaviour
{
    public UIBoundData ConfigWindow;
    public GameObject ListWindow;
    public string typeName;
    public bool toggleWindowVisibility = true;
    void Start()
    {
        UIBoundData.AddHandler(typeName, startEdit);
    }

    public object startEdit(object listData, GameObject callingObj)
    {
        if(toggleWindowVisibility)
        {
            ListWindow.SetActive(false);
            ConfigWindow.gameObject.SetActive(true);
        }
       
        Debug.Log("set window active");
        ConfigWindow.SetData(listData);

        return null;
    }
}
