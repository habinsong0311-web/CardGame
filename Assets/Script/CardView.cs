using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [Header("카드 정보")]
    [SerializeField] private CardSetting cardSetting;
    public CardSetting CardSetting => cardSetting;
    [Header("카드 내용")]
    [SerializeField] private Image artworkImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text explanationText;
    [SerializeField] private TMP_Text keywordText;


    public void Setup(CardSetting cardData)
    {
        if (cardData == null)
        {
            return;
        }
        cardSetting = cardData;
        artworkImage.sprite = cardSetting.Artwork;
        nameText.text = cardSetting.CardName;
        costText.text = cardSetting.Cost.ToString();
        attackText.text = cardSetting.Attack.ToString();
        healthText.text = cardSetting.MaxHealth.ToString();
        explanationText.text = cardSetting.Explanation;
        keywordText.text = string.Join(", ", cardSetting.Keywords);

    }
}
