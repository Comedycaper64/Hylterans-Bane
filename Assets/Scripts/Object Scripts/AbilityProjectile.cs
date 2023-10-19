using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyProjectileExploded;

    [SerializeField]
    private Transform explodeVFXPrefab;

    [SerializeField]
    private Transform projectileVisualVisual;

    [SerializeField]
    private AnimationCurve arcYAnimationCurve;

    // [SerializeField]
    // private AudioClip explosionSFX;

    private float damageAmount = 0f;

    private int damageRadius = 0;

    private bool isSpell;
    private StatType spellSave;

    private Unit attackingUnit;

    private GridPosition targetGridPosition;
    private Vector3 targetPosition;
    private Action onGrenadeBehaviourComplete;
    private float totalDistance;
    private Vector3 positionXZ;
    private bool destinationReached = false;

    private void Update()
    {
        if (destinationReached)
        {
            return;
        }

        Vector3 moveDir = (targetPosition - positionXZ).normalized;

        float moveSpeed = 10f;
        positionXZ += moveDir * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;

        float maxHeight = totalDistance / 4f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        float reachedTargetDistance = .2f;
        if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
        {
            // Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            List<Unit> hitUnits = new List<Unit>();

            // foreach (Collider collider in colliderArray)
            // {
            //     if (collider.TryGetComponent<Unit>(out Unit targetUnit))
            //     {
            //         hitUnits.Add(targetUnit);
            //     }
            // }

            // // AudioSource.PlayClipAtPoint(
            // //     explosionSFX,
            // //     Camera.main.transform.position,
            // //     SoundManager.Instance.GetSoundEffectVolume()
            // // );

            for (int x = -damageRadius; x <= damageRadius; x++)
            {
                for (int z = -damageRadius; z <= damageRadius; z++)
                {
                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > damageRadius)
                    {
                        continue;
                    }

                    GridPosition testGridPosition = targetGridPosition + new GridPosition(x, z);
                    if (
                        LevelGrid.Instance.TryGetUnitAtGridPosition(
                            testGridPosition,
                            out Unit targetUnit
                        )
                    )
                    {
                        hitUnits.Add(targetUnit);
                    }
                }
            }

            OnAnyProjectileExploded?.Invoke(this, EventArgs.Empty);

            if (explodeVFXPrefab)
                Instantiate(
                    explodeVFXPrefab,
                    targetPosition + Vector3.up * 1f,
                    Quaternion.identity
                );

            Destroy(projectileVisualVisual.gameObject);
            destinationReached = true;
            StartCoroutine(DealDamageToEachTarget(hitUnits));
        }
    }

    private IEnumerator DealDamageToEachTarget(List<Unit> targetUnits)
    {
        List<Unit> hitUnits = new List<Unit>();
        int damage = 0;
        foreach (Unit targetUnit in targetUnits)
        {
            AttackInteraction targetUnitAttackInteraction;
            if (isSpell)
            {
                targetUnitAttackInteraction = CombatSystem.Instance.TrySpell(
                    attackingUnit,
                    targetUnit,
                    spellSave
                );
            }
            else
            {
                targetUnitAttackInteraction = CombatSystem.Instance.TryAttack(
                    attackingUnit,
                    targetUnit
                );
            }
            targetUnit.PerformAOEAttack(targetUnitAttackInteraction);
            damage = targetUnitAttackInteraction.attackDamage;
            if (targetUnitAttackInteraction.attackHit)
            {
                hitUnits.Add(targetUnit);
            }
        }
        yield return new WaitForSeconds(1f);
        foreach (Unit hitUnit in hitUnits)
        {
            hitUnit.gameObject.GetComponent<Unit>().Damage(damage);
            OnAnyProjectileExploded?.Invoke(this, EventArgs.Empty);
        }
        yield return new WaitForSeconds(1f);
        onGrenadeBehaviourComplete();
    }

    public void Setup(
        GridPosition targetGridPosition,
        float damageAmount,
        int damageRadius,
        bool isSpell,
        StatType spellSave,
        Unit attackingUnit,
        Action onGrenadeBehaviourComplete
    )
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        this.targetGridPosition = targetGridPosition;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        this.damageAmount = damageAmount;
        this.damageRadius = damageRadius;
        this.isSpell = isSpell;
        this.spellSave = spellSave;
        this.attackingUnit = attackingUnit;
        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
