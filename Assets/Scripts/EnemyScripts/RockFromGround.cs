using System.Collections;
using UnityEngine;
using Redcode.Pools;

public class RockFromGround : MonoBehaviour, IPoolObject
{
    [Header("Damage Settings")]
    [SerializeField] private Transform damageStartPoint;
    [SerializeField] private float damageRange = 2f;
    [SerializeField] private LayerMask damageLayer;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float yOffset = 0.6f;

    [Header("Visuals")]
    [SerializeField] private ParticleSystem RockVisual;

    [Header("Pool Settings")]
    [SerializeField] private int poolIndex = 3; // index pool di PoolManager

    private Coroutine attackRoutine;

    // dipanggil 1x ketika prefab pertama kali dibuat oleh pool
    public void OnCreatedInPool()
    {
        if (RockVisual != null)
            RockVisual.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    // dipanggil setiap kali object diambil dari pool
    public void OnGettingFromPool()
    {
        if (RockVisual != null)
            RockVisual.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Panggil untuk mengaktifkan serangan batu di posisi target.
    /// </summary>
    public void InitRock(Vector3 targetPosition, float damage)
    {
        transform.position = new Vector3(targetPosition.x, yOffset, targetPosition.z);
        this.damage = damage;

        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        attackRoutine = StartCoroutine(StartAttackSequence());
    }

    private IEnumerator StartAttackSequence()
    {
        StartPreAttack();
        yield return new WaitForSeconds(1.5f);

        StartAttack();
        yield return new WaitForSeconds(3f);


        // Setelah selesai animasi, kembalikan ke pool
        PoolManager.Instance.TakeToPool(poolIndex, this);
    }

    private void StartPreAttack()
    {
        if (RockVisual != null)
            RockVisual.Play();
    }

    private void StartAttack()
    {
        GiveDamage();
    }

    private void GiveDamage()
    {
        var hitColliders = Physics.OverlapSphere(
            damageStartPoint.position,
            damageRange,
            damageLayer
        );

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<IDamageable>(out var target))
                target.TakeDamage(damage);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (damageStartPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(damageStartPoint.position, damageRange);
    }
#endif
}
