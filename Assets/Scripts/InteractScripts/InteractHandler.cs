using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractHandler : MonoBehaviour
{

    [Header("Interact Settings")]
    [FoldoutGroup("Interact"), SerializeField] private Transform detectionStartPoint;
    [FoldoutGroup("Interact"), SerializeField] private float raycastDetectionDistance = 0.5f;
    [FoldoutGroup("Interact"), SerializeField, ReadOnly] private IInteractable currentInteractable = null;
    [FoldoutGroup("Interact"), SerializeField] private LayerMask interactableLayer = ~0;

    [Header("Control")]
    [SerializeField] private bool enableInteraction = true;

    [Header("Input")]
    private PlayerInputAction playerInputAction;

    public void SetInteractionActive(bool value)
    {
        enableInteraction = value;
        if (!value && currentInteractable != null)SetCurrentInteractable(null);
    }
    public UnityEvent<IInteractable> OnCurrentInteractableChanged;

    // Call this method to interact with the current interactable object
    public void Interact()
    {
        if (!enableInteraction || currentInteractable == null) return;

        currentInteractable.Interact();
    }
    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
    }

    private void OnEnable()
    {
        playerInputAction.Player.Enable();
        playerInputAction.Player.Interact.performed += ctx => Interact();
    }
    private void OnDisable()
    {
        playerInputAction.Player.Interact.performed -= ctx => Interact();
        playerInputAction.Player.Disable();
    }

    private void Update()
    {
        if (!enableInteraction) return;
        HandleInteractDetection();
    }

    private void HandleInteractDetection()
    {
        IInteractable temp = GetInteractable();

        if (temp != null)
        {
            if (currentInteractable != temp)
            {
                SetCurrentInteractable(temp);
            }
        }
        else if (currentInteractable != null)
        {
            SetCurrentInteractable(null);
        }
    }

    private IInteractable GetInteractable()
    {
        Collider[] hitColliders = Physics.OverlapSphere(
            detectionStartPoint.position,
            raycastDetectionDistance,
            interactableLayer
        );

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.IsCanInteract()) return interactable;
            }
        }
        return null;
    }


    private void SetCurrentInteractable(IInteractable interactable)
    {
        currentInteractable?.SetCurrentInteractable(false);
        currentInteractable = interactable;
        currentInteractable?.SetCurrentInteractable(true);

        OnCurrentInteractableChanged?.Invoke(currentInteractable);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (detectionStartPoint == null) return;

        Gizmos.color = enableInteraction ? Color.green : Color.gray;
        Gizmos.DrawWireSphere(detectionStartPoint.position, raycastDetectionDistance);
    }
#endif
}
