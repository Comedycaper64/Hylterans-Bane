// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class BoneFusionAction : BaseAction
// {
//     public static event EventHandler<GridPosition> OnAnySummonStart;
//     public static event EventHandler OnAnyBonesSelected;

//     private GameObject fusedUnit;
//     private int currentBoneCost = 0;
//     private FuseActionUI fuseActionUI;

//     [SerializeField] private GameObject cancelFuseButton;
//     [SerializeField] private GameObject summonIndicator;
//     [SerializeField] private AudioClip skeletonFused;

//     private List<BonePile> usedBonePiles;

//     private int boneCount = 0;

//     private enum State 
//     {
//         SelectingBones,
//         SelectingSummon,
//         SelectingSummonLocation,
//     }

//     private State state;
//     private GridPosition summoningPosition;

//     // Start is called before the first frame update
//     void Start()
//     {
//         fuseActionUI = GameObject.FindGameObjectWithTag("FuseActionUI").GetComponent<FuseActionUI>();
//         usedBonePiles = new List<BonePile>();
//         FuseButtonUI.OnAnySummonChosen += FuseButtonUI_OnAnySummonChosen;
//         FuseCancelUI.OnFusionCancel += FuseCancelUI_OnFuseCancel;
//     }

//     public override string GetActionName()
//     {
//         return "Fusion";
//     }

//     public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
//     {
//         return new EnemyAIAction
//         {
//             gridPosition = gridPosition,
//             actionValue = 0,
//         };
//     }

//     public override List<GridPosition> GetValidActionGridPositionList()
//     {
//         List<GridPosition> validGridPositionList = new List<GridPosition>();
//         GridPosition unitGridPosition = unit.GetGridPosition();
//         int gridX = LevelGrid.Instance.GetWidth();
//         int gridZ = LevelGrid.Instance.GetHeight();
//         switch (state)
//         {
//             default:
//             case State.SelectingBones: 
//                 for (int x = 0; x <= gridX; x++)
//                 {
//                     for (int z = 0; z <= gridZ; z++)
//                     {
//                         GridPosition offsetGridPosition = new GridPosition(x, z);
//                         GridPosition testGridPosition = offsetGridPosition;

//                         if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
//                         {
//                             continue;
//                         }

//                         GridObject testGridObject = LevelGrid.Instance.GetGridObject(testGridPosition);
//                         bool hasBones = testGridObject.HasBones();

//                         if (!hasBones)
//                         {
//                             //No bones on gridposition
//                             continue;
//                         }

//                         validGridPositionList.Add(testGridPosition);
//                     }
//                 }

//                 return validGridPositionList;
//             case State.SelectingSummon:
//                 validGridPositionList.Add(unitGridPosition);
//                 return validGridPositionList;
//             case State.SelectingSummonLocation:
//                 for (int x = -1; x <= 1; x++)
//                 {
//                     for (int z = -1; z <= 1; z++)
//                     {
//                         GridPosition offsetGridPosition = new GridPosition(x, z);
//                         GridPosition testGridPosition = summoningPosition + offsetGridPosition;

//                         if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
//                         {
//                             continue;
//                         }

//                         if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
//                         {
//                             // Grid Position already occupied with another Unit
//                             continue;
//                         }

//                         if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
//                         {
//                             continue;
//                         }

//                         validGridPositionList.Add(testGridPosition);
//                     }
//                 }
//                 return validGridPositionList;
//         }
//     }

//     public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
//     {
//         //Tally up total amount of bones amongst bone piles
//         //Open up Fusion UI to show what can be summoned
//             //Fusion UI generates a button for each unit you have the bones for, takes in selected position for fusing and allows a unit to spawn in an
//             //unnocupied space around it
//         //Select which space to summon to
//         //Remove bones from piles
//         switch (state)
//         {
//             default:
//             case State.SelectingBones:
//                 summoningPosition = gridPosition; 
//                 float fuseRadius = 3f;
//                 boneCount = 0;
//                 currentBoneCost = 0;
//                 usedBonePiles = new List<BonePile>();
//                 Collider[] colliderArray = Physics.OverlapSphere(LevelGrid.Instance.GetWorldPosition(gridPosition), fuseRadius);

//                 foreach (Collider collider in colliderArray)
//                 {
//                     if (collider.TryGetComponent<BonePile>(out BonePile bonePile))
//                     {
//                         boneCount += bonePile.GetBones();
//                         usedBonePiles.Add(bonePile);
//                     }
//                 }
//                 ActionStart(onActionComplete);
//                 OnAnyBonesSelected?.Invoke(this, EventArgs.Empty);
//                 fuseActionUI.CreateFuseButtons(boneCount);
//                 state = State.SelectingSummon;
//                 break;
//             case State.SelectingSummonLocation:
//                 while (currentBoneCost > 0)
//                 {
//                     foreach (BonePile bonePile in usedBonePiles)
//                     {
//                         if (currentBoneCost == 0) {continue;}
//                         bonePile.RemoveBones();
//                         currentBoneCost--;
//                     }
//                 }
//                 //Instantiate(fusedUnit, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
//                 SummonBuffer.Instance.AddToSummonBuffer(fusedUnit, gridPosition);
//                 Instantiate(summonIndicator, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
//                 AudioSource.PlayClipAtPoint(skeletonFused, Camera.main.transform.position, SoundManager.Instance.GetSoundEffectVolume());
//                 OnAnySummonStart?.Invoke(this, gridPosition);
//                 state = State.SelectingBones;
//                 ActionComplete();
//                 break;
//         }
//     }

//     public void CancelFusion()
//     {
//         state = State.SelectingBones;
//         fuseActionUI.ClearButtons();
//         unit.RefundActionPoints(GetActionPointsCost());
//         ActionComplete();
//     }
    
//     private void FuseButtonUI_OnAnySummonChosen(object sender, FuseButtonUI.OnSummonChosenArgs e)
//     {
//         fusedUnit = e.unitSummon;
//         currentBoneCost = e.boneCost;
//         fuseActionUI.ClearButtons();
//         state = State.SelectingSummonLocation;
//     }

//     private void FuseCancelUI_OnFuseCancel(object sender, EventArgs e)
//     {
//         CancelFusion();
//     }

//     public override int GetActionPointsCost()
//     {
//         if (state == State.SelectingSummonLocation) {return 0;}
//         return 1;
//     }
// }
