using UnityEngine;
using DG.Tweening;

public class ScaleAnimation : AnimationBase
{
    [SerializeField] private Vector3 showScale = Vector3.one;
    [SerializeField] private Vector3 hiddenScale = Vector3.zero;
    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InBack;

    private Tween scaleTween;

    protected override void PlayShow()
    {
        scaleTween?.Kill();
        transform.localScale = hiddenScale;
        scaleTween = transform.DOScale(showScale, duration).SetEase(showEase).SetUpdate(runWhilePaused);
    }

    protected override void PlayHide()
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(hiddenScale, duration).SetEase(hideEase).SetUpdate(runWhilePaused);
    }
}
