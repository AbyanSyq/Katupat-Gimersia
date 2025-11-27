using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;


public static partial class Events
{
    public static readonly GameEvent<float, float> OnEnemyHealthChanged = new GameEvent<float, float>();
    public static readonly GameEvent<float> OnEnemyTakeDamaged = new GameEvent<float>();
    public static readonly GameEvent OnEnemyDied = new GameEvent();
}

    public class EnemyHealth : Health
{
    [Header("References")]
    [SerializeField,ReadOnly] private Enemy enemyComponent;
    [SerializeField] private BehaviorGraphAgent enemyBehaviorGraphAgent;
    [SerializeField] private BlackboardVariable<OnEnemyHurt> OnEnemyHurtEventChannel;
    [SerializeField] private BlackboardVariable<bool> IsGetHurt;

    [Header("Enemy Health Settings")]
    [SerializeField] private int hitCount;
    [SerializeField] private int hitCombo;

    private EnemyDamageHandler[] enemyDamageHandlers;
    private Collider[] enemyColliders;
    

    // [Header("WeakPoint Settings")]

    

    protected override void Awake()
    {
        base.Awake();
        enemyComponent = GetComponent<Enemy>();

        enemyDamageHandlers = GetComponentsInChildren<EnemyDamageHandler>(true);
        enemyColliders = GetComponentsInChildren<Collider>(true);

        enemyBehaviorGraphAgent.BlackboardReference.GetVariable<OnEnemyHurt>("OnEnemyHurt", out OnEnemyHurtEventChannel);
        enemyBehaviorGraphAgent.BlackboardReference.GetVariable<bool>("IsGetHurt", out IsGetHurt);
    }
    void Start()
    {
        enemyBehaviorGraphAgent.BlackboardReference.SetVariableValue("HealthPercentage", healthPercentage);
    }

    public override void ReduceHealth(float amount)
    {
        base.ReduceHealth(amount );

        enemyBehaviorGraphAgent.BlackboardReference.SetVariableValue("Health", currentHealth);
        enemyBehaviorGraphAgent.BlackboardReference.SetVariableValue("HealthPercentage", healthPercentage);
        Events.OnEnemyHealthChanged?.Publish(currentHealth, maxHealth);
        Events.OnEnemyTakeDamaged?.Publish(currentHealth);
    }
    public void Revive()
    {
        // Reset HP
        currentHealth = maxHealth;
        Debug.Log($"Enemy Revived with {currentHealth} HP");

        // Update ke Blackboard & event
        if (enemyBehaviorGraphAgent != null)
        {
            enemyBehaviorGraphAgent.BlackboardReference.SetVariableValue("Health", currentHealth);
            enemyBehaviorGraphAgent.BlackboardReference.SetVariableValue("HealthPercentage", healthPercentage);
        }

        Events.OnEnemyHealthChanged?.Publish(currentHealth, maxHealth);

        // Hidupkan lagi AI
        if (enemyBehaviorGraphAgent != null)
            enemyBehaviorGraphAgent.enabled = true;

        // Aktifkan lagi damage handler dan collider
        if (enemyDamageHandlers == null || enemyDamageHandlers.Length == 0)
            enemyDamageHandlers = GetComponentsInChildren<EnemyDamageHandler>(true);
        if (enemyColliders == null || enemyColliders.Length == 0)
            enemyColliders = GetComponentsInChildren<Collider>(true);

        foreach (var handler in enemyDamageHandlers)
        {
            if (handler != null)
                handler.enabled = true;
        }

        foreach (var col in enemyColliders)
        {
            if (col != null)
                col.enabled = true;
        }

        // Clear flag mati di Enemy
        if (enemyComponent != null)
            enemyComponent.IsDie = false;

        // Optionally: reset flag status lain di blackboard
        IsGetHurt.Value = false;
    }
    public void Stagger()
    {
        DisableDamageHandlersAndColliders();
    }
    protected override void Die()
    {
        if(!((enemyComponent.CurrentPhase + 1) > enemyComponent.MaxPhase)) {
            enemyComponent.StartStaggerSequence();
            return;
        }
        base.Die();
        Events.OnEnemyDied?.Publish();


        if (enemyComponent != null)
        {
            enemyComponent.IsDie = true;
        }

        if (enemyBehaviorGraphAgent != null)
        {
            enemyBehaviorGraphAgent.enabled = false;
        }
        // Matikan damage handler & collider
        DisableDamageHandlersAndColliders();
    }
    private void DisableDamageHandlersAndColliders()
    {
        if (enemyDamageHandlers == null || enemyDamageHandlers.Length == 0)
            enemyDamageHandlers = GetComponentsInChildren<EnemyDamageHandler>(true);
        if (enemyColliders == null || enemyColliders.Length == 0)
            enemyColliders = GetComponentsInChildren<Collider>(true);

        foreach (var handler in enemyDamageHandlers)
        {
            if (handler != null)
                handler.enabled = false;
        }

        foreach (var col in enemyColliders)
        {
            if (col != null)
                col.enabled = false;
        }
    }

    [ContextMenu("Test Hurt Event")]
    private void TestHurtEvent()
    {
        Debug.Log("Test Enemy Hurt Event Channel Send Event Message");
        OnEnemyHurtEventChannel.Value.SendEventMessage();
    }
    [ContextMenu("Test Get Hurt Event")]
    private void TestGetHurtEvent()
    {
        Debug.Log("Test Enemy Get Hurt Set IsGetHurt to True");
        IsGetHurt.Value = true;
    }
}
