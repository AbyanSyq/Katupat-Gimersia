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
    public RectTransform crossHairDot;
    public RectTransform chargeBar;
    public TMP_Text hitInfoText;
    public GameObject[] playerHealthImages = new GameObject[3];
    public Sprite[] bossIconSprites;

    private bool isFullyCharged = false, isLowHealth = false;
    [SerializeField] Animator anim;
    [SerializeField] AnimationController bossInformationAnim;
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
        Events.OnPlayerAttackHitted.Add(OnPlayerAttackHitted);
        Events.OnPlayerAttackMissed.Add(OnPlayerAttackMissed);
        Events.OnPlayerHealthChanged.Add(OnPlayerHealthChanged);
        Events.OnPlayerChargeForceChanged.Add(OnPlayerChargeForceChanged);
        Events.OnThrowCooldownChanged.Add(OnPlayerThrowCooldownChanged);

        StartCoroutine(ShowBossInformationCoroutine());
    }

    void OnDisable()
    {
        Events.OnEnemyHealthChanged.Remove(OnEnemyHealthChanged);
        Events.OnPlayerAttackHitted.Remove(OnPlayerAttackHitted);
        Events.OnPlayerAttackMissed.Remove(OnPlayerAttackMissed);
        Events.OnPlayerHealthChanged.Remove(OnPlayerHealthChanged);
        Events.OnPlayerChargeForceChanged.Remove(OnPlayerChargeForceChanged);
        Events.OnThrowCooldownChanged.Remove(OnPlayerThrowCooldownChanged);

    }

    #region OnEvents
    public void OnPlayerThrowCooldownChanged(float normalizedCooldown)
    {
        Image im = chargeBar.GetComponent<Image>(); 
        im.fillAmount = 1 - normalizedCooldown;
    }
    public void OnEnemyHealthChanged(float currentHealth, float maxHealth)
    {
        float fillAmount = currentHealth / maxHealth;
        healthbarFillRight.fillAmount = fillAmount;
        healthbarFillLeft.fillAmount = fillAmount;

        StartCoroutine(UpdateDamageBarCoroutine(fillAmount, damageDelay, damageDuration));

        if (lastBossHealth - currentHealth <= 5f)
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

        chargeBar.localScale = new Vector3(1 - normalizedCharge, 1 - normalizedCharge, 1);
        if (normalizedCharge >= 1f)
        {
            if (!isFullyCharged) // <-- cuma trigger SEKALI
            {
                isFullyCharged = true;
                StartCoroutine(ShowFullyChargedCoroutine());
            }
        }
        else
        {
            // Reset agar bisa trigger lagi kalau charge turun lalu naik penuh kembali
            isFullyCharged = false;
        }

        if (normalizedCharge >= 1f)
        {
            isFullyCharged = true;
        }
        else
        {
            isFullyCharged = false;
        }
    }


    public void OnPlayerHealthChanged(float currentHealth, float maxHealth)
    {
        float healthPercentage = currentHealth / maxHealth;
        int healthBars;
        if (healthPercentage >= 0.8f) healthBars = 3;
        else if (healthPercentage >= 0.4f) healthBars = 2;
        else if (healthPercentage >= 0f) healthBars = 1;
        else healthBars = 0;

        if (healthBars == 1)
        {
            isLowHealth = true;
        }

        if (isLowHealth)
        {
            LowHealthHandler();
        }

        StartCoroutine(HealthDamageCoroutine(playerHealthImages[Mathf.Clamp(healthBars, 0, 2)].transform.Find("Health").gameObject, playerHealthImages[Mathf.Clamp(healthBars, 0, 2)].transform.Find("Damage").gameObject));

        if (healthBars > 0)
        {
            playerHealthImages[healthBars - 1].transform.Find("Icon").GetComponent<RectTransform>().DOScale(1.15f, 0.2f).SetEase(Ease.OutQuad).SetLoops(4, LoopType.Yoyo);
        }
    }
    #endregion

    void Awake()
    {
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

    public IEnumerator ShowFullyChargedCoroutine()
    {
        // Show some visual effect for fully charged (e.g., scale up and down)
        Tween scaleTween = crossHairDot.DOScale(3f, 0.05f).SetLoops(2, LoopType.Yoyo);
        yield return scaleTween.WaitForCompletion();
    }

    public IEnumerator HealthDamageCoroutine(GameObject healthBarObj, GameObject damageBarObj)
    {
        Image healthImg = healthBarObj.GetComponent<Image>();
        Image damageImg = damageBarObj.GetComponent<Image>();

        // 1. HEALTH langsung hilang (scale down atau alpha 0)
        healthImg.DOKill();
        healthImg.DOFade(0f, 0.1f);  // Health hilang cepat

        // 2. DAMAGE delay sedikit sebelum bergerak
        yield return new WaitForSeconds(0.15f);

        // 3. DAMAGE bergerak turun mengikuti health bar
        Vector3 startPos = damageBarObj.transform.localPosition;
        Vector3 targetPos = startPos + new Vector3(0, -15f, 0); // turun 15 pixel

        damageBarObj.transform.DOKill();
        damageBarObj.transform.DOLocalMove(targetPos, 0.25f).SetEase(Ease.OutQuad);

        // 4. DAMAGE fade-out perlahan
        damageImg.DOKill();
        damageImg.DOFade(0f, 0.35f).SetEase(Ease.OutQuad);

        // 5. Reset setelah selesai
        yield return new WaitForSeconds(0.4f);

        // Reset alpha & posisi agar siap damage berikutnya
        healthImg.color = new Color(healthImg.color.r, healthImg.color.g, healthImg.color.b, 1f);
        damageImg.color = new Color(damageImg.color.r, damageImg.color.g, damageImg.color.b, 1f);

        damageBarObj.transform.localPosition = startPos;

        healthBarObj.transform.parent.gameObject.SetActive(false);
    }

    public void LowHealthHandler()
    {
        playerHealthImages[0].transform.Find("Icon").GetComponent<RectTransform>().DOScale(1.15f, 0.5f).SetEase(Ease.OutQuad).SetLoops(-1, LoopType.Yoyo);
        playerHealthImages[0].transform.parent.GetComponent<Image>().DOColor(Color.red, 0.5f).SetLoops(-1, LoopType.Yoyo);
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
}
