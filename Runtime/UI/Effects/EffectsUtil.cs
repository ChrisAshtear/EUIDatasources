using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SelectionEffects
{
    public static class Effects
    {
        public static void ChangeMaterials(GameObject selected, Material selectedMaterial, bool remove = false,bool replace = false)
        {
            foreach (MeshRenderer rend in selected.GetComponentsInChildren<MeshRenderer>())
            {
                List<Material> mats = new List<Material>();
                rend.GetMaterials(mats);
                if (replace) { mats.Clear();}
                if (remove && !replace) { mats.RemoveAt(mats.Count - 1); }
                else { mats.Add(selectedMaterial); }
                rend.materials = mats.ToArray();
            }
        }
    }
}

