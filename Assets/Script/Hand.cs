using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [Header("연결")]
    [SerializeField]private Graveyard graveyard;
    [Header("손패 설정")]
    private int maxHandSize = 8;
    private List<CardSetting> cards = new List<CardSetting>();
    public int cardCount => cards.Count;
    public void AddCard(CardSetting card)
    {
        if (card == null)
        {
            return;
            //나중에 패배 추가
        }
        cards.Add(card);
        DiscardExcessCards();
    }
    private void DiscardExcessCards()
    {
        while (cards.Count > maxHandSize)
        {
            int lastIndex = cards.Count - 1;
            CardSetting discardedCard = cards[lastIndex];

            cards.RemoveAt(lastIndex);
            graveyard.AddCard(discardedCard);
        }
    }


}
