using System;
using UnityEngine;
using DG.Tweening;
using Unity.Behavior;

public static partial class Events
{
    public static Action<float, float> OnEnemyHealthChanged;
    public static Action<float> OnEnemyTakeDamaged;
}

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health healthComponent;
    [SerializeField] private BehaviorGraphAgent enemyBehaviorGraphAgent;

    [Header("Enemy Attack")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackDamage = 10f; 
    [SerializeField] private Transform detectionStartPoint;
    [SerializeField] private float attackRange = 2f;
    [SerializeField, ReadOnly] private bool isEnemyInRangeAttack = false;

    [Header("Facing / Rotation")]
    [SerializeField, ReadOnly] private Transform playerTarget;
    [SerializeField] private float rotationLerpSpeed = 10f;

    [Header("Enemy Visuals")]
    [SerializeField] private Renderer enemyMeshRenderer;
    [SerializeField, ReadOnly] private Material enemyMaterial;

    #region Unity Methods

    private void Awake()
    {
        if (healthComponent == null)
            healthComponent = GetComponent<Health>();

        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
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
        Events.OnEnemyHealthChanged?.Invoke(currentHealth, maxHealth);
        Events.OnEnemyTakeDamaged?.Invoke(currentHealth);

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
}
