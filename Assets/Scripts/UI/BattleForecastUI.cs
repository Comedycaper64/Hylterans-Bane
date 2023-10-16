using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BattleForecastUI : MonoBehaviour
{
    // [SerializeField]
    // private TextMeshProUGUI chanceToHitText;

    // [SerializeField]
    // private TextMeshProUGUI damageText;

    //private bool forecastEnabled;

    //private Unit activeUnit;

    // private void Awake()
    // {
    //     ToggleForecastText(false);
    // }

    // private void Start()
    // {
    //     UnitActionSystem.Instance.OnSelectedActionChanged +=
    //         UnitActionSystem_OnSelectedActionChanged;
    //     UnitActionSystem.Instance.OnUnitActionStarted += UnitActionSystem_OnUnitActionStarted;
    //     //GridMouseVisual.OnMouseOverEnemyUnit += GridMouseVisual_OnMouseOverEnemyUnit;
    // }

    // private void OnDisable()
    // {
    //     UnitActionSystem.Instance.OnSelectedActionChanged -=
    //         UnitActionSystem_OnSelectedActionChanged;
    //     UnitActionSystem.Instance.OnUnitActionStarted -= UnitActionSystem_OnUnitActionStarted;
    //     //GridMouseVisual.OnMouseOverEnemyUnit -= GridMouseVisual_OnMouseOverEnemyUnit;
    // }

    // public void ToggleForecastText(bool enable)
    // {
    //     //chanceToHitText.enabled = enable;
    //     damageText.enabled = enable;
    //     //forecastEnabled = enable;
    // }

    // public void SetHitText(int hitValue)
    // {
    //     chanceToHitText.text = "Chance to hit: " + hitValue + "%";
    // }

    // public void SetDamageText(int damageValue)
    // {
    //     damageText.text = "Damage: " + damageValue;
    // }

    // private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction baseAction)
    // {
    //     if (baseAction.ActionDealsDamage())
    //     {
    //         //SetHitText(0);
    //         SetDamageText(baseAction.GetUnit().GetUnitStats().GetDamage());
    //         ToggleForecastText(true);
    //         //activeUnit = baseAction.GetUnit();
    //     }
    //     else
    //     {
    //         ToggleForecastText(false);
    //     }
    // }

    // private void UnitActionSystem_OnUnitActionStarted()
    // {
    //     ToggleForecastText(false);
    // }

    // private void GridMouseVisual_OnMouseOverEnemyUnit(object sender, Unit enemyUnit)
    // {
    //     if (forecastEnabled)
    //     {
    //         BattleForecast currentBattleForecast = CombatSystem.Instance.GetBattleForecast(
    //             activeUnit,
    //             enemyUnit
    //         );
    //         SetHitText(currentBattleForecast.attackingUnitChanceToHit);
    //         SetDamageText(currentBattleForecast.attackingUnitDamage);w
    //     }
    // }
}
