using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Etouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerInteractSystem : MonoBehaviour
{
    [SerializeField] private InputActionReference interact;
    [SerializeField] private Transform itemSlot;
    [SerializeField] private InputActionReference drop;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float interactDistance;
    [SerializeField] private float yInteractionOffset;

    private void OnEnable()
    {
        interact.action.Enable();
        interact.action.performed += Interact;
        
        drop.action.Enable();
        drop.action.performed += Drop;
        
        EnhancedTouchSupport.Enable();
        Etouch.Touch.onFingerDown += HandleFingerDown;
    }

    private void OnDisable()
    {
        interact.action.performed -= Interact;
        interact.action.Disable();
        
        drop.action.performed -= Drop;
        drop.action.Disable();
        
        Etouch.Touch.onFingerDown -= HandleFingerDown;
        EnhancedTouchSupport.Disable();
    }

    //Touch interaction only
    private void HandleFingerDown(Finger obj)
    {
        Vector2 touchPos = obj.currentTouch.screenPosition;
        
        Ray ray = mainCamera.ScreenPointToRay(touchPos);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(transform);
            }
        }
    }

    //Non-touch interaction
    private void Interact(InputAction.CallbackContext obj)
    {
        IInteractable interactableObject = GetInteractableObject();
        if (interactableObject != null)
        {
            interactableObject.Interact(transform);
        }
    }
    
    private void Drop(InputAction.CallbackContext obj)
    {
        DropItem();
    }

    public void DropItem()
    {
        if (itemSlot.childCount != 0)
        {
            GameObject item = itemSlot.GetChild(0).gameObject;
            
            IInteractable interactableObject = item.GetComponent<IInteractable>();
            
            interactableObject.Drop(transform);
        }
    }

    private IInteractable GetInteractableObject()
    {
        Vector3 rayOrigin = mainCamera.transform.position + new Vector3(0, yInteractionOffset, 0);
        Vector3 rayDirection = mainCamera.transform.forward;
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                return interactable;
            }
        }
        return null;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 rayOrigin = mainCamera.transform.position + new Vector3(0, yInteractionOffset, 0);
        Gizmos.DrawRay(rayOrigin, mainCamera.transform.forward * interactDistance);
    }
}
