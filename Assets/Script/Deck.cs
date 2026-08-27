using UnityEngine;
using System.Collections.Generic;
[System.Serializable] public class DeckEntry
{
    [SerializeField] private CardSetting card; //들어갈 카드
    [SerializeField, Range(1, 3)] private int count; // 카드 개수는 1~3
    public CardSetting Card => card;
    public int Count => count;
}
public class Deck : MonoBehaviour
{
    [Header("덱 구성")]
    [SerializeField] private List<DeckEntry> deckEntries = new List<DeckEntry>();
    private List<CardSetting> remainingCards = new List<CardSetting>();

    private void CreateDeck()
    {
        remainingCards.Clear();
        foreach (DeckEntry entry in deckEntries)
        {
            if (entry.Card == null)
            {
                continue;
            }
            for (int i = 0; i < entry.Count; i++)
            {
                remainingCards.Add(entry.Card);
            }

        }
    }
}

