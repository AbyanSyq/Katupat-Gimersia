using System;
using UnityEngine;
using DG.Tweening;
public static partial class Events
{
    public static Action<float, float> OnEnemyHealthChanged;
    public static Action<float> OnEnemyTakeDamaged;
    
}
public class Enemy : MonoBehaviour
{
    [SerializeField] private Health healthComponent;
    [SerializeField] private Renderer enemyMeshRenderer;

    void OnEnable()
    {
        healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.onHealthChanged.AddListener(OnDamageChanged);
            healthComponent.onDeath.AddListener(Die);
        }
    }

    void OnDisable()
    {
        if (healthComponent != null)
        {
            healthComponent.onHealthChanged.RemoveListener(OnDamageChanged);
            healthComponent.onDeath.RemoveListener(Die);
        }
    }

    public void OnDamageChanged(float currentHealth, float maxHealth)
    {
        Events.OnEnemyHealthChanged?.Invoke(currentHealth, maxHealth);
        Events.OnEnemyTakeDamaged?.Invoke(currentHealth);

        Flash(Color.Lerp(Color.red, Color.green, currentHealth / maxHealth));
    }
    public void Flash(Color color)
    {
        // Kill any ongoing color tween (avoid overlapping flashes)
        enemyMeshRenderer.material.DOKill();


        // Change to red and then back to original
        enemyMeshRenderer.material.DOColor(color, 0.05f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);
    }
    private void Die()
    {
        Debug.Log("Enemy Died");
    }
}
