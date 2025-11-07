using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimationController : MonoBehaviour, IDisableHandler
{
    [Header("Animation Control")]
    [SerializeField] private bool playOnEnable = false;
    [SerializeField] private bool disableAfterHide = true;

    [SerializeField] private List<AnimationBase> animations = new();

    private Coroutine showCoroutine;
    private Coroutine hideCoroutine;

    public float Duration
    {
        get
        {
            float longest = 0f;
            foreach (var anim in animations)
                if (anim.Duration > longest)
                    longest = anim.Duration;
            return longest;
        }
    }

    private void OnEnable()
    {
        if (playOnEnable)
            Show();
    }

    public void HandleDisable()
    {
        Hide();
    }

    public void Show()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        // Start fresh
        if (showCoroutine != null)
            StopCoroutine(showCoroutine);

        gameObject.SetActive(true);
        showCoroutine = StartCoroutine(ShowCoroutine());
    }

    private IEnumerator ShowCoroutine()
    {
        yield return null; // beri kesempatan GameObject untuk memanggil Awake/OnEnable child-nya

        foreach (var anim in animations)
            anim.TriggerShow();

        showCoroutine = null;
    }

    public void Hide()
    {
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
            showCoroutine = null;
        }

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        if (gameObject.activeInHierarchy) hideCoroutine = StartCoroutine(HideCoroutine());
    }

    private IEnumerator HideCoroutine()
    {
        foreach (var anim in animations)
            anim.TriggerHide();

        if (disableAfterHide)
        {
            yield return new WaitForSeconds(Duration);
            gameObject.SetActive(false);
        }

        hideCoroutine = null;
    }

    [ContextMenu("Assign Animation")]
    public void AssignAnimation()
    {
        animations.Clear();
        foreach (var anim in GetComponents<AnimationBase>())
        {
            if (!animations.Contains(anim))
                animations.Add(anim);
        }
    }
}
