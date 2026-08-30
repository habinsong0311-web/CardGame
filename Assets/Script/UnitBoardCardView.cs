using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitBoardCardView : MonoBehaviour
{
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
        attackText.text = currentAttack.ToString();
        healthText.text = currentHealth.ToString();
    }
    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }
        currentHealth -= damage;
        healthText.text = currentHealth.ToString();
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void UseAttack()
    {
        canAttack = false;
    }
    public void ResetAttack()
    {
        canAttack = true;
    }
}
