using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Etouch = UnityEngine.InputSystem.EnhancedTouch;

public class MovementController : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private TouchJoystick joystick;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float maxSpeed = 100f;
    
    private Rigidbody playerRB;
    private Finger movementFinger;

    private Vector2 input;
    private Vector2 joystickSize = new(300f, 300f);
    private Vector3 moveDir;
    private float horizontalInput, verticalInput;

    public float velocity;

    private void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerRB.freezeRotation = true;
    }

    private void Update()
    {
        velocity = playerRB.velocity.magnitude;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        Etouch.Touch.onFingerDown += HandleFingerDown;
        Etouch.Touch.onFingerUp += HandleLoseFinger;
        Etouch.Touch.onFingerMove += HandleFingerMove;
        
        move.action.Enable();
        move.action.started += GetInput;
        move.action.performed += GetInput;
        move.action.canceled += ResetInput;
    }
    
    private void OnDisable()
    {
        move.action.started -= GetInput;
        move.action.performed -= GetInput;
        move.action.canceled -= ResetInput;
        move.action.Disable();
        
        Etouch.Touch.onFingerDown -= HandleFingerDown;
        Etouch.Touch.onFingerUp -= HandleLoseFinger;
        Etouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();
    }

    private void HandleFingerDown(Finger obj)
    {
        if (movementFinger == null && obj.screenPosition.x < Screen.width / 2)
        {
            movementFinger = obj;
            input = Vector2.zero;
        }
    }

    private void HandleLoseFinger(Finger obj)
    {
        if (obj == movementFinger)
        {
            movementFinger = null;
            joystick.knob.anchoredPosition = Vector2.zero;
            input = Vector2.zero;
        }
    }

    private void HandleFingerMove(Finger obj)
    {
        if (obj == movementFinger)
        {
            Vector2 knobPos;
            float maxMovement = joystickSize.x / 2f;
            Etouch.Touch curTouch = obj.currentTouch;

            if (Vector2.Distance(
                    curTouch.screenPosition,
                    joystick.rectTransform.anchoredPosition
                ) > maxMovement)
            {
                knobPos = (curTouch.screenPosition - joystick.rectTransform.anchoredPosition).normalized * maxMovement;
            }
            else
            {
                knobPos = curTouch.screenPosition - joystick.rectTransform.anchoredPosition;
            }
            
            joystick.knob.anchoredPosition = knobPos;
            input = knobPos / maxMovement;
        }
    }

    private void GetInput(InputAction.CallbackContext obj)
    {
        input = obj.ReadValue<Vector2>();
    }
    
    private void ResetInput(InputAction.CallbackContext obj)
    {
        input = Vector2.zero;
    }
    
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        moveDir = transform.forward * input.y + transform.right * input.x;
         
        playerRB.AddForce(moveDir.normalized * moveSpeed, ForceMode.Force);
        
        Vector3 velocityXZ = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);

        //Limit player speed
        if (velocityXZ.magnitude > maxSpeed)
        {
            Vector3 maxVelocity = velocityXZ.normalized * maxSpeed;
            playerRB.velocity = new Vector3(maxVelocity.x, playerRB.velocity.y, maxVelocity.z);
        }
    }
}
