using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    //Camera controls
    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 12f;

    private float xAxisCameraRange;
    private float zAxisCameraRange;

    private Transform defaultFollowTarget;
    private Transform dwarfFollowTarget;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffset;

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        defaultFollowTarget = cinemachineVirtualCamera.Follow;
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
        xAxisCameraRange = LevelGrid.Instance.GetWidth() * LevelGrid.Instance.GetCellSize();
        zAxisCameraRange = LevelGrid.Instance.GetHeight() * LevelGrid.Instance.GetCellSize();
        EnemyAI.Instance.OnEnemyUnitBeginAction += EnemyAI_OnEnemyUnitBeginAction;
        EnemyAI.Instance.OnEnemyTurnFinished += EnemyAI_OnEnemyTurnFinished;
    }

    private void LateUpdate()
    {
        HandleZoom();
        HandleRotation();
        if (!TurnSystem.Instance.IsPlayerTurn() && dwarfFollowTarget) 
        {
            FollowEnemyUnit();
            return;
        }
        HandleMovement();
    }

    private void FollowEnemyUnit()
    {
        transform.position = dwarfFollowTarget.position;
    }

    private void HandleMovement()
    {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();

        float moveSpeed = 10f;
        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        Vector3 movementThisFrame = moveVector * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + movementThisFrame;
        transform.position = new Vector3(Mathf.Clamp(newPosition.x, 0, xAxisCameraRange), transform.position.y, Mathf.Clamp(newPosition.z, 0, zAxisCameraRange));
        //transform.position += Mathf.Clamp(movementThisFrame, new Vector3(0, 0, 0), new Vector3(xAxisCameraRange, 0, zAxisCameraRange)); 
        //Mathf.Clamp(transform.position.x, 0, xAxisCameraRange);
        //Mathf.Clamp(transform.position.z, 0, zAxisCameraRange);
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);
        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomIncreaseAmount = 0.01f;
        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;   

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        float zoomSpeed = 5f;
        cinemachineTransposer.m_FollowOffset =
            Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }

    private void EnemyAI_OnEnemyUnitBeginAction(object sender, Unit e)
    {
        dwarfFollowTarget = e.transform;
        //cinemachineVirtualCamera.Follow = e.gameObject.transform;
        //cinemachineVirtualCamera.LookAt = e.gameObject.transform;
    }

    private void EnemyAI_OnEnemyTurnFinished(object sender, EventArgs e)
    {
        dwarfFollowTarget = null;
        //cinemachineVirtualCamera.Follow = defaultFollowTarget;
        //cinemachineVirtualCamera.LookAt = defaultFollowTarget;
    }

}
