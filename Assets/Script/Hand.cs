using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [Header("연결")]
    [SerializeField]private Graveyard graveyard;
    [SerializeField]private CardPlayManager cardPlayManager;
    [SerializeField] private PlayerState player;
    public PlayerState Player => player;
    [Header("카드 미리보기")]
    [SerializeField] private CardPreviewRoot cardPreviewRoot;
    [Header("손패 설정")]
    private int maxHandSize = 8;
    private List<CardSetting> cards = new List<CardSetting>();
    [Header("상대 패 설정")]
    [SerializeField] private bool hideCards;
    public int cardCount => cards.Count;

    public IReadOnlyList<CardSetting> Cards => cards;

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
    public void RemoveCard(CardSetting card,CardView cardView)
    {
        if (card == null || cardView == null)
        {
            return;
        }
        cards.Remove(card);
        Destroy(cardView.gameObject);
    }
    public CardView FindCardView(CardSetting card)
    {
        foreach (Transform child in transform)
        {
            CardView cardView = child.GetComponent<CardView>();

            if (cardView != null && cardView.CardSetting == card)
            {
                return cardView;
            }
        }
        return null;
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
        cardView.SetHidden(hideCards);
        CardPreview cardPreview = cardView.GetComponent<CardPreview>();
        if (cardPreview != null && !hideCards)
        {
            cardPreview.Setup(card, cardPreviewRoot);
        }
        CardSelect cardSelect = cardView.GetComponent<CardSelect>();
        if (cardSelect != null)
        {
            cardSelect.Setup(card,cardView,this,cardPlayManager);

        }
    }


}
