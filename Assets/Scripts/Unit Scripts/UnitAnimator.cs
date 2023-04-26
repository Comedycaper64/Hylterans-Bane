using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    public event EventHandler OnAttack;
    public event EventHandler OnAttackEnd;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    //[SerializeField] private Transform rifleTransform;
    //[SerializeField] private Transform swordTransform;

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
            shootAction.OnShoot += ShootAction_OnShoot;
            shootAction.OnAim += OnAttackStarted;
        }

        if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += OnAttackStarted;
            swordAction.OnSwordActionCompleted += OnAttackEnded;
        }

        if (TryGetComponent<WideSlashAction>(out WideSlashAction slashAction))
        {
            slashAction.OnSlashActionStarted += OnAttackStarted;
            slashAction.OnSlashActionCompleted += OnAttackEnded;
        }

        if (TryGetComponent<GrenadeAction>(out GrenadeAction grenadeAction))
        {
            grenadeAction.OnGrenadeActionStarted += OnAttackStarted;
            grenadeAction.OnGrenadeActionCompleted += OnAttackEnded;
        }

        if (TryGetComponent<HealthSystem>(out HealthSystem healthSystem))
        {
            healthSystem.OnDead += HealthSystem_OnDead;
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

    //Instantiates bullet for shooting visual, puts it in the correct place, gets target and lets the bulletProjectile script do the rest
    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {   
        // Transform bulletProjectileTransform = 
        //     Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);

        // BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        // Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();

        // targetUnitShootAtPosition.y = shootPointTransform.position.y;

        // bulletProjectile.Setup(targetUnitShootAtPosition);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        if (animator)
            animator.SetTrigger("Die");
    }

}
