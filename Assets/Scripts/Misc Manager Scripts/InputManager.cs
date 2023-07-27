using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, Controls.IPlayerActions
{
    public static InputManager Instance { get; private set; }

    private Controls controls;

    private bool leftClickHeld;
    private bool rightClickHeld;
    public Action OnRallyingCryEvent;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InputManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        controls = new Controls();
        controls.Enable();
    }

    public Vector2 GetMouseScreenPosition()
    {
        return Mouse.current.position.ReadValue();
    }

    public bool IsLeftClickDownThisFrame()
    {
        return leftClickHeld;
    }

    public bool IsRightClickDownThisFrame()
    {
        return rightClickHeld;
    }

    public Vector2 GetCameraMoveVector()
    {
        return controls.Player.CameraMove.ReadValue<Vector2>();
    }

    public float GetCameraRotateAmount()
    {
        return controls.Player.CameraRotate.ReadValue<float>();
    }

    public float GetCameraZoomAmount()
    {
        return controls.Player.CameraZoom.ReadValue<Vector2>().y;
    }

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnCameraZoom(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnCameraRotate(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            leftClickHeld = true;
        }
        else if (context.canceled)
        {
            leftClickHeld = false;
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rightClickHeld = true;
        }
        else if (context.canceled)
        {
            rightClickHeld = false;
        }
    }

    public void OnRallyingCry(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        OnRallyingCryEvent?.Invoke();
    }
}
