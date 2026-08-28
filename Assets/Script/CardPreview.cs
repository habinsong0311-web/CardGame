using UnityEngine;
using UnityEngine.EventSystems;

public class CardPreview : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CardSetting cardData;
    private CardPreviewRoot cardPreviewRoot;
    public void Setup(CardSetting card, CardPreviewRoot preview)
    {
        cardData = card;
        cardPreviewRoot = preview;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cardPreviewRoot == null)
        {
            return;
        }
        cardPreviewRoot.Show(cardData);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (cardPreviewRoot == null)
        {
            return;
        }
        cardPreviewRoot.Hide();
    }
}
