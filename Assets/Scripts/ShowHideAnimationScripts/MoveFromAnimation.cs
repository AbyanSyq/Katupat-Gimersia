using UnityEngine;
using DG.Tweening;

public class MoveFromAnimation : AnimationBase
{
    [SerializeField] private Transform hideTarget;
    [SerializeField] private Vector3 hideTargetPosition;
    [SerializeField] private Ease ease = Ease.OutQuad;
    [SerializeField, ReadOnly] private Vector3 originalPos;

    private Tween moveToTween;

    private void Awake()
    {
        originalPos = transform.localPosition;
    }

    protected override void PlayShow()
    {
        Vector3 targetPos = hideTarget != null
            ? hideTarget.localPosition
            : hideTargetPosition;

        moveToTween?.Kill();

        transform.localPosition = targetPos;
        moveToTween = transform.DOLocalMove(originalPos, duration)
            .SetEase(ease)
            .SetUpdate(runWhilePaused);
    }

    protected override void PlayHide()
    {
        Vector3 targetPos = hideTarget != null
            ? hideTarget.localPosition
            : hideTargetPosition;

        moveToTween?.Kill();
        moveToTween = transform.DOLocalMove(targetPos, duration)
            .SetEase(ease)
            .SetUpdate(runWhilePaused)
            .OnComplete(() =>
            {
                transform.localPosition = originalPos;
                gameObject.SetActive(false);
            });
    }
}
