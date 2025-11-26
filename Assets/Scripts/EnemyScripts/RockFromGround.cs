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
    public LayerMask groundLayer; // Assign ini di Inspector (pilih layer Ground/Terrain)
    public float rayStartHeight = 50f; // Seberapa tinggi ray dimulai dari atas

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
    // public void InitRock(Vector3 targetPosition, float damage)
    // {
    //     transform.position = new Vector3(targetPosition.x, yOffset, targetPosition.z);
    //     this.damage = damage;

    //     if (attackRoutine != null)
    //         StopCoroutine(attackRoutine);

    //     attackRoutine = StartCoroutine(StartAttackSequence());
    // }

    public void InitRock(Vector3 targetPosition, float damage)
    {
        // 1. Tentukan posisi awal ray (X dan Z sama dengan target, Y dari atas)
        Vector3 rayOrigin = new Vector3(targetPosition.x, rayStartHeight, targetPosition.z);
        
        float finalY = targetPosition.y; // Fallback jika raycast tidak kena apa-apa

        // 2. Tembakkan Ray ke bawah (Vector3.down)
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayStartHeight * 2, groundLayer))
        {
            // Jika kena tanah, ambil posisi Y titik tabrakannya
            finalY = hit.point.y;
        }

        // 3. Set posisi (tambahkan yOffset Anda jika ingin tetap ada offset dari permukaan tanah)
        transform.position = new Vector3(targetPosition.x, finalY + yOffset, targetPosition.z);
        
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
                target.TakeDamage(damage, transform.position);
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
