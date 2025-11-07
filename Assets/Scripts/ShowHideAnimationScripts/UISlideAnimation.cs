using UnityEngine;
using DG.Tweening;

public class UISlideAnimation : AnimationBase
{
    public enum Direction { Left, Right, Top, Bottom }

    [SerializeField] private Direction slideDirection = Direction.Left;
    [SerializeField] private float offsetX = 1920f;
    [SerializeField] private float offsetY = 1080f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    [SerializeField, ReadOnly] private Vector3 originalPos;
    private Tween slideTween;

    private void Awake()
    {
        originalPos = transform.localPosition;
            Debug.Log("Original Position: " + originalPos);
    }

    protected override void PlayShow()
    {
        slideTween?.Kill();
        Vector3 startPos = originalPos;

        switch (slideDirection)
        {
            case Direction.Left:   startPos.x -= offsetX; break;
            case Direction.Right:  startPos.x += offsetX; break;
            case Direction.Top:    startPos.y += offsetY; break;
            case Direction.Bottom: startPos.y -= offsetY; break;
        }

        transform.localPosition = startPos;
        Debug.Log(this.gameObject.name + " : Starting Slide Position: " + startPos);
        slideTween = transform.DOLocalMove(originalPos, duration).SetEase(ease).SetUpdate(runWhilePaused);
    }

    protected override void PlayHide()
    {
        slideTween?.Kill();
        Vector3 endPos = originalPos;

        switch (slideDirection)
        {
            case Direction.Left:   endPos.x -= offsetX; break;
            case Direction.Right:  endPos.x += offsetX; break;
            case Direction.Top:    endPos.y += offsetY; break;
            case Direction.Bottom: endPos.y -= offsetY; break;
        }

        slideTween = transform.DOLocalMove(endPos, duration).SetEase(ease).SetUpdate(runWhilePaused);
    }
}
