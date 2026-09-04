using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class DeckEditorManager : MonoBehaviour
{
    [Header("현재 덱 화면")]
    [SerializeField] private Transform currentDeckContent;
    [SerializeField] private DeckListItem deckListItemPrefab;
    private Dictionary<string, DeckListItem> deckListItems = new Dictionary<string, DeckListItem>();


    [Header("카드 프리팹")]
    [SerializeField] private CardView unitCardPrefab;
    [SerializeField] private CardView skillCardPrefab;

    [Header("카드가 생성될 위치")]
    [SerializeField] private Transform cardCollectionContent;

    [Header("덱 설정")]
    [SerializeField] private int maxDeckSize = 30;
    [SerializeField] private int maxSameCardCount = 3;
    [SerializeField] private TMP_Text deckCountText;
    private List<CardSetting> ownedCards = new List<CardSetting>();
    private List<CardSetting> currentDeck = new List<CardSetting>();

    private void Start()
    {
        LoadOwnedCards();
        CreateCardCollection();
        LoadDeck();
        UpdateDeckCountText();
    }
    private void CreateCardCollection()
    {
        List<CardSetting> sortedCards = new List<CardSetting>(ownedCards);
        sortedCards.Sort((a, b) =>
        {
            int costResult = a.Cost.CompareTo(b.Cost);
            if (costResult != 0)
                return costResult;
            return string.Compare(a.CardName, b.CardName);
        });
        foreach (CardSetting card in sortedCards)
        {
            if (card == null)
                continue;
            CardView cardView = null;
            if (card.CardType == CardType.Unit)
            {
                cardView = Instantiate(unitCardPrefab, cardCollectionContent);
            }
            else if (card.CardType == CardType.Skill)
            {
                cardView = Instantiate(skillCardPrefab, cardCollectionContent);
            }
            if (cardView == null)
                continue;
            cardView.Setup(card);
            DeckEditorCard editorCard = cardView.gameObject.AddComponent<DeckEditorCard>();
            editorCard.Setup(card, this);
        }
    }
    public void AddCardToDeck(CardSetting card)
    {
        if (card == null)
        {
            return;
        }

        if (currentDeck.Count >= maxDeckSize)
        {
            Debug.Log("덱에는 최대 30장까지 넣을 수 있습니다.");
            return;
        }

        int sameCardCount = 0;

        foreach (CardSetting deckCard in currentDeck)
        {
            if (deckCard.CardId == card.CardId)
            {
                sameCardCount++;
            }
        }
        if (sameCardCount >= maxSameCardCount)
        {
            Debug.Log("같은 카드는 최대 3장까지 넣을 수 있습니다.");
            return;
        }
        currentDeck.Add(card);
        UpdateDeckListItem(card);
        SortDeckList();
        UpdateDeckCountText();
        Debug.Log($"{card.CardName} 추가, 현재 덱: {currentDeck.Count}/{maxDeckSize}");
    }
    private void UpdateDeckCountText()
    {
        if (deckCountText == null)
            return;
        deckCountText.text = $"{currentDeck.Count}/{maxDeckSize}";
    }
    public void RemoveCardFromDeck(CardSetting card)
    {
        if (card == null)
            return;
        if (!currentDeck.Contains(card))
            return;
        currentDeck.Remove(card);
        UpdateDeckListItem(card);
        SortDeckList();
        UpdateDeckCountText();
        Debug.Log($"{card.CardName} 제거, 현재 덱: {currentDeck.Count}/{maxDeckSize}");
    }
    private void UpdateDeckListItem(CardSetting card)
    {
        int count = 0;
        foreach (CardSetting deckCard in currentDeck)
        {
            if (deckCard.CardId == card.CardId)
            {
                count++;
            }
        }
        if (deckListItems.TryGetValue(card.CardId, out DeckListItem item))
        {
            if (count == 0)
            {
                Destroy(item.gameObject);
                deckListItems.Remove(card.CardId);
                return;
            }
            item.Setup(card, count, this);
            return;
        }
        if (count > 0)
        {
            DeckListItem newItem = Instantiate(deckListItemPrefab, currentDeckContent);
            newItem.Setup(card, count, this);
            deckListItems.Add(card.CardId, newItem);
        }
    }
    private void SortDeckList()
    {
        List<DeckListItem> items = new List<DeckListItem>(deckListItems.Values);
        items.Sort((a, b) =>
        {
            int costResult = a.CardData.Cost.CompareTo(b.CardData.Cost);
            if (costResult != 0)
                return costResult;
            return string.Compare(a.CardData.CardName, b.CardData.CardName);
        });
        for (int i = 0; i < items.Count; i++)
        {
            items[i].transform.SetSiblingIndex(i);
        }
    }
    private void LoadOwnedCards()
    {
        CardSetting[] loadedCards = Resources.LoadAll<CardSetting>("Cards");
        ownedCards = new List<CardSetting>(loadedCards);
    }
    private string GetDeckFilePath()
    {
        int deckIndex =
            DeckSelection.SelectedDeckIndex;

        return Path.Combine(
            Application.persistentDataPath,
            $"deck_{deckIndex}.json"
        );
    }
    public void SaveDeck()
    {
        if (currentDeck.Count != maxDeckSize)
        {
            Debug.Log($"덱은 정확히 {maxDeckSize}장이어야 합니다.");
            return;
        }

        DeckSaveData saveData = new DeckSaveData();
        saveData.deckIndex =DeckSelection.SelectedDeckIndex;
        saveData.deckName =$"{saveData.deckIndex}번 덱";
        Dictionary<string, int> cardCounts = new Dictionary<string, int>();
        foreach (CardSetting card in currentDeck)
        {
            if (cardCounts.ContainsKey(card.CardId))
            {
                cardCounts[card.CardId]++;
            }
            else
            {
                cardCounts.Add(card.CardId, 1);
            }
        }
        foreach (var cardCount in cardCounts)
        {
            DeckCardSaveData cardData = new DeckCardSaveData();
            cardData.cardId = cardCount.Key;
            cardData.count = cardCount.Value;
            saveData.cards.Add(cardData);
        }
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(GetDeckFilePath(),json);
        Debug.Log($"{saveData.deckIndex}번 덱을 저장했습니다.");
    }
    private CardSetting FindOwnedCard(string cardId)
    {
        foreach (CardSetting card in ownedCards)
        {
            if (card.CardId == cardId)
            {
                return card;
            }
        }
        return null;
    }
    private void ClearDeckListItems()
    {
        foreach (DeckListItem item in deckListItems.Values)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        deckListItems.Clear();
    }
    public void LoadDeck()
    {
        string filePath = GetDeckFilePath();
        if (!File.Exists(filePath))
        {
            Debug.Log($"{DeckSelection.SelectedDeckIndex}번 덱은 아직 저장되지 않았습니다.");
            return;
        }
        string json = File.ReadAllText(filePath);
        DeckSaveData saveData = JsonUtility.FromJson<DeckSaveData>(json);
        if (saveData == null || saveData.cards == null)
        {
            Debug.Log("덱 데이터를 불러올 수 없습니다.");
            return;
        }
        currentDeck.Clear();
        ClearDeckListItems();
        foreach (DeckCardSaveData savedCard in saveData.cards)
        {
            CardSetting card = FindOwnedCard(savedCard.cardId);
            if (card == null)
            {
                Debug.LogWarning($"카드를 찾을 수 없습니다: {savedCard.cardId}");
                continue;
            }
            int count = Mathf.Clamp(savedCard.count,0,maxSameCardCount);
            for (int i = 0; i < count; i++)
            {
                if (currentDeck.Count >= maxDeckSize)
                    break;
                currentDeck.Add(card);
            }
        }
        foreach (CardSetting card in currentDeck)   
        {
            if (!deckListItems.ContainsKey(card.CardId))
            {
                UpdateDeckListItem(card);
            }
        }
        SortDeckList();
        UpdateDeckCountText();
        Debug.Log($"{saveData.deckIndex}번 덱을 불러왔습니다.");
    }
}