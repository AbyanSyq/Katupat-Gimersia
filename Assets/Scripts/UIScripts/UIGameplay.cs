using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Reflection;
using System.Diagnostics.Tracing;

public class UIGameplay : UIBase
{
    public Image healthbarFillRight, healthbarFillLeft, damagebarFillRight, damagebarFillLeft;

    public TextMeshProUGUI healthText, bestHitText, hitText;

    public float damageDelay, damageDuration;

    public void PauseHandler()
    {
        UIManager.Instance.ChangeUI(UIType.PAUSEMENU);
    }

    void OnEnable()
    {
        Events.OnEnemyHealthChanged.Add(UpdateHealthBar);
        // Events.OnPlayerAttackHitted.Add(UpdateCounter);
        // Events.OnPlayerAttackHittedCount.Add(UpdateCounter);
        // Events.OnPlayerAttackHittedCombo.Add(UpdateBestCombo);
    }

    void OnDisable()
    {
        Events.OnEnemyHealthChanged.Remove(UpdateHealthBar);
        // Events.OnPlayerAttackHittedCount.Remove(UpdateCounter);
        // Events.OnPlayerAttackHittedCombo.Remove(UpdateBestCombo);
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float fillAmount = currentHealth / maxHealth;
        healthbarFillRight.fillAmount = fillAmount;
        healthbarFillLeft.fillAmount = fillAmount;
        healthText.text = $"{currentHealth}";
        StartCoroutine(UpdateDamageBarCoroutine(fillAmount, damageDelay, damageDuration));
    }

    public IEnumerator UpdateDamageBarCoroutine(float targetFillAmount, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        float initialFillRight = damagebarFillRight.fillAmount;
        float initialFillLeft = damagebarFillLeft.fillAmount;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float newFill = Mathf.Lerp(initialFillRight, targetFillAmount, t);
            damagebarFillRight.fillAmount = newFill;
            damagebarFillLeft.fillAmount = newFill;
            yield return null;
        }

        damagebarFillRight.fillAmount = targetFillAmount;
        damagebarFillLeft.fillAmount = targetFillAmount;
    }

    public void UpdateCounter(int hitCount)
    {
        hitText.text = $"{hitCount}";
    }

    public void UpdateBestCombo(int bestCombo)
    {
        bestHitText.text = $"{bestCombo}";
    }


    [ContextMenu("Test Health Change")]
    public void testHealthChange()
    {
        UpdateHealthBar(500f, 1000f);
    }

    [ContextMenu("Test Health Change2")]
    public void testmaxHealthChange()
    {
        UpdateHealthBar(1000f, 1000f);
    }

    [ContextMenu("Test hit")]
    public void testHit()
    {
        UpdateCounter(Random.Range(0, 100));
    }
}
