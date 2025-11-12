using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIGameplay : UIBase
{
    [Header("References")]
    public Image healthbarFillRight, healthbarFillLeft, damagebarFillRight, damagebarFillLeft;
    public TextMeshProUGUI healthText, comboText, totalHitText;
    public TMP_Text hitInfoText;
    public Image[] playerHealthImages = new Image[3];
    public Sprite[] bossIconSprites;
    [SerializeField] Animator anim;
    [SerializeField] AnimationController bossInformationAnim;
    [SerializeField] private Slider playerChargeForceSlider;
    [SerializeField] GameObject bossHealthBar;

    [Header("Inputs")]
    public float damageDelay, damageDuration;

    [Header("Shake")]
    [SerializeField] float bossHealthBarShakeDuration;
    [SerializeField] float bossHealhBarShakeStrength;
    [SerializeField, ReadOnly] float lastBossHealth;

    void OnEnable()
    {
        Events.OnEnemyHealthChanged.Add(OnEnemyHealthChanged);
        Events.OnPlayerAtkTotalHitCounted.Add(OnPlayerAtkTotalHitCounted);
        Events.OnPlayerAtkComboCounted.Add(OnPlayerAtkComboCounted);
        Events.OnPlayerAttackHitted.Add(OnPlayerAttackHitted);
        Events.OnPlayerAttackMissed.Add(OnPlayerAttackMissed);
        Events.OnPlayerHealthChanged.Add(OnPlayerHealthChanged);
        Events.OnPlayerChargeForceChanged.Add(OnPlayerChargeForceChanged);

        StartCoroutine(ShowBossInformationCoroutine());
    }

    void OnDisable()
    {
        Events.OnEnemyHealthChanged.Remove(OnEnemyHealthChanged);
        Events.OnPlayerAtkTotalHitCounted.Remove(OnPlayerAtkTotalHitCounted);
        Events.OnPlayerAtkComboCounted.Remove(OnPlayerAtkComboCounted);
        Events.OnPlayerAttackHitted.Remove(OnPlayerAttackHitted);
        Events.OnPlayerAttackMissed.Remove(OnPlayerAttackMissed);
        Events.OnPlayerHealthChanged.Remove(OnPlayerHealthChanged);
        Events.OnPlayerChargeForceChanged.Remove(OnPlayerChargeForceChanged);

    }

    #region OnEvents
    public void OnEnemyHealthChanged(float currentHealth, float maxHealth)
    {
        float fillAmount = currentHealth / maxHealth;
        healthbarFillRight.fillAmount = fillAmount;
        healthbarFillLeft.fillAmount = fillAmount;

        healthText.text = $"{currentHealth}";

        StartCoroutine(UpdateDamageBarCoroutine(fillAmount, damageDelay, damageDuration));
    
        if(lastBossHealth - currentHealth <= 5f)
            bossHealthBar.transform.DOShakePosition(bossHealthBarShakeDuration, bossHealhBarShakeStrength * 0.5f);
        else
            bossHealthBar.transform.DOShakePosition(bossHealthBarShakeDuration, bossHealhBarShakeStrength);
        lastBossHealth = currentHealth;
    }
    public IEnumerator ShowBossInformationCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        if (GameManager.Instance.CurrentSceneType == SceneType.STAGE1) bossInformationAnim.Show();
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

        hitInfoText.transform.DOKill();
        hitInfoText.color = Color.white;
        hitInfoText.text = "Hit!";
        hitInfoText.gameObject.SetActive(true);
        hitInfoText.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.5f, 10, 1f).OnComplete(() =>
        {
            hitInfoText.gameObject.SetActive(false);
        });
    }

    void OnPlayerAttackMissed()
    {
        anim.SetTrigger("Miss");

        hitInfoText.transform.DOKill();
        hitInfoText.color = Color.red;
        hitInfoText.text = "Miss!";
        hitInfoText.gameObject.SetActive(true);
        hitInfoText.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.5f, 10, 1f).OnComplete(() =>
        {
            hitInfoText.gameObject.SetActive(false);
        });
    }
    void OnPlayerChargeForceChanged(float normalizedCharge)
    {
        playerChargeForceSlider.value = normalizedCharge;
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
