using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] private bool canInteract = true;
    public UnityEvent onInteract;
    public UnityEvent onCanInteract;
    public UnityEvent onCantInteract;

    public virtual void Interact()
    {
        onInteract.Invoke();
    }
    public virtual bool IsCanInteract()
    {
        return canInteract;
    }
    public virtual void SetCurrentInteractable(bool isActive)
    {
        if (isActive) onCanInteract.Invoke();
        else onCantInteract.Invoke();
    }
}