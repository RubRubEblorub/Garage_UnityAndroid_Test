using UnityEngine;

public interface IInteractable
{
    void Interact(Transform target);
    public void Drop(Transform origin);
    string GetInteractableText();
    Transform GetTransform();
}