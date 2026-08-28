using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [Header("연결")]
    [SerializeField]private Graveyard graveyard;
    [Header("카드 미리보기")]
    [SerializeField] private CardPreviewRoot cardPreviewRoot;
    [Header("손패 설정")]
    private int maxHandSize = 8;
    private List<CardSetting> cards = new List<CardSetting>();
    public int cardCount => cards.Count;

    [Header("카드 화면")]
    [SerializeField] private CardView unitCardPrefab;
    [SerializeField] private CardView skillCardPrefab;

    public void AddCard(CardSetting card)
    {
        if (card == null)
        {
            return;
            //나중에 패배 추가
        }
        if (cards.Count >= maxHandSize)
        {
            graveyard.AddCard(card);
            return;
        }
        cards.Add(card);
        CreateCardView(card);
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
    private void CreateCardView(CardSetting card)
    {
        CardView cardView = null;
        if (card.CardType == CardType.Unit)
        {
            cardView = Instantiate(unitCardPrefab, transform);
        }
        else if (card.CardType == CardType.Skill)
        {
            cardView = Instantiate(skillCardPrefab, transform);
        }
        if (cardView == null)
        {
            return;
        }
        cardView.Setup(card);
        CardPreview cardPreview = cardView.GetComponent<CardPreview>();
        if (cardPreview != null)
        {
            cardPreview.Setup(card, cardPreviewRoot);
        }


    }


}
