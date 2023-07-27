using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RallyingCrySystem : MonoBehaviour
{
    private UnitActionSystem unitActionSystem;

    private void Awake()
    {
        unitActionSystem = GetComponent<UnitActionSystem>();
    }

    private void Start()
    {
        InputManager.Instance.OnRallyingCryEvent += InputManage_OnRallyingCry;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnRallyingCryEvent -= InputManage_OnRallyingCry;
    }

    private void InputManage_OnRallyingCry()
    {
        //Pause game (or not?)
        //Open rallying cry UI with a callback EventHandler event containing a rallying cry
    }

    //callback event runs a script that sees if the callback is empty (if so do nothing), then sees if the action system is busy.
    //If it's the enemy turn or the system is busy, a new initiative is created for the unit to do a rallying cry
    //Otherwise it's simply done
}
