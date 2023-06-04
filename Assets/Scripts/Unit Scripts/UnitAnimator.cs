using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    public event EventHandler OnAttack;
    public event EventHandler OnAttackEnd;

    [SerializeField]
    private Animator animator;

    //Setup for all the subscriptions, making sure the components are actually attached to unit
    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnAim += OnAttackStarted;
        }

        if (TryGetComponent<FireboltAction>(out FireboltAction fireboltAction))
        {
            fireboltAction.OnAim += OnAttackStarted;
        }

        if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += OnAttackStarted;
            swordAction.OnSwordActionCompleted += OnAttackEnded;
        }

        if (TryGetComponent<CleaveAction>(out CleaveAction slashAction))
        {
            slashAction.OnCleaveActionStarted += OnAttackStarted;
            slashAction.OnCleaveActionCompleted += OnAttackEnded;
        }

        if (TryGetComponent<FireballAction>(out FireballAction grenadeAction))
        {
            grenadeAction.OnFireballActionStarted += OnAttackStarted;
            grenadeAction.OnFireballActionCompleted += OnAttackEnded;
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

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnAim -= OnAttackStarted;
        }

        if (TryGetComponent<FireboltAction>(out FireboltAction fireboltAction))
        {
            fireboltAction.OnAim -= OnAttackStarted;
        }

        if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted -= OnAttackStarted;
            swordAction.OnSwordActionCompleted -= OnAttackEnded;
        }

        if (TryGetComponent<CleaveAction>(out CleaveAction slashAction))
        {
            slashAction.OnCleaveActionStarted -= OnAttackStarted;
            slashAction.OnCleaveActionCompleted -= OnAttackEnded;
        }

        if (TryGetComponent<FireballAction>(out FireballAction grenadeAction))
        {
            grenadeAction.OnFireballActionStarted -= OnAttackStarted;
            grenadeAction.OnFireballActionCompleted -= OnAttackEnded;
        }

        if (TryGetComponent<HealthSystem>(out HealthSystem healthSystem))
        {
            healthSystem.OnDead -= HealthSystem_OnDead;
        }
    }

    private void OnAttackStarted(object sender, EventArgs e)
    {
        if (animator)
            animator.SetTrigger("IsAttacking");
        OnAttack?.Invoke(this, EventArgs.Empty);
    }

    private void OnAttackEnded(object sender, EventArgs e)
    {
        OnAttackEnd?.Invoke(this, EventArgs.Empty);
    }

    //Sets the bool based on which event has fired
    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        if (animator)
            animator.SetBool("IsWalking", true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        if (animator)
            animator.SetBool("IsWalking", false);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        if (animator)
            animator.SetTrigger("Die");
    }
}
