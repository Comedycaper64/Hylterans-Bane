using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLookAtCamera : MonoBehaviour
{
    //Object looks at camera, can be inverted (looks away from camera) if invert is true
    // [SerializeField]
    // private bool invert;
    // private bool lookAtCamera = true;

    // private Transform cameraTransform;

    // private void Awake()
    // {
    //     cameraTransform = Camera.main.transform;
    //     if (TryGetComponent<UnitAnimator>(out UnitAnimator animator))
    //     {
    //         animator.OnAttack += UnitAnimator_OnAttack;
    //         animator.OnAttackEnd += UnitAnimator_OnAttackEnd;
    //     }
    // }

    // private void OnDisable()
    // {
    //     if (TryGetComponent<UnitAnimator>(out UnitAnimator animator))
    //     {
    //         animator.OnAttack -= UnitAnimator_OnAttack;
    //         animator.OnAttackEnd -= UnitAnimator_OnAttackEnd;
    //     }
    // }

    // private void UnitAnimator_OnAttack(object sender, EventArgs e)
    // {
    //     SetLookAtCamera(false);
    // }

    // private void UnitAnimator_OnAttackEnd(object sender, EventArgs e)
    // {
    //     SetLookAtCamera(true);
    // }

    // private void LateUpdate()
    // {
    //     if (lookAtCamera)
    //     {
    //         if (invert)
    //         {
    //             Vector3 dirToCamera = (
    //                 new Vector3(cameraTransform.position.x, 0, cameraTransform.position.z)
    //                 - transform.position
    //             ).normalized;
    //             transform.LookAt(transform.position + dirToCamera * -1);
    //         }
    //         else
    //         {
    //             transform.LookAt(
    //                 new Vector3(cameraTransform.position.x, 0, cameraTransform.position.z)
    //             );
    //         }
    //     }
    // }

    // private void SetLookAtCamera(bool isLooking)
    // {
    //     lookAtCamera = isLooking;
    // }
}
