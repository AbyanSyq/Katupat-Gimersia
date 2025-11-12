using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : Health, IDamageable
{
    [Header("Health Change Cooldown")]
    [SerializeField] float reduceHealthCooldown;
    [SerializeField, ReadOnly] float lastHitTime;
    [SerializeField] private CinemachineImpulseSource takeDamageImpulseSource;
    [SerializeField] private float impulseIntensity = 0.1f;

    void Start()
    {
        lastHitTime = 0f;
    }

    [ContextMenu("Die Now")]
    protected override void Die()
    {
        base.Awake();
        UIManager.Instance.ChangeUI(UIType.GAMEOVER);
    }    

    public void TakeDamage(float dmg, Vector3 dmgPos)
    {
        takeDamageImpulseSource.GenerateImpulse(impulseIntensity);
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
