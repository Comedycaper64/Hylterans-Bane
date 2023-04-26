using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuseButtonUI : MonoBehaviour
{
    public static event EventHandler<OnSummonChosenArgs> OnAnySummonChosen;

    public class OnSummonChosenArgs : EventArgs
    {
        public GameObject unitSummon;
        public int boneCost;
    }

    [SerializeField] private TextMeshProUGUI summonText;
    [SerializeField] private TextMeshProUGUI boneCostText;
    [SerializeField] private Button button;

    private GameObject unitSummon;

    public void SetSummon(GameObject unitSummon, int boneCost)
    {
        this.unitSummon = unitSummon;
        summonText.text = "SUMMON " + unitSummon.GetComponent<Unit>().GetUnitName().ToUpper();
        boneCostText.text = "BONE COST: " + boneCost; 
        button.onClick.AddListener(() => {
            OnAnySummonChosen?.Invoke(this, new OnSummonChosenArgs{unitSummon = unitSummon, boneCost = boneCost});
        });

    }
}
