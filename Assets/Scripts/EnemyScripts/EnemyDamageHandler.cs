using UnityEngine;

public class EnemyDamageHandler : MonoBehaviour
{
    private Enemy enemyComponent;
    void Awake()
    {
        enemyComponent = FindAnyObjectByType<Enemy>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if(enemyComponent.IsDie) return;
        if(other.tag != "Player") return;

        Debug.Log(this.gameObject +  " give damage to player");
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(enemyComponent.AttackDamage, other.ClosestPoint(transform.position));
        }
    }
}
