using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    //Contains logic to show or hide the BusyUI based on the OnBusyChanged event
    private void Start() 
    {
        Hide();
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;    
    }

    private void Show()
    {
        if (this)
            gameObject.SetActive(true);
    }

    private void Hide()
    {
        if (this)
            gameObject.SetActive(false);
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        if (isBusy)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
}
