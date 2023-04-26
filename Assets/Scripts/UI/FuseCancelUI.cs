using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseCancelUI : MonoBehaviour
{
    public static event EventHandler OnFusionCancel;

    private GameObject cancelButton;

    private void Awake() 
    {
        cancelButton = this.gameObject;
        cancelButton.SetActive(false);    
        // BoneFusionAction.OnAnyBonesSelected += BoneFusionAction_OnAnyBonesSelected;
        // BoneFusionAction.OnAnySummonStart += BoneFusionAction_OnAnySummonStart;
    }

    private void BoneFusionAction_OnAnyBonesSelected(object sender, EventArgs e)
    {
        if (cancelButton)
            cancelButton.SetActive(true);
    }

    private void BoneFusionAction_OnAnySummonStart(object sender, GridPosition e)
    {
        if (cancelButton)
            cancelButton.SetActive(false);
    }   

    public void CancelButtonPressed()
    {
        OnFusionCancel?.Invoke(this, EventArgs.Empty);
        if (cancelButton)
            cancelButton.SetActive(false);  
    }

}
