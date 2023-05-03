using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private Controls controls;

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
        return controls.Player.LeftClick.WasPressedThisFrame();
    }

    public bool IsRightClickDownThisFrame()
    {
        return controls.Player.RightClick.WasPressedThisFrame();
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
}
