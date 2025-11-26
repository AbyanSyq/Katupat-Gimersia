using Redcode.Pools;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectile : MonoBehaviour, IPoolObject
{
    [Header("Damage Settings")]
    [SerializeField] private LayerMask damageLayer;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 5f; // Durasi hidup peluru sebelum hilang

    private Rigidbody rb;

    // 1. Cache Rigidbody di Awake agar performa lebih baik (tidak dipanggil berulang kali)
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Pastikan gravitasi mati agar lurus
    }

    public void InitProjectile(Vector3 direction)
    {
        // 2. Pastikan Rigidbody sudah ada
        if (rb == null) rb = GetComponent<Rigidbody>();

        // 3. Set Velocity (Unity 6 menggunakan linearVelocity, Unity lama gunakan velocity)
        rb.linearVelocity = direction.normalized * speed; 
        
        // 4. Putar visual peluru agar menghadap ke arah tembakan
        transform.rotation = Quaternion.LookRotation(direction);

        // 5. Set timer untuk mengembalikan ke pool jika tidak mengenai apa-apa
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    // Logika ketika menabrak sesuatu
    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah object yang ditabrak ada di LayerMask yang ditentukan
        if (((1 << other.gameObject.layer) & damageLayer) != 0)
        {
            Debug.Log($"Hit {other.name} with {damage} damage");

            ReturnToPool();
        }
        else if (!other.isTrigger) 
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        PoolManager.Instance.TakeToPool<EnemyProjectile>(this);
    }

    public void OnCreatedInPool()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnGettingFromPool()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}