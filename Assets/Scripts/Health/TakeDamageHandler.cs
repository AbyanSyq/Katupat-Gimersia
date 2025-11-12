using UnityEngine;
using DG.Tweening;

public class TakeDamageHandler : MonoBehaviour, IDamageable
{
    public Health healthComponent;
    public float damageMultiplaier = 1f;
    public Renderer rendererComponent;
    public Color flashColor = Color.red;
    public virtual void TakeDamage(float amount, Vector3 dmgImpactPos)
    {
        if (healthComponent != null)
        {
            Debug.Log($"Taking Damage: {amount * damageMultiplaier}");
            healthComponent.ReduceHealth(amount * damageMultiplaier);
            Flash();
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
}
