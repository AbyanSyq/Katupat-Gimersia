using UnityEngine;
using DG.Tweening;
using Ami.BroAudio;

public class TakeDamageHandler : MonoBehaviour, IDamageable
{
    public Health healthComponent;
    public float damageMultiplaier = 1f;
    public float damageReduction = 15f;
    public Renderer rendererComponent;
    public Color flashColor = Color.red;
    [SerializeField] private SoundID Damage_Sound; 

    public virtual void TakeDamage(float amount, Vector3 dmgImpactPos)
    {
        if (healthComponent != null)
        {
            Debug.Log($"Taking Damage: {amount * damageMultiplaier}");
            healthComponent.ReduceHealth(damageReduction);
            Flash();
            PlayDamageSound();
            
        }
    }
    public void Flash()
    {
        if (rendererComponent == null) return;
        rendererComponent.material.DOKill();

        rendererComponent.material.DOColor(flashColor, 0.05f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);
    }
        private void PlayDamageSound()
    {
        if (Damage_Sound.IsValid())
            BroAudio.Play(Damage_Sound);
    }
}
