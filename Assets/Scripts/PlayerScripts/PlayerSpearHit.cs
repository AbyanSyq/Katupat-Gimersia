using Unity.Cinemachine;
using UnityEngine;

public class PlayerSpearHit : MonoBehaviour
{
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] float impulseIntensity;
    [SerializeField] float lastBossHealth;
    [SerializeField] float currentBossHealth;

    void Start()
    {
        lastBossHealth  = 0;
    }

    void OnEnable()
    {
        Events.OnEnemyHealthChanged.Add(OnEnemyHealthChanged);
    }
    void OnDisable()
    {
        Events.OnEnemyHealthChanged.Remove(OnEnemyHealthChanged);
    }

    public void OnEnemyHealthChanged(float currentHealth, float maxHealth)
    {
        if (lastBossHealth <= 0) lastBossHealth = currentHealth;

        Vector3 randomDir = UnityEngine.Random.insideUnitSphere.normalized * 0.2f;
        
        if (lastBossHealth - currentHealth <= 5f)
            impulseSource.GenerateImpulse(impulseIntensity * 0.5f);
        else
            impulseSource.GenerateImpulse(impulseIntensity);
        
        lastBossHealth = currentHealth;
    }

}
