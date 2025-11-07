using UnityEngine;

public interface IInteractable
{
    void Interact();
    bool IsCanInteract(){return true;}
    void SetCurrentInteractable(bool isActive);
}
