using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_SelectionEffect
{
    public void OnClicked(GameObject obj);
    public void SetHighlighted(bool isHighlighted, GameObject obj);
}