using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    //Just the necessary functions for each indivisual tile visual
    public void Show()
    {
        if (meshRenderer)
            meshRenderer.enabled = true;
    }

    public void Show(Material material)
    {
        if (meshRenderer)
        {
            meshRenderer.enabled = true;
            meshRenderer.material = material;
        }
    }


    public void Hide()
    {
        if (meshRenderer)
            meshRenderer.enabled = false;
    }
}
