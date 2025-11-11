using UnityEngine;

public class EnemyDamageHandler : MonoBehaviour
{
    private Enemy enemyComponent;
    void Awake()
    {
        enemyComponent = GetComponentInParent<Enemy>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player") return;
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(enemyComponent.AttackDamage);
        }
    }
}
