using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckListItem : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private TMP_Text nameText;
    private CardSetting cardData;
    private DeckEditorManager deckEditorManager;
    public CardSetting CardData => cardData;
    public void Setup(CardSetting card,int count,DeckEditorManager manager)
    {
        cardData = card;
        deckEditorManager = manager;
        nameText.text = $"{card.CardName} ×{count}";
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        deckEditorManager.RemoveCardFromDeck(cardData);
    }
    
}