using UnityEngine;
using UnityEngine.Events;


public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth;

    [Header("Events")]
    public UnityEvent<float, float> onHealthChanged;//mungkin nanti dibuat di player pake partial Events
    public UnityEvent onDeath;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    private bool isDead = false;
    public float healthPercentage => currentHealth / maxHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        onHealthChanged.Invoke(currentHealth, maxHealth);
    }

    public virtual void ReduceHealth(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onHealthChanged.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void IncreaseHealth(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onHealthChanged.Invoke(currentHealth, maxHealth);
    }

    protected virtual void Die()
    {
        isDead = true;
        onDeath.Invoke();
    }

}
