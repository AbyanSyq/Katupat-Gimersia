using UnityEngine;
using DG.Tweening;

public class TakeDamageHandler : MonoBehaviour, IDamageable
{
    public Health healthComponent;
    public float damageMultiplaier;
    public Renderer rendererComponent;
    public Color flashColor = Color.red;
    public void TakeDamage(float amount)
    {
        if (healthComponent != null)
        {
            healthComponent.ReduceHealth(amount * damageMultiplaier);
            Flash();
        }
    }
    public void Flash()
    {
        rendererComponent.material.DOKill();

        rendererComponent.material.DOColor(flashColor, 0.05f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);
    }
}
