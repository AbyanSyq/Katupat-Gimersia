using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : Health, IDamageable
{
    [Header("Health Change Cooldown")]
    [SerializeField] float reduceHealthCooldown;
    [SerializeField, ReadOnly] float lastHitTime;

    void Start()
    {
        lastHitTime = 0f;
    }

    [ContextMenu("Die Now")]
    protected override void Die()
    {
        UIManager.Instance.ChangeUI(UIType.GAMEOVER);
    }    

    public void TakeDamage(float dmg, Vector3 dmgPos)
    {
        if (Time.time - lastHitTime > reduceHealthCooldown)
            ReduceHealth(dmg);

        Events.OnPlayerHealthChanged.Publish(CurrentHealth, MaxHealth);

        var controller = GetComponent<PlayerController3D>();
        if (controller != null)
        {
            controller.ApplyKnockback(dmgPos);
        }
    }
}
