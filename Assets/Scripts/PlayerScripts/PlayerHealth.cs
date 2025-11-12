using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : Health, IDamageable
{

    [ContextMenu("Die Now")]
    protected override void Die()
    {
        base.Awake();
        UIManager.Instance.ChangeUI(UIType.GAMEOVER);
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
