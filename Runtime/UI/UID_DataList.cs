using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

public class UID_DataList : MonoBehaviour
{
    public GameObject ListItemObject;
    private List<GameObject> ListItems = new List<GameObject>();
    private object dat;
    public Transform listItemParent;
    public Transform itemSelectedDetailsWindow;

    public void SetData(object data)
    {
        if(!listItemParent)
        {
            listItemParent = transform;
        }
        dat = data;
        if(data is IList && data.GetType().IsGenericType)
        {
            IList list = (IList)data;

            UIBoundData sameObjData = GetComponent<UIBoundData>();
            if(sameObjData)
            {
                sameObjData.listObj = this;
                sameObjData.SetData(data);
            }
            //ListItemObject.SetActive(false);
            int count = listItemParent.childCount;
            int lastItemIdx = 0;
            for (int i = 0; i < list.Count; i++)
            {
                GameObject itemObj = null;
                if(i < count)
                {
                    Transform t = listItemParent.GetChild(i);
                    if (t)
                    {
                        itemObj = t.gameObject;
                    }
                }
                else
                {
                    itemObj = GameObject.Instantiate(ListItemObject, listItemParent);
                }

                object item = list[i];
                Debug.Log("list count" + list.Count+"-"+ data.ToString());
                
                itemObj.SetActive(true);
                UIBoundData boundData = itemObj.GetComponent<UIBoundData>();
                boundData.listObj = this;
                boundData.SetData(item,null,i,data,itemSelectedDetailsWindow);
                
                ListItems.Add(boundData.gameObject);
                Debug.Log(item.ToString());
                lastItemIdx = i;
            }
            if(list.Count >0)
            {
                lastItemIdx++;
            }
            for(int i = lastItemIdx; i < listItemParent.childCount; i++)
            {
                Transform t = listItemParent.GetChild(i);
                if (t)
                {
                    t.gameObject.SetActive(false);
                }
            }
            
        }
    }

    public void UpdateData()
    {
        SetData(dat);
    }

    private void Update()
    {
        if (dat != null)
        {
           // SetData(dat);
        }
    }

}
