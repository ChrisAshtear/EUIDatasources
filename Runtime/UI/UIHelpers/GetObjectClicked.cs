using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GetObjectClicked
{
    bool selecting = false;
    public GameObject selected;
    public UnityEvent<GameObject> onClick = new UnityEngine.Events.UnityEvent<GameObject>();
    public Material selectedMaterial;
    Vector3 lastClick;
    I_SelectionEffect effect;

    private int layerToSelect = 1;
    public GetObjectClicked(UnityAction<GameObject> onClk,I_SelectionEffect effect, int layerToSelect)//Layer to select or object with an interface to select?
    {
        onClick.AddListener(onClk);
        this.effect = effect;  
        UpdateCaller.AddUpdateCallback(Update);
        this.layerToSelect = layerToSelect;
        if(effect == null) { Debug.LogError("No Selection Effect found or Selected."); }
        
    }

    ~GetObjectClicked()
    {
        UpdateCaller.RemoveUpdateCallback(Update);
    }

    public void StartSelect()
    {
        selecting = true;
    }

    public void ChangeMaterials(bool remove= false)
    {
        foreach (MeshRenderer rend in selected.GetComponentsInChildren<MeshRenderer>())
        {
            List<Material> mats = new List<Material>();
            rend.GetMaterials(mats);
            if (remove) { mats.RemoveAt(mats.Count-1); }
            else { mats.Add(selectedMaterial); }
            rend.materials = mats.ToArray();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (selecting)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //New Collider
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << layerToSelect) && selected != hit.collider.gameObject)
            {
                //Disable Last Collider
                if (selected != null)
                {
                    effect.SetHighlighted(false, selected);
                }
                        
                selected = hit.collider.gameObject;
                //Enable New Collider
                effect.SetHighlighted(true, selected);
            }
            //No Collider
            else if (selected != null && hit.collider == null)
            {
                effect.SetHighlighted(false, selected);
                selected = null;
            }
            //Checks when to register clicks
            if (Input.GetMouseButtonDown(0))
                lastClick = Input.mousePosition;
            if (Input.GetMouseButtonUp(0) && Vector3.Distance(lastClick, Input.mousePosition) < 50 && selected != null)
            {
                effect.SetHighlighted(false, selected);
                effect.OnClicked(selected);
                selecting = false;
                onClick.Invoke(selected);
                //clear onclick?
            }
        }
    }
}
