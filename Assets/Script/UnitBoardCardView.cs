using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitBoardCardView : MonoBehaviour
{
    [Header("능력치 색상")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color increasedColor = Color.green;
    [SerializeField] private Color decreasedColor = Color.red;
    [Header("카드 정보")]
    [SerializeField] private CardSetting cardSetting;
    [Header("카드 내용")]
    [SerializeField] private Image artworkImage;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text healthText;
    private int currentAttack;
    private int currentHealth;
    private bool canAttack;
    private PlayerState ownerPlayer;
    public PlayerState OwnerPlayer => ownerPlayer;
    public int CurrentAttack => currentAttack;
    public int CurrentHealth => currentHealth;
    public bool CanAttack => canAttack;
    public void Setup(CardSetting cardData , PlayerState player)
    {
        if (cardData == null || player == null)
        {
            return;
        }
        ownerPlayer = player;
        cardSetting = cardData;
        canAttack = false;
        currentHealth = cardData.MaxHealth;
        currentAttack = cardData.Attack;
        artworkImage.sprite = cardData.Artwork;
        UpdateStatText();
    }
    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }
        currentHealth -= damage;
        UpdateStatText();
        if (currentHealth <= 0)
        {
            ownerPlayer.Graveyard.AddCard(cardSetting);
            Destroy(gameObject);
        }
    }
    public void TakeHeal(int effectValue)
    {
        if(effectValue <= 0)
        {
            return;
        }
        currentHealth += effectValue;
        UpdateStatText();
    }
    public void UseAttack()
    {
        canAttack = false;
    }
    public void ResetAttack()
    {
        canAttack = true;
    }
    private Color GetStatColor(int currentValue,int originalValue)
    {
        if (currentValue > originalValue)
        {
            return increasedColor;
        }
        if (currentValue < originalValue)
        {
            return decreasedColor;
        }
        return normalColor;
    }
    private void UpdateStatText()
    {
        attackText.text = currentAttack.ToString();
        healthText.text = currentHealth.ToString();
        attackText.color = GetStatColor(currentAttack,cardSetting.Attack);
        healthText.color = GetStatColor(currentHealth,cardSetting.MaxHealth);
    }
}
