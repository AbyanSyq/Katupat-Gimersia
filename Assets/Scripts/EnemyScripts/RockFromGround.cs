using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFromGround : MonoBehaviour
{
    [SerializeField] private Transform damageStartPoint;
    [SerializeField] private float damageRange = 2f;
    [SerializeField] private LayerMask damageLayer;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float yOffset = 0.6f;
    [Space]
    [SerializeField] private ParticleSystem RockVisual;
    [ContextMenu("Test Attack")]
    public void TestAttack()
    {
        initRock(new Vector3(0, 0, 10), damage);
    }
    public void initRock(Vector3 targetPosition, float damage)
    {
        gameObject.SetActive(true);
        transform.position = new Vector3(targetPosition.x, yOffset, targetPosition.z);
        this.damage = damage;

        StartCoroutine(StartAttackSequence());
    }
    public IEnumerator StartAttackSequence()
    {
        StartPreAttack();
        yield return new WaitForSeconds(1.5f);
        StartAttack();
        yield return new WaitForSeconds(3f);
        // RockVisual.Hide();
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
    public void StartPreAttack()
    {
        RockVisual.Play();
    }
    public void StartAttack()
    {
        GiveDamage();
    }
    public void GiveDamage()
    {
        var hitColliders = Physics.OverlapSphere(
            damageStartPoint.position,
            damageRange,
            damageLayer
        );

        foreach (var hitCollider in hitColliders)
        {
            var target = hitCollider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(damageStartPoint.position, damageRange);
    }
}
