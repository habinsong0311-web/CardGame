using UnityEngine;

public class CardPreviewRoot : MonoBehaviour
{
    [Header("미리보기 카드")]
    [SerializeField] private CardView unitPreview;
    [SerializeField] private CardView skillPreview;

    public void Show(CardSetting card)
    {
        if (card == null)
        {
            return;
        }
        gameObject.SetActive(true);

        unitPreview.gameObject.SetActive(false);
        skillPreview.gameObject.SetActive(false);
        if (card.CardType == CardType.Unit)
        {
            unitPreview.gameObject.SetActive(true);
            unitPreview.Setup(card);
        }
        else if (card.CardType == CardType.Skill)
        {
            skillPreview.gameObject.SetActive(true);
            skillPreview.Setup(card);
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
