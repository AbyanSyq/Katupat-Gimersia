using UnityEngine;

public class ColliderTriggerSetter : MonoBehaviour
{
    public bool isTrigger = true;
    [ContextMenu("Set All Child Colliders To Trigger")]
    private void SetAllChildCollidersToTrigger()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(true);

        int count = 0;
        foreach (Collider col in colliders)
        {
            col.isTrigger = isTrigger;
            count++;
        }

        Debug.Log($"✅ {count} collider(s) have been set to Trigger on '{gameObject.name}'.");
    }
}
