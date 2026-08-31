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
    [Header("연결")]
    [SerializeField] private PlayerField field;
    [SerializeField] private GameManager gameManager;
    public PlayerField Field => field;

    public string PlayerName => playerName;
    public int CurrentHealth => currentHealth;
    public int CurrentLight => currentLight;

    public Deck Deck => deck;
    public Hand Hand => hand;
    public Graveyard Graveyard => graveyard;
    public bool IsAlive => currentHealth > 0;

    public void Initialize()
    {
        currentHealth = maxHealth;
        healthText.text = currentHealth.ToString();
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
        healthText.text = currentHealth.ToString();
        if (!IsAlive)
        {
            gameManager.EndGame(this);
        }
    }

}
