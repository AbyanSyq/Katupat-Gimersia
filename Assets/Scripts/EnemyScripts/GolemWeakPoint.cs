using System.Collections;
using UnityEngine;
using Ami.BroAudio;
using DG.Tweening;


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
    public void disableWithVFX()
    {
        StartCoroutine(PlayVFX());
    }
    public void Init()
    {
        Debug.Log("init weak point");
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);

        transform.DOScale(Vector3.one, 0.5f).OnComplete(() => {
            transform.localScale = Vector3.one;
        }); 
    }
    
    public IEnumerator PlayVFX()
    {
        transform.DOKill();
        modelToHideOnDeath.SetActive(false);
        colliderToDisableOnDeath.enabled = false;

        if (hitEffect != null)
            hitEffect.Play();

        yield return new WaitForSeconds(hitEffect.main.duration);

        gameObject.SetActive(false);
    }
}
