using System.Collections.Generic;
using TMPro;
using UnityEngine;
[System.Serializable] public class DeckEntry
{
    [SerializeField] private CardSetting card; //들어갈 카드
    [SerializeField, Range(1, 3)] private int count; // 카드 개수는 1~3
    public CardSetting Card => card;
    public int Count => count;
}
public class Deck : MonoBehaviour
{
    [SerializeField] private TMP_Text deckCountText;
    private int deckCount;
    [Header("덱 구성")]
    [SerializeField] private List<DeckEntry> deckEntries = new List<DeckEntry>(); // 저장된 덱
    private List<CardSetting> remainingCards = new List<CardSetting>(); // 플레이 덱

    //테스트
    private void Start()
    {
        CreateDeck();
    }

    //시작시 덱을 저장된 덱을 복사
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
        UpdateDeckCount();
    }
    public void UpdateDeckCount()
    {
        deckCount = remainingCards.Count;
        deckCountText.text = deckCount.ToString();
    }
}

