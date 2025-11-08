using UnityEngine;

public class TakeDamageHandler : MonoBehaviour, IDamageable
{
    public Health healthComponent;
    public void TakeDamage(float amount)
    {
        if (healthComponent != null)
        {
            healthComponent.ReduceHealth(amount);
        }
    }
}
