using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    //Object looks at camera, can be inverted (looks away from camera) if invert is true
    [SerializeField] private bool invert;

    private Transform cameraTransform;


    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }


    private void LateUpdate()
    {
        if (invert)
        {
            Vector3 dirToCamera = (cameraTransform.position - transform.position).normalized;
            transform.LookAt(transform.position + dirToCamera * -1);
        } 
        else
        {
            transform.LookAt(cameraTransform);
        }
        
    }
    
}
