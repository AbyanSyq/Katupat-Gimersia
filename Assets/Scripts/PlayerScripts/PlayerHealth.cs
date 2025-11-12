using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : Health, IDamageable
{
    private void OnDestroy()
    {
        if (onDeath != null)
            onDeath.RemoveListener(OnPlayerDeath);
    }

    private void OnPlayerDeath()
    {
        Debug.Log("Player has died.");
    }

    public void TakeDamage(float dmg, Vector3 dmgPos)
    {
        ReduceHealth(dmg);

        Events.OnPlayerHealthChanged.Publish(CurrentHealth, MaxHealth);

        var controller = GetComponent<PlayerController3D>();
        if (controller != null)
        {
            controller.ApplyKnockback(dmgPos);
        }
    }
}
