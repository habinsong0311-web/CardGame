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
    public void Setup(CardSetting cardData)
    {
        cardSetting = cardData;
        currentHealth = cardData.MaxHealth;
        currentAttack = cardData.Attack;
        artworkImage.sprite = cardData.Artwork;
        attackText.text = currentAttack.ToString();
        healthText.text = currentHealth.ToString();
    }
    void Start()
    {
        Setup(cardSetting);
    }
}
