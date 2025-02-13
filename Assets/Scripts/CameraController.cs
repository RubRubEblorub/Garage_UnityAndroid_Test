using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Etouch = UnityEngine.InputSystem.EnhancedTouch;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputActionReference look;
    [SerializeField] private TouchJoystick joystick;
    [SerializeField] private Transform orientation;
    [SerializeField] private bool touchControl;
    [SerializeField] private float xMouseSens, yMouseSens;
    
    private Finger lookFinger;
    
    private float xRotation, yRotation;
    public float xInput, yInput;
    private Vector2 joystickSize = new(300f, 300f);
    public Vector2 input;

    private void Start()
    {
        if (!touchControl)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
    }

    private void OnEnable()
    {
        look.action.Enable();
        look.action.performed += GetInput;
        look.action.canceled += ResetInput;
        
        EnhancedTouchSupport.Enable();
        Etouch.Touch.onFingerDown += HandleFingerDown;
        Etouch.Touch.onFingerUp += HandleLoseFinger;
        Etouch.Touch.onFingerMove += HandleFingerMove;
    }

    private void OnDisable()
    {
        look.action.performed -= GetInput;
        look.action.canceled -= ResetInput;
        look.action.Disable();
        
        Etouch.Touch.onFingerDown -= HandleFingerDown;
        Etouch.Touch.onFingerUp -= HandleLoseFinger;
        Etouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();
    }
    
    private void HandleFingerDown(Finger obj)
    {
        if (lookFinger == null && obj.screenPosition.x > Screen.width / 2)
        {
            lookFinger = obj;
            input = Vector2.zero;
        }
    }

    private void HandleLoseFinger(Finger obj)
    {
        if (obj == lookFinger)
        {
            lookFinger = null;
            joystick.knob.anchoredPosition = Vector2.zero;
            input = Vector2.zero;
        }
    }

    private void HandleFingerMove(Finger obj)
    {
        if (obj == lookFinger)
        {
            //Convert .screenPosition 'cause otherwise it doesn't work correctly
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                joystick.rectTransform, 
                obj.currentTouch.screenPosition, 
                null, 
                out localPoint);
            
            float maxMovement = joystickSize.x / 2f;
            Vector2 knobPos = localPoint.magnitude > maxMovement ? localPoint.normalized * maxMovement : localPoint;
            
            joystick.knob.anchoredPosition = knobPos;
            input = knobPos / maxMovement;
        }
    }

    private void Update()
    {
        RotatePlayer();
    }
    
    private void GetInput(InputAction.CallbackContext obj)
    {
        input = obj.ReadValue<Vector2>();
    }
    
    private void ResetInput(InputAction.CallbackContext obj)
    {
        input = Vector2.zero;
    }

    private void RotatePlayer()
    {
        xInput = input.x * Time.deltaTime * xMouseSens;
        yInput = input.y * Time.deltaTime * yMouseSens;
        
        yRotation += xInput;
        xRotation -= yInput;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
