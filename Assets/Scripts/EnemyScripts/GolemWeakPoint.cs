using System.Collections;
using UnityEngine;
using Ami.BroAudio;


public class GolemWeakPoint : TakeDamageHandler
{
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private GameObject modelToHideOnDeath;
    [SerializeField] private Collider colliderToDisableOnDeath;
    [SerializeField, ReadOnly] private bool isAlreadyTakeDamage = false;
   

    public override void TakeDamage(float amount, Vector3 dmgImpactPos)
    {
        if (isAlreadyTakeDamage) return;
        isAlreadyTakeDamage = true;
        base.TakeDamage(amount, dmgImpactPos);

        StartCoroutine(PlayVFX());
    }
    
    public IEnumerator PlayVFX()
    {
        modelToHideOnDeath.SetActive(false);
        colliderToDisableOnDeath.enabled = false;

        if (hitEffect != null)
            hitEffect.Play();

        yield return new WaitForSeconds(hitEffect.main.duration);

        Destroy(gameObject);
    }
}
