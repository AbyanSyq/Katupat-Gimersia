using UnityEngine;
using DG.Tweening;
public class UIMoveToAnimation : AnimationBase
{
    [SerializeField] private RectTransform hideTarget;
    [SerializeField] private Vector3 hideTargetPosition;
    [SerializeField] private Vector3 showTargetPosition;
    [SerializeField] private Ease ease = Ease.OutQuad;

    private Tween moveToTween;


    protected override void PlayShow()
    {
        Vector3 targetPos = hideTarget != null
            ? hideTarget.localPosition
            : hideTargetPosition;

        moveToTween?.Kill();
        transform.localPosition = targetPos;
        moveToTween = transform.DOLocalMove(showTargetPosition, duration).SetEase(ease).SetUpdate(runWhilePaused);
    }

    protected override void PlayHide()
    {
        if (hideTarget == null) return;
        moveToTween?.Kill();
        moveToTween = transform.DOLocalMove(hideTarget.localPosition, duration).SetEase(ease).SetUpdate(runWhilePaused);
    }
}
