using System;
using UnityEngine;
using DG.Tweening;
using Unity.Behavior;
using Redcode.Pools;
using Unity.VisualScripting;
using Ami.BroAudio;

public static partial class Events
{
    public static readonly GameEvent<float, float> OnEnemyHealthChanged = new GameEvent<float, float>();
    public static readonly GameEvent<float> OnEnemyTakeDamaged = new GameEvent<float>();
    public static readonly GameEvent OnEnemyDied = new GameEvent();
}

[RequireComponent(typeof(EnemyHealth))]
public class Enemy : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] private Health healthComponent;
    [SerializeField] private BehaviorGraphAgent enemyBehaviorGraphAgent;
    [SerializeField] private BlackboardVariable<bool> IsEnemyAttacking;

    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float movementSpeed = 3f;

    [Header("Enemy Attack")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackDamage = 10f; 
    [SerializeField] private Transform detectionStartPoint;
    [SerializeField] private float attackRange = 2f;
    [SerializeField, ReadOnly] private bool isEnemyInRangeAttack = false;

    public float AttackDamage => attackDamage;
    [Header("Attack Rock Appear")]
    [SerializeField] private RockFromGround rockFromGroundPrefab;
    private Pool<RockFromGround> rockPool;
    [SerializeField] private int rockTotalSpawn = 10;
    [SerializeField] private float rockAppearRange = 5f;
    [SerializeField] private float rockAppearInterval = 1f;

    

    [Header("Facing / Rotation")]
    [SerializeField, ReadOnly] private Transform playerTarget;
    [SerializeField] private float rotationLerpSpeed = 10f;

    [Header("Enemy Visuals")]
    [SerializeField] private Renderer enemyMeshRenderer;
    [SerializeField, ReadOnly] private Material enemyMaterial;

    [Header("Animation ")]
    [SerializeField, ReadOnly] private Animator enemyAnimator;
    [SerializeField, ReadOnly] private bool isDie;
    public bool IsDie => isDie;

    [Header("Audio")]
    [SerializeField] private SoundID rockSlam;

    public enum EnemyAnimationEventTriggerType
    {
        OnAttackGroundSlam,
        OnAttackRockAppear,
        OnPlaySlamSound
    }

    #region Unity Methods

    private void Awake()
    {
        enemyBehaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
        enemyBehaviorGraphAgent.GetVariable("IsEnemyAttacking", out IsEnemyAttacking);

        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemyBehaviorGraphAgent.BlackboardReference.SetVariableValue("PlayerTransform", playerTarget);

        enemyAnimator = GetComponent<Animator>();
    }
    private void Start()
    {
        rockPool = Pool.Create(rockFromGroundPrefab, rockTotalSpawn).NonLazy();
    }

    private void OnEnable()
    {
        if (healthComponent != null)
        {
            healthComponent.onHealthChanged.AddListener(OnHealthChanged);
            healthComponent.onDeath.AddListener(Die);
        }
    }

    private void OnDisable()
    {
        if (healthComponent != null)
        {
            healthComponent.onHealthChanged.RemoveListener(OnHealthChanged);
            healthComponent.onDeath.RemoveListener(Die);
        }
    }

    private void Update()
    {
        DetectPlayer();
        RotateTowardsPlayer();
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionStartPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionStartPoint.position, attackRange);
    }

    #endregion

    #region Health & Damage

    private void OnHealthChanged(float currentHealth, float maxHealth)
    {
        Events.OnEnemyHealthChanged?.Publish(currentHealth, maxHealth);
        Events.OnEnemyTakeDamaged?.Publish(currentHealth);
    }
    private void Die()
    {
        if (isDie) return;
        isDie = true;

        enemyAnimator.SetTrigger("Die");
        
        if (enemyBehaviorGraphAgent != null)
        {
            enemyBehaviorGraphAgent.enabled = false;
        }

        EnemyDamageHandler[] enemyDamageHandlers = GetComponentsInChildren<EnemyDamageHandler>();
        Collider[] enemyColliders = GetComponentsInChildren<Collider>();
        foreach (var handler in enemyDamageHandlers) handler.enabled = false;
        foreach (var col in enemyColliders) col.isTrigger = false;

        Events.OnEnemyDied?.Publish();
        this.enabled = false;
    }

    #endregion

    #region Detection & Facing

    private void DetectPlayer()
    {
        if (detectionStartPoint == null) return;

        var hitColliders = Physics.OverlapSphere(
            detectionStartPoint.position,
            attackRange,
            playerLayer
        );

        isEnemyInRangeAttack = hitColliders.Length > 0;

        if (enemyBehaviorGraphAgent != null)
        {
            enemyBehaviorGraphAgent.BlackboardReference
                .SetVariableValue(nameof(isEnemyInRangeAttack), isEnemyInRangeAttack);
        }

        if (isEnemyInRangeAttack && playerTarget == null)
        {
            playerTarget = hitColliders[0].transform;
        }
    }

    private void RotateTowardsPlayer()
    {
        if (isDie) return;
        if (playerTarget == null) return;
        if (IsEnemyAttacking == true) return;
        

        Vector3 direction = playerTarget.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationLerpSpeed * Time.deltaTime
        );
    }

    #endregion
    #region Attack Rock From Ground
    private void SpawnRocks()
    {
        for (int i = 0; i < rockTotalSpawn; i++)
        {
            // Ambil dari PoolManager berdasarkan index pool RockFromGround
            var rock = PoolManager.Instance.GetFromPool<RockFromGround>(3);
            if (rock == null) continue;

            Vector3 randomPos = transform.position + new Vector3(
                UnityEngine.Random.Range(-rockAppearRange, rockAppearRange),
                0,
                UnityEngine.Random.Range(-rockAppearRange, rockAppearRange)
            );

            // Batu pertama muncul di bawah player
            if (i == 0 && playerTarget != null)
                randomPos = playerTarget.position;

            rock.InitRock(randomPos, attackDamage);
        }
    }


    private System.Collections.IEnumerator ReturnRockToPoolAfterDelay(RockFromGround rock, float delay)
    {
        yield return new WaitForSeconds(delay);
        rockPool.Take(rock);
    }

    #endregion

    #region Animation Trigger
    
    public void OnAnimationEventTrigger(EnemyAnimationEventTriggerType type)
    {
        switch (type)
        {
            case EnemyAnimationEventTriggerType.OnAttackGroundSlam:

                break;
            case EnemyAnimationEventTriggerType.OnAttackRockAppear:
                    SpawnRocks();
                break;
            case EnemyAnimationEventTriggerType.OnPlaySlamSound:
                BroAudio.Play(rockSlam);
                break;
        }
    }

    #endregion
}