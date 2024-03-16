using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Material selectMaterial;

    private void OnEnable()
    {
        
    }

    public virtual void BindHover(GameObject triggeringObj, Material effectMaterial)
    {
        selectMaterial = effectMaterial;
        EventTrigger trig = triggeringObj.GetComponentInChildren<EventTrigger>();
        if(trig == null) { Debug.Log("Triggering object requires an EventTrigger"); return; }

        EventTrigger.Entry eventHover = new EventTrigger.Entry();
        eventHover.eventID = EventTriggerType.PointerEnter;
        eventHover.callback.AddListener((data) => Hover(gameObject, true));
        trig.triggers.Add(eventHover);
        EventTrigger.Entry eventUnhover = new EventTrigger.Entry();
        eventUnhover.eventID = EventTriggerType.PointerExit;
        eventUnhover.callback.AddListener((data) => Hover(gameObject, false));
        trig.triggers.Add(eventUnhover);
    }

    public virtual void Hover(GameObject glowObj, bool glow)
    {
        SetGlow(glowObj,glow, selectMaterial);
    }

    public static void SetGlow(GameObject glowObj, bool glow, Material selectMaterial)
    {
            foreach (MeshRenderer rend in glowObj.GetComponentsInChildren<MeshRenderer>())
            {
                List<Material> mats = new List<Material>();
                rend.GetMaterials(mats);
                if (!glow) { mats.RemoveAt(mats.Count - 1); }
                else { mats.Add(selectMaterial); }
                rend.materials = mats.ToArray();
            }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverEffect.SetGlow(gameObject, true,selectMaterial);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverEffect.SetGlow(gameObject, false, selectMaterial);
    }
}
