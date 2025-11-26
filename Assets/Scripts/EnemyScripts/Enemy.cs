using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Behavior;
using Redcode.Pools;
using Ami.BroAudio;
using DG.Tweening;
using System.Collections;




[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BehaviorGraphAgent))]
public class Enemy : MonoBehaviour
{
    #region Configuration & References
    
    [Header("Core Components")]
    [SerializeField] private EnemyHealth healthComponent;
    [SerializeField] private BehaviorGraphAgent enemyBehaviorGraphAgent;
    [SerializeField] private Renderer enemyMeshRenderer;

    [Header("Blackboard Variables")]
    [SerializeField] private BlackboardVariable<bool> isEnemyAttacking;
    [SerializeField] private BlackboardVariable<bool> isEnemyStaggered;

    [Header("Combat Settings")]
    [SerializeField] private float attackDamage = 10f;
    
    [Header("Phase Settings")]
    [SerializeField, ReadOnly] private int currentPhase = 1; 
    [SerializeField] private int maxPhase = 2;

    // HAPUS threshold health list, ganti logic ke WeakPoint
    [Header("Golem Weak Point Settings")]
    public List<GolemWeakPoint> weakPoints; 

    [Header("Attack: Rock Skill")]
    [SerializeField] private int rockTotalSpawn = 10;
    [SerializeField] private float rockAppearRange = 5f;

    [Header("Attack: Projectile Skill")]
    [SerializeField] private Transform projectileSpawnPoint; 

    [Header("Phase: Staggered")]
    [SerializeField] private Transform staggeredCoreSpawnPoint; 
    [SerializeField] private Transform staggeredCoreTargetPoint; 
    [SerializeField] private GolemCore staggeredCoreObject;
    [SerializeField] private float staggeredCoreDuration = 5f; 
    [SerializeField,ReadOnly] private bool hasSpawnEventTriggered = false;

    [Header("Movement & Rotation")]
    [SerializeField, ReadOnly] private Transform playerTarget;
    [SerializeField] private float rotationLerpSpeed = 10f;

    [Header("Audio Settings")]
    [SerializeField] private SoundID Golem_Attack_Slam;
    [SerializeField] private SoundID Golem_Attack_Sweep;
    [SerializeField] private SoundID Golem_Attack_Grow;
    [SerializeField] private SoundID Golem_Attack_Ultimate;
    [SerializeField] private SoundID Golem_Roar;
    [SerializeField] private SoundID Golem_Wake;

    [Header("Debug & Development")]
    [SerializeField, ReadOnly] private Animator enemyAnimator;
    [SerializeField, ReadOnly] private bool isDie;
    [SerializeField, ReadOnly] private Material enemyMaterial; 

    #endregion

    #region Properties

    public float AttackDamage => attackDamage;
    public bool IsAtMaxPhase => CurrentPhase >= maxPhase;
    public int MaxPhase => maxPhase;

    public bool IsEnemyStaggered
    {
        get => isEnemyStaggered != null && isEnemyStaggered.Value;
        set
        {
            if (isEnemyStaggered != null) 
            {
                isEnemyStaggered.Value = value; 
            }
        }
    }

    public int CurrentPhase
    {
        get => currentPhase;
        private set
        {
            if (currentPhase != value)
            {
                currentPhase = value;
                
                if (enemyBehaviorGraphAgent != null)
                    enemyBehaviorGraphAgent.BlackboardReference.SetVariableValue("Phase", currentPhase);
            }
        }
    }

    public bool IsDie
    {
        get => isDie;
        set
        {
            isDie = value;
            if (enemyAnimator != null && isDie)
                enemyAnimator.SetTrigger("Die");
        }
    }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeComponents();
        InitializeBlackboard();

