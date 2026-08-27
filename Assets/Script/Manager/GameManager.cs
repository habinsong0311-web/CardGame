using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("플레이어 1")]
    [SerializeField] private Deck player1Deck;
    [SerializeField] private Hand player1Hand;
    [Header("플레이어 2")]
    [SerializeField] private Deck player2Deck;
    [SerializeField] private Hand player2Hand;

    void Start()
    {
        player1Deck.InitializeDeck();
        for (int i = 0; i < 5; i++)
        {
            DrawPlayer1Card();
        }
        
    }

    private void DrawPlayer1Card()
    {
        CardSetting drawnCard = player1Deck.DrawCard();
        if (drawnCard == null)
        {
            Debug.Log("카드를 뽑지 못했습니다.");
            return;
        }
        player1Hand.AddCard(drawnCard);
    }
}
