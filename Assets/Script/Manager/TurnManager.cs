using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private PlayerState player1;
    [SerializeField] private PlayerState player2;

    public PlayerState currentPlayer;

    public void StartGame()
    {
        currentPlayer = player1;
        StartTurn();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            EndTurn();
        }
    }

    public void StartTurn()
    {
        Debug.Log($"{currentPlayer.PlayerName}의 턴 시작");
        CardSetting drawnCard = currentPlayer.Deck.DrawCard();
        if (drawnCard == null)
        {
            Debug.Log($"{currentPlayer.PlayerName}의 덱에 카드가 없습니다.");
            return;
        }
        currentPlayer.Hand.AddCard(drawnCard);
        currentPlayer.LightInitialize();// 빛 초기화(턴마다 하나씩 늘어나는거)


    }
    public void EndTurn()
    {
        if (currentPlayer == player1)
        {
            currentPlayer = player2;
        }
        else
        {
            currentPlayer = player1;
        }
        StartTurn();
    }

}
