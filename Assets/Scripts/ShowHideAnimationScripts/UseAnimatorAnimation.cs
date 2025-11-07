using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UseAnimator : AnimationBase
{
    [SerializeField] private string showTrigger = "Show";
    [SerializeField] private string hideTrigger = "Hide";
    private Animator animator;

    private void Awake() => animator = GetComponent<Animator>();

    protected override void PlayShow()
    {
        if (animator != null && !string.IsNullOrEmpty(showTrigger))
            animator.SetTrigger(showTrigger);
    }

    protected override void PlayHide()
    {
        if (animator != null && !string.IsNullOrEmpty(hideTrigger))
            animator.SetTrigger(hideTrigger);
    }
}
