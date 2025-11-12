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
    [Header("References")]
    public Image healthbarFillRight, healthbarFillLeft, damagebarFillRight, damagebarFillLeft;
    public TextMeshProUGUI healthText, comboText, totalHitText;
    public Image[] playerHealthImages = new Image[3];
    public Sprite[] bossIconSprites;
    [SerializeField] Animator anim;

    [Header("Inputs")]
    public float damageDelay, damageDuration;

    void OnEnable()
    {
        Events.OnEnemyHealthChanged.Add(OnEnemyHealthChanged);
        Events.OnPlayerAtkTotalHitCounted.Add(OnPlayerAtkTotalHitCounted);
        Events.OnPlayerAtkComboCounted.Add(OnPlayerAtkComboCounted);
        Events.OnPlayerAttackHitted.Add(OnPlayerAttackHitted);
        Events.OnPlayerAttackMissed.Add(OnPlayerAttackMissed);
        Events.OnPlayerHealthChanged.Add(OnPlayerHealthChanged);
    }

    void OnDisable()
    {
        Events.OnEnemyHealthChanged.Remove(OnEnemyHealthChanged);
        Events.OnPlayerAtkTotalHitCounted.Remove(OnPlayerAtkTotalHitCounted);
        Events.OnPlayerAtkComboCounted.Remove(OnPlayerAtkComboCounted);
        Events.OnPlayerAttackHitted.Remove(OnPlayerAttackHitted);
        Events.OnPlayerAttackMissed.Remove(OnPlayerAttackMissed);
        Events.OnPlayerHealthChanged.Remove(OnPlayerHealthChanged);
    }

    #region OnEvents
    public void OnEnemyHealthChanged(float currentHealth, float maxHealth)
    {
        float fillAmount = currentHealth / maxHealth;
        healthbarFillRight.fillAmount = fillAmount;
        healthbarFillLeft.fillAmount = fillAmount;
        healthText.text = $"{currentHealth}";
        StartCoroutine(UpdateDamageBarCoroutine(fillAmount, damageDelay, damageDuration));
    }

    public void OnPlayerAtkTotalHitCounted(int hitCount)
    {
        totalHitText.text = $"{hitCount}";
    }

    public void OnPlayerAtkComboCounted(int combo)
    {
        comboText.text = $"{combo}";
    }

    void OnPlayerAttackHitted()
    {
        anim.SetTrigger("Hit");
    }

    void OnPlayerAttackMissed()
    {
        anim.SetTrigger("Miss");
    }

    public void OnPlayerHealthChanged(float currentHealth, float maxHealth)
    {
        float healthPercentage = currentHealth / maxHealth;
        int healthBars;
        if (healthPercentage >= 0.8f) healthBars = 3;
        else if (healthPercentage >= 0.4f) healthBars = 2;
        else if (healthPercentage >= 0f) healthBars = 1;
        else healthBars = 0;

        switch (healthBars)
        {
            case 3:
                playerHealthImages[0].enabled = false;
                playerHealthImages[1].enabled = false;
                playerHealthImages[2].enabled = true;
                break;
            case 2:
                playerHealthImages[0].enabled = false;
                playerHealthImages[1].enabled = true;
                playerHealthImages[2].enabled = false;
                break;
            case 1:
                playerHealthImages[0].enabled = true;
                playerHealthImages[1].enabled = false;
                playerHealthImages[2].enabled = false;
                break;
            case 0:
                playerHealthImages[0].enabled = false;
                playerHealthImages[1].enabled = false;
                playerHealthImages[2].enabled = false;
                break;
            default:
                Debug.Log("Current health is not within range of 0-3");
                playerHealthImages[0].enabled = false;
                playerHealthImages[1].enabled = false;
                playerHealthImages[2].enabled = false;
                break;
        }
    }
    #endregion

    void Awake()
    {
        comboText.text = "0";
        totalHitText.text = "0";

        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
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


    [ContextMenu("Test Health Change")]
    public void testHealthChange()
    {
        OnEnemyHealthChanged(500f, 1000f);
    }

    [ContextMenu("Test Health Change2")]
    public void testmaxHealthChange()
    {
        OnEnemyHealthChanged(1000f, 1000f);
    }

    [ContextMenu("Test hit")]
    public void testHit()
    {
        OnPlayerAtkTotalHitCounted(Random.Range(0, 100));
    }
}
