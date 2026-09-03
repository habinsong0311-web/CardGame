using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        UpdateDeckCountText();
    }
    private void CreateCardCollection()
    {
        List<CardSetting> sortedCards = new List<CardSetting>(ownedCards);
        sortedCards.Sort((a, b) =>
        {
            int costResult =a.Cost.CompareTo(b.Cost);
            if (costResult != 0)
                return costResult;
            return string.Compare(a.CardName,b.CardName);
        });
        foreach (CardSetting card in sortedCards)
        {
            if (card == null)
                continue;
            CardView cardView = null;
            if (card.CardType == CardType.Unit)
            {
                cardView = Instantiate(unitCardPrefab,cardCollectionContent);
            }
            else if (card.CardType == CardType.Skill)
            {
                cardView = Instantiate(skillCardPrefab,cardCollectionContent);
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
        if (deckListItems.TryGetValue(card.CardId,out DeckListItem item))
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
            DeckListItem newItem = Instantiate(deckListItemPrefab,currentDeckContent);
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
            return string.Compare(a.CardData.CardName,b.CardData.CardName);
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
}