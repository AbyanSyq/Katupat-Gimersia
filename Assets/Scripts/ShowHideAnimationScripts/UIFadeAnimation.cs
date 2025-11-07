using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class UIFadeAnimation : AnimationBase
{
    [SerializeField] private Ease ease = Ease.OutQuad;
    private CanvasGroup cg;
    private Tween fadeTween;

    private void Awake() => cg = GetComponent<CanvasGroup>();

    protected override void PlayShow()
    {
        fadeTween?.Kill();
        cg.alpha = 0f;
        fadeTween = cg.DOFade(1f, duration > 0 ? duration : 0.5f)
            .SetEase(ease)
            .SetUpdate(runWhilePaused);
    }

    protected override void PlayHide()
    {
        fadeTween?.Kill();
        fadeTween = cg.DOFade(0f, duration > 0 ? duration : 0.5f)
            .SetEase(ease)
            .SetUpdate(runWhilePaused);
    }
}
