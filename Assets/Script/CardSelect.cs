using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelect : MonoBehaviour,IPointerClickHandler
{
    private CardSetting cardData;
    private CardView cardView;
    private Hand ownerHand;
    private CardPlayManager cardPlayManager;
    public void Setup(CardSetting card,CardView view,Hand hand,CardPlayManager manager)
    {
        cardData = card;
        cardView = view;
        ownerHand = hand;
        cardPlayManager = manager;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardPlayManager == null)
        {
            return;
        }
        cardPlayManager.SelectCard(cardData, cardView, ownerHand);
    }
}
