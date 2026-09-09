using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class DeckEditorManager : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private TitleManager titleManager;

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
        List<CardSetting> sortedCards = new List<CardSetting>(ownedCards);//공간 만들기
        sortedCards.Sort((a, b) =>
        {//코스트 순으로 정렬
            int costResult = a.Cost.CompareTo(b.Cost);
            if (costResult != 0)
                return costResult;
            return string.Compare(a.CardName, b.CardName);
            //코스트가 같으면 이름순으로
        });
        foreach (CardSetting card in sortedCards)
        {
            if (card == null)
                continue;
            CardView cardView = null;
            //타입에 맞는 카드 생성
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
            cardView.Setup(card);//카드안에 데이터 넣기
            DeckEditorCard editorCard = cardView.gameObject.AddComponent<DeckEditorCard>();
            editorCard.Setup(card, this);//덱에 카드 전달
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
            Debug.Log($"덱에는 최대 {maxDeckSize}장까지 넣을 수 있습니다.");
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
            Debug.Log($"같은 카드는 최대 {maxSameCardCount}장까지 넣을 수 있습니다.");
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
        currentDeck.Remove(card);// 현재 덱에서 선택한 카드 한 장 제거
        UpdateDeckListItem(card);// 덱 목록에 표시되는 카드 수량 갱신
        SortDeckList();// 덱 목록을 카드 코스트 순서로 다시 정렬
        UpdateDeckCountText();// 현재 덱의 전체 카드 수 표시 갱신
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
        int deckIndex = DeckSelection.SelectedDeckIndex;

        return Path.Combine(Application.persistentDataPath,$"deck_{deckIndex}.json");
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
            cardData.cardId = cardCount.Key;// 카드 ID 저장
            cardData.count = cardCount.Value;// 카드 수량 저장
            saveData.cards.Add(cardData);// 덱 저장 목록에 추가
        }
        string json = JsonUtility.ToJson(saveData, true);// 저장 데이터를 JSON 문자열로 변환
        File.WriteAllText(GetDeckFilePath(),json);// 지정된 경로에 JSON 파일 저장
        titleManager.DeckSelectScene();
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
        currentDeck.Clear();// 기존 덱 데이터 초기화
        ClearDeckListItems();// 기존 덱 UI 목록 초기화
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
                    break;// 전체 덱이 30장이면 추가 중단
                currentDeck.Add(card);// 저장된 수량만큼 카드 복원
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