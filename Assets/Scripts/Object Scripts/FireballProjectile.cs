using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField]
    private Transform fireballExplodeVFXPrefab;

    [SerializeField]
    private AnimationCurve arcYAnimationCurve;

    [SerializeField]
    private AudioClip fireballExplosionSFX;

    private float damageAmount = 0f;

    private float damageRadius = 0f;

    private Vector3 targetPosition;
    private Action onGrenadeBehaviourComplete;
    private float totalDistance;
    private Vector3 positionXZ;

    private void Update()
    {
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

            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage((int)damageAmount);
                }
            }

            AudioSource.PlayClipAtPoint(
                fireballExplosionSFX,
                Camera.main.transform.position,
                SoundManager.Instance.GetSoundEffectVolume()
            );

            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

            if (fireballExplodeVFXPrefab)
                Instantiate(
                    fireballExplodeVFXPrefab,
                    targetPosition + Vector3.up * 1f,
                    Quaternion.identity
                );

            Destroy(gameObject);

            onGrenadeBehaviourComplete();
        }
    }

    public void Setup(
        GridPosition targetGridPosition,
        float damageAmount,
        float damageRadius,
        Action onGrenadeBehaviourComplete
    )
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        this.damageAmount = damageAmount;
        this.damageRadius = damageRadius;
        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
