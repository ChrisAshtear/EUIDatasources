using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SelectionEffects;

public class CursorEffect : MonoBehaviour, I_SelectionEffect
{
    public GameObject CursorPrefab;
    public Material highlightedMat;
    public Material selectedMat;
    public float yOffset = 2;

    GameObject cursorObj;
    GameObject selectedCursorObj;
    public void OnClicked(GameObject obj)
    {
        cursorObj.SetActive(false);
        SetCursor(selectedCursorObj, obj, true);
        Effects.ChangeMaterials(selectedCursorObj, selectedMat,false,true);
    }

    public void SetHighlighted(bool isHighlighted, GameObject obj)
    {
        SetCursor(cursorObj, obj, true);
        if(cursorObj.transform.parent == selectedCursorObj.transform.parent) { cursorObj.gameObject.SetActive(false); }
        Effects.ChangeMaterials(cursorObj, highlightedMat, !isHighlighted,true);
    }

    public void SetCursor(GameObject cursor, GameObject parent, bool active)
    {
        cursor.SetActive(active);
        Vector3 offset = new Vector3(0, yOffset, 0);
        cursor.transform.position = parent.transform.position + offset;
        cursor.transform.parent = parent.transform;//might be issues with old parents?
    }

    // Start is called before the first frame update
    void Start()
    {
        cursorObj = GameObject.Instantiate(CursorPrefab);
        cursorObj.SetActive(false);
        selectedCursorObj = GameObject.Instantiate(CursorPrefab);
        selectedCursorObj.SetActive(false);
    }

    void OnDestroy()
    {
        Destroy(cursorObj);
    }

    //do we want to parent cursor or update its position? Potential issues could be that if a selected object does something with its children itll affect the cursor.
}
