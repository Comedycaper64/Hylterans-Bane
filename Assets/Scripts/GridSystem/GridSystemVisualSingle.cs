using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;

    private bool oscillateTransparency;

    private void Update()
    {
        if (oscillateTransparency)
        {
            float alphaValue = Mathf.Abs(Mathf.Sin(3f * Time.time));
            meshRenderer.material.color = new Color(1, 1, 1, alphaValue);
        }
    }

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

    public void ToggleTransparencyOscillation(bool toggle)
    {
        oscillateTransparency = toggle;
    }
}
