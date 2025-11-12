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

    public TextMeshProUGUI healthText, comboText, totalHitText;

    public float damageDelay, damageDuration;

    public Image[] playerHealthImages = new Image[3];


    void OnEnable()
    {
        Events.OnEnemyHealthChanged.Add(UpdateHealthBar);
        Events.OnPlayerAtkHitCounted.Add(UpdateHitCounter);
        Events.OnPlayerAtkHitComboCounted.Add(UpdateComboCounter);
        // Events.OnPlayerAttackHitted.Add(UpdateCounter);
        // Events.OnPlayerAttackHittedCount.Add(UpdateCounter);
        // Events.OnPlayerAttackHittedCombo.Add(UpdateBestCombo);
        // Events.OnPlayerHealthChanged.Add(UpdatePlayerHealth);
    }

    void OnDisable()
    {
        Events.OnEnemyHealthChanged.Remove(UpdateHealthBar);
        Events.OnPlayerAtkHitCounted.Remove(UpdateHitCounter);
        Events.OnPlayerAtkHitComboCounted.Remove(UpdateComboCounter);
        // Events.OnPlayerAttackHittedCount.Remove(UpdateCounter);
        // Events.OnPlayerAttackHittedCombo.Remove(UpdateBestCombo);
        // Events.OnPlayerHealthChanged.Remove(UpdatePlayerHealth);
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

    public void UpdateHitCounter(int hitCount)
    {
        totalHitText.text = $"{hitCount}";
    }

    public void UpdateComboCounter(int bestCombo)
    {
        comboText.text = $"{bestCombo}";
    }

    public void UpdatePlayerHealth(float currentHealth, float maxHealth)
    {
        switch (currentHealth)
        {
            case 3f:
                playerHealthImages[0].enabled = false;
                playerHealthImages[1].enabled = false;
                playerHealthImages[2].enabled = true;
                break;
            case 2f:
                playerHealthImages[0].enabled = false;
                playerHealthImages[1].enabled = true;
                playerHealthImages[2].enabled = false;
                break;
            case 1f:
                playerHealthImages[0].enabled = true;
                playerHealthImages[1].enabled = false;
                playerHealthImages[2].enabled = false;
                break;
        }
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
        UpdateHitCounter(Random.Range(0, 100));
    }
}
