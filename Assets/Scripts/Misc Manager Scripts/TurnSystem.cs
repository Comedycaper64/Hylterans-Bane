using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{ 
    //Instanced because there should only be one
    public static TurnSystem Instance {get; private set;}

    [SerializeField] private AudioClip turnButtonPressed;

    public event EventHandler OnTurnChanged;
    //Keeps track of what turn it is
    private int turnNumber = 1;
    private bool isPlayerTurn = true;

    //Singleton-ed
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TurnSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    //Advances turn, fires off OnTurnChanged event
    public void NextTurn()
    {
        turnNumber++;
        isPlayerTurn = !isPlayerTurn;
        AudioSource.PlayClipAtPoint(turnButtonPressed, Camera.main.transform.position, SoundManager.Instance.GetSoundEffectVolume());
        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    } 

    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
