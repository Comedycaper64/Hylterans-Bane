// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class SummonBuffer : MonoBehaviour
// {
//     public static SummonBuffer Instance { get; private set; }
//     private List<SummonData> summonedUnits;

//     private void Awake()
//     {
//         //Singleton-ed and Instance-d
//         if (Instance != null)
//         {
//             Debug.LogError("There's more than one SummonBuffer! " + transform + " - " + Instance);
//             Destroy(gameObject);
//             return;
//         }
//         Instance = this;
//     }

//     void Start()
//     {
//         summonedUnits = new List<SummonData>();
//         TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;   
//     }

//     public void AddToSummonBuffer(GameObject unit, GridPosition summonPosition)
//     {
//         summonedUnits.Add(new SummonData(unit, summonPosition));
//     }

//     private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
//     {
//         if (!TurnSystem.Instance.IsPlayerTurn()) {return;}
        
//         foreach(SummonData summon in summonedUnits)
//         {
//             Instantiate(summon.unit, LevelGrid.Instance.GetWorldPosition(summon.summonPosition), Quaternion.identity);
//         }
//         summonedUnits.Clear();
//     }
// }
