using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity;
using UnityEngine.Events;
using UnityEditor;


// Custom serializable class

public class BaseProps
{
    public string label = "";
    public Color32 color = Color.grey;
}

public class BaseDataProps : BaseProps
{
    public DataSource data;
    
}

public class BaseCustomProps : BaseProps
{
    public UnityEvent ev;
}

public enum PopulateInputType { Custom, DataSource}

public class populateBase : MonoBehaviour
{
    //populate select input:
    //custom - 
    //*or*
    //datasource. editor options change after selecting the option.
    //if custom, displays the props list
    //if datasource, displays filter + source selection.
    public PopulateInputType inputType;
    public GameObject layoutGroup; // where to place generated buttons
    public GameObject prefab;//object prefab
    public bool populateOnStart = true;

    [HideInInspector]
    public GameObject selectedItem;

    void Reset()
    {
        layoutGroup = gameObject;
        //prefab = ProjectSettings.data.defaultButton;
    }
    // Start is called before the first frame update
    

    public virtual void PrePopulate()
    {

    }

    public void DoPopulate()
    {
        PrePopulate();
        Populate();
    }

    public virtual void Populate()
    {
    }

    public void Clear()
    {
        if (Application.isEditor)
        {
            foreach (Transform child in layoutGroup.transform)
            {
                GameObject obj = child.gameObject;
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (obj != null)
                    {
                        UnityEditor.Undo.DestroyObjectImmediate(obj);
                    }
                };
            }
        }
        else
        {
            GUIutil.clearChildren(layoutGroup.transform);
        }
        //props.Clear();?
    }

    private void OnValidate()
    {
        DoPopulate();
    }

    public void SetSelected(GameObject obj)
    {
        selectedItem = obj;
    }

    public GameObject GetSelected()
    {
        return selectedItem;
    }

}
