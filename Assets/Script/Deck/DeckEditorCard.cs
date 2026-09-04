using UnityEngine;
using UnityEngine.EventSystems;

public class DeckEditorCard : MonoBehaviour,IPointerClickHandler
{
    private CardSetting cardData;
    private DeckEditorManager deckEditorManager;
    public void Setup(CardSetting card,DeckEditorManager manager)
    {
        cardData = card;
        deckEditorManager = manager;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardData == null || deckEditorManager == null)
            return;
        deckEditorManager.AddCardToDeck(cardData);
    }

}