using UnityEngine;

public class AddScriptToAllCollider : MonoBehaviour
{
    [ContextMenu("Add TakeDamageHandler to All Children With Collider")]
    private void AddDamageHandler()
    {
        TakeDamageHandler[] handlers = GetComponentsInChildren<TakeDamageHandler>(true);
        int addedCount = 0;

        foreach (TakeDamageHandler handler in handlers)
        {
            handler.healthComponent = GetComponent<Health>();
        }

        Debug.Log($"✅ Added TakeDamageHandler to {addedCount} GameObject(s) under '{gameObject.name}'.");
    }
}
