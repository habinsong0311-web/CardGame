using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("플레이어 1")]
    [SerializeField] private PlayerState player1;
    [SerializeField] private Deck player1Deck;
    [SerializeField] private Hand player1Hand;
    [Header("플레이어 2")]
    [SerializeField] private PlayerState player2;
    [SerializeField] private Deck player2Deck;
    [SerializeField] private Hand player2Hand;

    [Header("매니저")]
    [SerializeField] private TurnManager turnManager;

    void Start()
    {
        player1Deck.InitializeDeck();
        player2Deck.InitializeDeck();
        player1.Initialize();
        player2.Initialize();
        for (int i = 0; i < 5; i++)
        {
            DrawPlayer1Card();
            DrawPlayer2Card();
        }
        turnManager.StartGame();
    }
    //테스트 드로우
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            DrawPlayer1Card();
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            DrawPlayer2Card();
        }
    }

    public void DrawPlayer1Card()
    {
        CardSetting drawnCard = player1Deck.DrawCard();
        if (drawnCard == null)
        {
            Debug.Log("카드를 뽑지 못했습니다.");
            return;
        }
        player1Hand.AddCard(drawnCard);
    }
    public void DrawPlayer2Card()
    {
        CardSetting drawnCard = player2Deck.DrawCard();
        if (drawnCard == null)
        {
            Debug.Log("카드를 뽑지 못했습니다.");
            return;
        }
        player2Hand.AddCard(drawnCard);
    }
}
