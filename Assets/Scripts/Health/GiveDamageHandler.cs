using UnityEngine;

public class GiveDamageHandler : MonoBehaviour
{
    public float damageAmount = 10f;

    public void OnCollisionEnter(Collision collision)
    {
        
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount, collision.contacts[0].point);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount, other.ClosestPoint(transform.position));
        }
    }
}
