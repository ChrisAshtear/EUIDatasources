using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChildren : MonoBehaviour
{
    //Attach this to Transforms that will be used a lot to access its children

    public Dictionary<string, Transform> children = new Dictionary<string, Transform>();

    bool activateOnce = true;

    // Start is called before the first frame update
    public void Start()
    {
        if (!activateOnce)
            return;
        activateOnce = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (!children.ContainsKey(transform.GetChild(i).name))
                children.Add(transform.GetChild(i).name, transform.GetChild(i));
        }
    }
}
