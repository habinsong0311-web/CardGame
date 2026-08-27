using System.Collections.Generic;
using UnityEngine;

public class Graveyard : MonoBehaviour
{
    private List<CardSetting> cards = new List<CardSetting>();
    public int cardCount => cards.Count;

    public void AddCard(CardSetting card)
    {
        if (card == null)
        {
            return;
        }
        cards.Add(card);
        Debug.Log($"{card.CardName} 카드가 묘지로 이동했습니다.");
    }
}

