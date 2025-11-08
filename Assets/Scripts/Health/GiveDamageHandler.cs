using UnityEngine;

public class GiveDamageHandler : MonoBehaviour
{
    public float damageAmount = 10f;

    public void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount);
        }  
    }
}
