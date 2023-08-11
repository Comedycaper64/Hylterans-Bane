using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private List<BaseAction> unitActionList = new List<BaseAction>();

    [SerializeField]
    private Animator animator;

    private void Start()
    {
        unitActionList = GetComponent<Unit>().GetBaseActionList();

        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        foreach (BaseAction action in unitActionList)
        {
            if (action.ActionDealsDamage())
            {
                action.OnActionStarted += OnAttackStarted;
            }
        }

        if (TryGetComponent<RallyingCry>(out RallyingCry rallyingCry))
        {
            rallyingCry.OnAbilityStarted += OnAttackStarted;
        }

        if (TryGetComponent<HealthSystem>(out HealthSystem healthSystem))
        {
            healthSystem.OnDead += HealthSystem_OnDead;
        }
    }

    private void OnDisable()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving -= MoveAction_OnStartMoving;
            moveAction.OnStopMoving -= MoveAction_OnStopMoving;
        }

        foreach (BaseAction action in unitActionList)
        {
            if (action.ActionDealsDamage())
            {
                action.OnActionStarted -= OnAttackStarted;
            }
        }

        if (TryGetComponent<RallyingCry>(out RallyingCry rallyingCry))
        {
            rallyingCry.OnAbilityStarted -= OnAttackStarted;
        }

        if (TryGetComponent<HealthSystem>(out HealthSystem healthSystem))
        {
            healthSystem.OnDead -= HealthSystem_OnDead;
        }
    }

    private void OnAttackStarted(object sender, EventArgs e)
    {
        animator.SetTrigger("IsAttacking");
    }

    //Sets the bool based on which event has fired
    private void MoveAction_OnStartMoving(object sender, int e)
    {
        animator.SetBool("IsWalking", true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", false);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        animator.SetTrigger("Die");
    }
}