        CurrentPhase = 1;
    }

    private void Start()
    {
        Events.OnEnemyHealthChanged?.Publish(healthComponent.CurrentHealth, healthComponent.MaxHealth);
    
    }

    private void Update()
    {
        RotateTowardsPlayer();
    }

    void OnEnable()
    {
        Events.OnEnemyHealthChanged?.Add(OnHealthChanged);
    }
    void OnDisable()
    {
        Events.OnEnemyHealthChanged?.Remove(OnHealthChanged);
    }

    #endregion

    #region Initialization

    private void InitializeComponents()
    {
        enemyAnimator = GetComponent<Animator>();
        
        if (enemyMeshRenderer != null) 
            enemyMaterial = enemyMeshRenderer.material;

        if (playerTarget == null)
            playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void InitializeBlackboard()
    {
        if (enemyBehaviorGraphAgent == null) 
            enemyBehaviorGraphAgent = GetComponent<BehaviorGraphAgent>();

        enemyBehaviorGraphAgent.GetVariable("IsEnemyAttacking", out isEnemyAttacking);
        enemyBehaviorGraphAgent.GetVariable("IsEnemyStaggered", out isEnemyStaggered);
        
        if (playerTarget != null)
            enemyBehaviorGraphAgent.BlackboardReference.SetVariableValue("PlayerTransform", playerTarget);
    }

    #endregion

    #region Logic: Weak Point & Phase Control (NEW SYSTEM)

    // Method ini DIPANGGIL oleh script GolemWeakPoint ketika hancur
    public void OnWeakPointDestroyed()
    {
        if (CheckAllWeakPointsDestroyed())
        {
            
        }
    }

    private bool CheckAllWeakPointsDestroyed()
    {
        if (weakPoints == null || weakPoints.Count == 0) return true;

        foreach (var wp in weakPoints)
        {
            // Cek jika WeakPoint masih aktif (belum hancur)
            // Asumsi: WeakPoint yang hancur gameObject-nya di-set active false
            if (wp != null && wp.gameObject.activeSelf)
            {
                return false; // Masih ada yang hidup
            }
        }
        return true; // Semua mati
    }

    // OnHealthChanged tidak lagi mengurus Phase, hanya purely logic Health
    public void OnHealthChanged(float currentHealth, float maxHealth)
    {
        // Kosongkan logic phase disini, atau gunakan hanya untuk UI update
    }

    
    private void Revive()
    {
        healthComponent.Revive();
        GoToNextPhase();

        Debug.Log("Enemy Revived and moved to next phase.");
    }

    public void GoToNextPhase()
    {
        if (IsAtMaxPhase)
        {
            isDie = true;
        }
        else
        {
            CurrentPhase += 1;
            Debug.Log($"Going to next phase: {CurrentPhase + 1}");
        }
    }
    #endregion

    #region Logic: Staggered Core Sequence

    public void StartStaggerSequence()
    {
        IsEnemyStaggered = true;

        if (staggeredCoreObject == null)
        {
            Debug.LogWarning("[Enemy] Staggered Core Prefab belum di-assign!");
            IsEnemyStaggered = false;
            return;
        }

        healthComponent.Stagger();
        
        // Play Animasi Die (Stagger Awal)
        if(enemyAnimator != null) enemyAnimator.SetTrigger("Die");
    }

    private IEnumerator SpawnStaggeredCore()
    {
        if(isDie) yield break;
        
        // Reset Posisi
        if (staggeredCoreSpawnPoint != null)
            staggeredCoreObject.transform.position = staggeredCoreSpawnPoint.position;
        else
            staggeredCoreObject.transform.position = transform.position + Vector3.up;

        staggeredCoreObject.gameObject.SetActive(true);

        Vector3 targetPos = (staggeredCoreTargetPoint != null) ? staggeredCoreTargetPoint.position : playerTarget.position;

        Debug.Log("Move Staggered Core.");

        staggeredCoreObject.Spawn(staggeredCoreDuration);

        yield return new WaitForSeconds(staggeredCoreDuration + 0.5f);

        if (enemyAnimator != null) enemyAnimator.SetTrigger("Revive");
        Revive();
    }

    #endregion

    #region Logic: Movement & Rotation
    // ... (Sama seperti sebelumnya) ...
    private void RotateTowardsPlayer()
    {
        if (isDie || playerTarget == null) return;
        if (IsEnemyStaggered) return;
        if (isEnemyAttacking != null && isEnemyAttacking.Value) return;

        Vector3 direction = playerTarget.position - transform.position;
        direction.y = 0f; 

        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
    }
    #endregion

    #region Logic: Combat Skills
    // ... (Sama seperti sebelumnya) ...
    private void SpawnRocks()
    {
        for (int i = 0; i < rockTotalSpawn; i++)
        {
            var rock = PoolManager.Instance.GetFromPool<RockFromGround>(3); 
            if (rock == null) continue;
            Vector3 spawnPos = (i == 0 && playerTarget != null) ? playerTarget.position : 
                transform.position + new Vector3(UnityEngine.Random.insideUnitCircle.x * rockAppearRange, 0, UnityEngine.Random.insideUnitCircle.y * rockAppearRange);
            rock.InitRock(spawnPos, attackDamage);
        }
    }
    private void ShootProjectile()
    {
        if (projectileSpawnPoint == null) projectileSpawnPoint = transform; 
        var projectile = PoolManager.Instance.GetFromPool<EnemyProjectile>();
        if (projectile == null) return;
        projectile.transform.position = projectileSpawnPoint.position;
        projectile.transform.rotation = Quaternion.LookRotation(transform.forward);
        Vector3 targetPosition = playerTarget != null ? playerTarget.position : transform.forward * 10f;
        targetPosition.y += 1.0f; 
        Vector3 shootDirection = (targetPosition - projectileSpawnPoint.position).normalized;
        projectile.InitProjectile(shootDirection);
    }
    #endregion

    #region Animation Events
    public enum EnemyAnimationEventTriggerType
    {
        OnAttackGroundSlam, OnAttackRockAppear, OnShootProjectile, OnSpawnStaggeredCore, OnStaggerComplete,
        OnPlaySlamSound, OnPlaySweepSound, OnPlayWakeSound, OnPlayGrowSound, OnPlayUltimateSound, OnPlayRoarSound
    }
    public void OnAnimationEventTrigger(EnemyAnimationEventTriggerType type)
    {
        switch (type)
        {
            case EnemyAnimationEventTriggerType.OnAttackGroundSlam: break;
            case EnemyAnimationEventTriggerType.OnAttackRockAppear: SpawnRocks(); break;
            case EnemyAnimationEventTriggerType.OnShootProjectile: ShootProjectile(); break;
            case EnemyAnimationEventTriggerType.OnSpawnStaggeredCore: StartCoroutine(SpawnStaggeredCore()); break;
            case EnemyAnimationEventTriggerType.OnStaggerComplete: IsEnemyStaggered = false; break;
            case EnemyAnimationEventTriggerType.OnPlaySlamSound: BroAudio.Play(Golem_Attack_Slam); break;
            case EnemyAnimationEventTriggerType.OnPlaySweepSound: BroAudio.Play(Golem_Attack_Sweep); break;
            case EnemyAnimationEventTriggerType.OnPlayWakeSound: BroAudio.Play(Golem_Wake); break;
            case EnemyAnimationEventTriggerType.OnPlayGrowSound: BroAudio.Play(Golem_Attack_Grow); break;
            case EnemyAnimationEventTriggerType.OnPlayUltimateSound: BroAudio.Play(Golem_Attack_Ultimate); break;
            case EnemyAnimationEventTriggerType.OnPlayRoarSound: BroAudio.Play(Golem_Roar); break;
        }
    }
    #endregion

    [ContextMenu("Get All WeakPoints")]
    public void GetAllWeakPointa()
    {
        weakPoints = new List<GolemWeakPoint>(GetComponentsInChildren<GolemWeakPoint>());
    }
}