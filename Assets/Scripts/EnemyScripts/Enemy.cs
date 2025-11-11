using System;
using UnityEngine;
using DG.Tweening;
using Unity.Behavior;

public static partial class Events
{
    public static readonly GameEvent<float, float> OnEnemyHealthChanged = new GameEvent<float, float>();
    public static readonly GameEvent<float> OnEnemyTakeDamaged = new GameEvent<float>();
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
    [SerializeField,ReadOnly] private RockFromGround rockFromGroundObject;
    [SerializeField] private float damageRockFromGround;

    [Header("Facing / Rotation")]
    [SerializeField, ReadOnly] private Transform playerTarget;
    [SerializeField] private float rotationLerpSpeed = 10f;

    [Header("Enemy Visuals")]
    [SerializeField] private Renderer enemyMeshRenderer;
    [SerializeField, ReadOnly] private Material enemyMaterial;

    public enum EnemyAnimationEventTriggerType
    {
        OnAttackGroundSlam,
        OnAttackRockAppear,
    }

    #region Unity Methods

    private void Awake()
    {
        enemyBehaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
        enemyBehaviorGraphAgent.GetVariable("IsEnemyAttacking", out IsEnemyAttacking);

        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemyBehaviorGraphAgent.BlackboardReference.SetVariableValue("PlayerTransform", playerTarget);
    }
    private void Start()
    {
        rockFromGroundObject = Instantiate(rockFromGroundPrefab);
        rockFromGroundObject.gameObject.SetActive(false);
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

        if (enemyMaterial == null) return;

        Flash(Color.red);
    }

    private void Flash(Color color)
    {
        enemyMaterial.DOKill(true); 
        enemyMaterial.color = Color.white;

        enemyMaterial.DOColor(color, 0.05f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                enemyMaterial.color = Color.white;
            });
    }

    private void Die()
    {
        Debug.Log("Enemy Died");
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
    

    #endregion

    #region Animation Trigger
    
    public void OnAnimationEventTrigger(EnemyAnimationEventTriggerType type)
    {
        switch (type)
        {
            case EnemyAnimationEventTriggerType.OnAttackGroundSlam:

                break;
            case EnemyAnimationEventTriggerType.OnAttackRockAppear:
                rockFromGroundObject.initRock(playerTarget.position, attackDamage);
                break;
        }
    }

    #endregion
}