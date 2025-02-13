using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour, IInteractable
{
    const string ITEM_SLOT = "ItemSlot";
    
    [SerializeField] private string interactionText;
    
    public void Interact(Transform target)
    {
        Transform itemSlot = target.Find(ITEM_SLOT);

        if (itemSlot.childCount == 0)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        
            Collider col = GetComponent<Collider>();
            col.enabled = false;
        
            transform.rotation = Quaternion.identity;
            
            transform.position = itemSlot.position;
            transform.SetParent(itemSlot);
        }
        else
        {
            Debug.Log("Hands full");
        }
    }

    public void Drop(Transform origin)
    {
        transform.SetParent(null);
        
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        
        rb.AddForce(origin.forward * 5, ForceMode.Impulse);
        
        Collider col = GetComponent<Collider>();
        col.enabled = true;
    }

    public string GetInteractableText()
    {
        return interactionText;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
