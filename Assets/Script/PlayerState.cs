using DamageNumbersPro;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [Header("플레이어 정보")]
    [SerializeField] private string playerName;
    [Header("체력")]
    [SerializeField, Min(1)]private int maxHealth = 30;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private int currentHealth;
    [Header("빛")]
    [SerializeField] private int maxLight = 0;
    private int currentLight;
    [SerializeField] private TMP_Text lightText;
    [Header("카드 연결")]
    [SerializeField] private Deck deck;
    [SerializeField] private Hand hand;
    [SerializeField] private Graveyard graveyard;
    [Header("피해 및 회복 UI")]
    [SerializeField] private DamageNumber damagePopupPrefab;
    [SerializeField] private DamageNumber healPopupPrefab;

    private RectTransform heroRect;
    private RectTransform popupRoot;
    [Header("연결")]
    [SerializeField] private PlayerField field;
    [SerializeField] private GameManager gameManager;
    public PlayerField Field => field;

    public string PlayerName => playerName;
    public int CurrentHealth => currentHealth;
    public int CurrentLight => currentLight;
    public RectTransform HeroRect => heroRect;

    public Deck Deck => deck;
    public Hand Hand => hand;
    public Graveyard Graveyard => graveyard;
    public bool IsAlive => currentHealth > 0;
    private Vector2 originalHeroPosition;
    private Vector3 originalHeroScale;

    public void Initialize()
    {
        heroRect = GetComponent<RectTransform>();
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            popupRoot = canvas.GetComponent<RectTransform>();
        }
        currentHealth = maxHealth;
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }
        else
        {
            Debug.LogWarning($"{name}: healthText가 연결되지 않았습니다.");
        }
        UpdateLightText();
    }
    private void UpdateLightText()
    {
        lightText.text = $"{currentLight}/{maxLight}";
    }
    public void PlusLight()
    {
        if (maxLight < 10)
        {
            maxLight++;
        }
    }
    public void SpendLight(int spendLight)
    {
        if(currentLight < spendLight)
        {
            Debug.Log("빛이 부족합니다");
            return;
        }
        currentLight -= spendLight;
        UpdateLightText();
    }
    public void SetLight()
    {
        currentLight = maxLight;
    }
    public void LightInitialize()
    {
        PlusLight();
        SetLight();
        UpdateLightText();
    }
    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }
        currentHealth -= damage;
        ShowDamageNumber(damage);
        healthText.text = currentHealth.ToString();
        gameManager.CheckDeathBy(this);
        PlayHitAnimation();
    }
    public void TakeHeal(int effectValue)
    {
        if (effectValue <= 0)
        {
            return;
        }
        currentHealth += effectValue;
        ShowHealNumber(effectValue);
        PlayHealAnimation();
        healthText.text = currentHealth.ToString();
    }
    private void ShowDamageNumber(int damage)
    {
        if (damagePopupPrefab == null || popupRoot == null || heroRect == null)
            return;
        damagePopupPrefab.SpawnGUI(popupRoot, heroRect, Vector2.zero, damage);
    }
    private void ShowHealNumber(int heal)
    {
        if (healPopupPrefab == null || popupRoot == null || heroRect == null)
            return;
        healPopupPrefab.SpawnGUI(popupRoot, heroRect, Vector2.zero, heal);
    }
    private void PlayHitAnimation()
    {
        if (heroRect == null)
        {
            return;
        }
        heroRect.DOShakeAnchorPos(0.25f,new Vector2(10f, 0f),10,90f);
    }
    private void PlayHealAnimation()
    {
        if (heroRect == null)
        {
            return;
        }
        heroRect.DOKill();
        heroRect.anchoredPosition = originalHeroPosition;
        heroRect.localScale = originalHeroScale;
        heroRect.DOPunchScale(new Vector3(0.15f, 0.15f, 0f), 0.35f, 6, 0.5f);
    }

}
