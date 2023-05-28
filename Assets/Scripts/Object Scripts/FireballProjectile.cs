using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyFireballExploded;
    public static event EventHandler<Unit> OnDamageUnit;
    public static Action OnFinishFireballExplosion;

    [SerializeField]
    private Transform fireballExplodeVFXPrefab;

    [SerializeField]
    private Transform fireballVisual;

    [SerializeField]
    private AnimationCurve arcYAnimationCurve;

    [SerializeField]
    private AudioClip fireballExplosionSFX;

    private float damageAmount = 0f;

    private float damageRadius = 0f;

    private UnitStats attackingUnit;

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
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            List<Unit> hitUnits = new List<Unit>();

            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    hitUnits.Add(targetUnit);
                    //targetUnit.Damage((int)damageAmount);
                }
            }

            AudioSource.PlayClipAtPoint(
                fireballExplosionSFX,
                Camera.main.transform.position,
                SoundManager.Instance.GetSoundEffectVolume()
            );

            OnAnyFireballExploded?.Invoke(this, EventArgs.Empty);

            if (fireballExplodeVFXPrefab)
                Instantiate(
                    fireballExplodeVFXPrefab,
                    targetPosition + Vector3.up * 1f,
                    Quaternion.identity
                );

            Destroy(fireballVisual.gameObject);
            destinationReached = true;
            StartCoroutine(DealDamageToEachTarget(hitUnits));
        }
    }

    private IEnumerator DealDamageToEachTarget(List<Unit> targetUnits)
    {
        foreach (Unit unit in targetUnits)
        {
            OnDamageUnit?.Invoke(this, unit);
            bool unitHit = CombatSystem.Instance.TrySpell(attackingUnit, unit.GetUnitStats());
            yield return new WaitForSeconds(1f);
            if (unitHit)
            {
                int damageAmount = attackingUnit.GetDamage();
                unit.gameObject.GetComponent<Unit>().Damage(damageAmount);
                AudioSource.PlayClipAtPoint(
                    fireballExplosionSFX,
                    Camera.main.transform.position,
                    SoundManager.Instance.GetSoundEffectVolume()
                );
                OnAnyFireballExploded?.Invoke(this, EventArgs.Empty);
            }
            yield return new WaitForSeconds(1f);
        }
        OnFinishFireballExplosion?.Invoke();
        onGrenadeBehaviourComplete();
    }

    public void Setup(
        GridPosition targetGridPosition,
        float damageAmount,
        float damageRadius,
        UnitStats attackingUnit,
        Action onGrenadeBehaviourComplete
    )
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        this.damageAmount = damageAmount;
        this.damageRadius = damageRadius;
        this.attackingUnit = attackingUnit;
        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
