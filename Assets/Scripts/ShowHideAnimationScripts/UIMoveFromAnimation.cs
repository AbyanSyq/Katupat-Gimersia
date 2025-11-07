using UnityEngine;
using DG.Tweening;

public class UIMoveToAnimation : AnimationBase
{
    [SerializeField] private RectTransform hideTarget;
    [SerializeField] private Ease ease = Ease.OutQuad;

    private Vector3 originalPos;
    private Tween moveToTween;

    private void Awake() => originalPos = transform.localPosition;

    protected override void PlayShow()
    {
        if (hideTarget == null) return;
        moveToTween?.Kill();
        transform.localPosition = hideTarget.localPosition;
        moveToTween = transform.DOLocalMove(originalPos, duration).SetEase(ease).SetUpdate(runWhilePaused);
    }

    protected override void PlayHide()
    {
        if (hideTarget == null) return;
        moveToTween?.Kill();
        moveToTween = transform.DOLocalMove(hideTarget.localPosition, duration).SetEase(ease).SetUpdate(runWhilePaused);
    }
}
