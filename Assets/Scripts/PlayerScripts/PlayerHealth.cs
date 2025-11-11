using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : Health, IDamageable
{
    protected override void Awake()
    {
        base.Awake();

        // Ensure the UnityEvent is initialized and subscribe to death
        if (onDeath == null) onDeath = new UnityEvent();
        onDeath.AddListener(OnPlayerDeath);

        if (onHealthChanged == null) onHealthChanged = new UnityEvent<float, float>();
        onHealthChanged.AddListener(OnPlayerHealthChanged);
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid potential memory leaks
        if (onDeath != null)
            onDeath.RemoveListener(OnPlayerDeath);
    }

    /// <summary>
    /// Called when this player's health reaches zero.
    /// Disable player input/controller and play death animation if available.
    /// </summary>
    private void OnPlayerDeath()
    {
        Debug.Log("Player has died.");
        // UIManager.Instance.ShowUI(UIType.GAMEOVER);

        // Disable movement script if present
        // var controller = GetComponent<PlayerController3D>();
        // if (controller != null)
        //     controller.enabled = false;

        // // Disable input handler (if your input handler exposes a SetInput method)
        // var inputHandler = GetComponent<PlayerInputHandler>();
        // if (inputHandler != null)
        // {
        //     // Try to disable input to prevent further player actions
        //     try { inputHandler.SetInput(false); } catch { }
        // }

        // // Disable CharacterController to stop physics/character movement
        // var charController = GetComponent<CharacterController>();
        // if (charController != null)
        //     charController.enabled = false;

        // // Trigger death animation if animator exists
        // var animator = GetComponent<Animator>();
        // if (animator != null)
        // {
        //     if (animator.HasState(0, Animator.StringToHash("Die")))
        //         animator.SetTrigger("Die");
        // }

        // You can add additional game-specific behavior here: show UI, respawn, etc.
    }

    public void TakeDamage(float dmg, Vector3 dmgPos)
    {
        ReduceHealth(dmg);
    }
    
    void OnPlayerHealthChanged(float currentHealth, float maxHealth)
    {
        // handle health change
        Debug.Log("Player health changed: " + currentHealth + "/" + maxHealth);
    }

    public override void ReduceHealth(float amount)
    {
        base.ReduceHealth(amount);
    }
}
