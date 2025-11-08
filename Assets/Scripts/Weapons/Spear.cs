using UnityEngine;
using Redcode.Pools;

public class Spear : MonoBehaviour
{
    private Rigidbody rb;
    float timeToLive = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0f)
        {
            Despawn();
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Only rotate if moving
        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            // Align spear's UP axis (local up) with its velocity direction
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, rb.linearVelocity.normalized);
            rb.MoveRotation(targetRotation);
        }
    }
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            // // Stick the spear into the enemy
            // rb.isKinematic = true;
            // transform.SetParent(collision.transform);
            damageable.TakeDamage(10f);
            Despawn();
        }
    }

    void Despawn()
    {
        // Return to pool
        timeToLive = 5f;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        PoolManager.Instance.TakeToPool(0, this);
        
    }
}
