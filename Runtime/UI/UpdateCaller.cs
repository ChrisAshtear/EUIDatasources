using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCaller : MonoBehaviour
{
    private static UpdateCaller instance;
    private Action updateCallback;
    static void SetUpdateCallback(Action updateMethod, bool remove = false)
    {
        if (instance == null)
        {
            instance = new GameObject("[Update Caller]").AddComponent<UpdateCaller>();
        }
        if (remove) { instance.updateCallback -= updateMethod; }
        else { instance.updateCallback += updateMethod; }
        
    }

    public static void AddUpdateCallback(Action updateMethod)
    {
        SetUpdateCallback(updateMethod);
    }

    public static void RemoveUpdateCallback(Action updateMethod)
    {
        SetUpdateCallback(updateMethod, true);
    }

    private void Update()
    {
        //detect null callbacks.
        updateCallback();
    }
}