using UnityEngine;
using DG.Tweening;

public abstract class AnimationBase : MonoBehaviour
{

    [Header("Base Animation")]
    [SerializeField] protected bool runWhilePaused = true;
    [SerializeField] protected float duration = 0.5f;

    public float Duration { get => duration; }

    public void TriggerShow() => PlayShow();
    public void TriggerHide() => PlayHide();
    protected abstract void PlayShow();
    protected abstract void PlayHide();
}
