using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI turnNumberText;

    // [SerializeField]
    // private GameObject enemyTurnVisualGameObject;

    [SerializeField]
    private Transform initiativeContainer;

    [SerializeField]
    private Image currentInitiativeImage;

    [SerializeField]
    private GameObject initiativePrefab;

    private Queue<GameObject> initiativeUIQueue = new Queue<GameObject>();

    private void Start()
    {
        //TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        TurnSystem.Instance.OnNewInitiative += TurnSystem_OnNewInitiative;
        TurnSystem.Instance.OnNextUnitInitiative += TurnSystem_OnNextUnitInitiative;
        UpdateTurnText();
        //UpdateEnemyTurnVisual();
    }

    private void OnDisable()
    {
        //TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        TurnSystem.Instance.OnNewInitiative -= TurnSystem_OnNewInitiative;
        TurnSystem.Instance.OnNextUnitInitiative -= TurnSystem_OnNextUnitInitiative;
    }

    // private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    // {
    //     UpdateEnemyTurnVisual();
    // }

    private void UpdateTurnText()
    {
        if (turnNumberText)
            turnNumberText.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
    }

    // private void UpdateEnemyTurnVisual()
    // {
    //     if (enemyTurnVisualGameObject)
    //         enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    // }

    private void ClearInitiativeUI()
    {
        foreach (Transform initiativeUI in initiativeContainer)
        {
            Destroy(initiativeUI.gameObject);
        }
        initiativeUIQueue.Clear();
    }

    private void TurnSystem_OnNewInitiative(object sender, Queue<Initiative> initiatives)
    {
        ClearInitiativeUI();
        foreach (Initiative initiative in initiatives)
        {
            GameObject newInitiativeUI = Instantiate(initiativePrefab, initiativeContainer);
            if (initiative.unit)
            {
                newInitiativeUI.GetComponent<Image>().sprite = initiative.unit.GetInitiativeUI();
            }
            else
            {
                newInitiativeUI.GetComponent<Image>().sprite = initiative.rallyingCry
                    .GetUnit()
                    .GetInitiativeUI();
            }
            initiativeUIQueue.Enqueue(newInitiativeUI);
        }
        UpdateTurnText();
    }

    private void TurnSystem_OnNextUnitInitiative()
    {
        GameObject lastTurnUnit = initiativeUIQueue.Dequeue();
        currentInitiativeImage.sprite = lastTurnUnit.GetComponent<Image>().sprite;
        Destroy(lastTurnUnit);
    }
}
