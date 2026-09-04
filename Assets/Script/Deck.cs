using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
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
    public CardSetting DrawCard()
    {
        if (remainingCards.Count <= 0)
        {
            Debug.Log("덱에 카드가 없습니다.");
            return null;
            //나중에 패배처리?
        }
        int lastIndex = remainingCards.Count - 1;
        CardSetting drawnCard = remainingCards[lastIndex];

        remainingCards.RemoveAt(lastIndex);
        UpdateDeckCount();
        return drawnCard;
    }
    public void InitializeDeck()
    {
        CreateDeck();
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        for (int i = remainingCards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CardSetting temporaryCard = remainingCards[i];
            remainingCards[i] = remainingCards[randomIndex];
            remainingCards[randomIndex] = temporaryCard;
        }
    }
    public bool InitializeSavedDeck(int deckIndex)
    {
        remainingCards.Clear();
        string filePath = Path.Combine(Application.persistentDataPath,$"deck_{deckIndex}.json");
        if (!File.Exists(filePath))
        {
            Debug.Log($"{deckIndex}번 덱 파일이 없습니다.");
            return false;
        }
        string json = File.ReadAllText(filePath);
        DeckSaveData saveData = JsonUtility.FromJson<DeckSaveData>(json);

        if (saveData == null || saveData.cards == null)
        {
            Debug.Log("덱 데이터를 불러올 수 없습니다.");
            return false;
        }
        CardSetting[] allCards =Resources.LoadAll<CardSetting>("Cards");
        foreach (DeckCardSaveData savedCard in saveData.cards)
        {
            CardSetting foundCard = null;

            foreach (CardSetting card in allCards)
            {
                if (card.CardId == savedCard.cardId)
                {
                    foundCard = card;
                    break;
                }
            }
            if (foundCard == null)
            {
                Debug.LogWarning($"카드를 찾을 수 없습니다: {savedCard.cardId}");
                continue;
            }
            for (int i = 0; i < savedCard.count; i++)
            {
                remainingCards.Add(foundCard);
            }
        }
        UpdateDeckCount();
        ShuffleDeck();
        return true;
    }
}

