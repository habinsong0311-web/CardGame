using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private PlayerState player1;
    [SerializeField] private PlayerState player2;
    public PlayerState currentPlayer;

    [Header("연결")]
    [SerializeField] private CardPlayManager cardPlayManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private GameManager gameManager;
    [Header("AI")]
    [SerializeField] private PlayerState aiPlayer;
    [SerializeField] private SimpleAI simpleAI;


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
        if (gameManager.IsGameOver)
        {
            return;
        }
        Debug.Log($"{currentPlayer.PlayerName}의 턴 시작");
        CardSetting drawnCard = currentPlayer.Deck.DrawCard();
        if (drawnCard == null)
        {
            Debug.Log($"{currentPlayer.PlayerName}의 덱에 카드가 없어 패배했습니다.");
            gameManager.EndGame(currentPlayer);
            return;
        }
        currentPlayer.Hand.AddCard(drawnCard);
        currentPlayer.LightInitialize();// 빛 초기화(턴마다 하나씩 늘어나는거)
        currentPlayer.Field.ResetAllUnitsAttack();
        if (currentPlayer == aiPlayer)
        {
            simpleAI.StartAITurn();
        }

    }
    public void EndTurn()
    {
        if (gameManager.IsGameOver)
        {
            return;
        }
        battleManager.ClearSelection();
        cardPlayManager.ClearSelection();
        currentPlayer.Field.DisableAllUnitsAttack();
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
    public bool IsCurrentPlayer(PlayerState player)
    {
        //턴을 확인하는 함수
        return currentPlayer == player;
    }
    public PlayerState GetOpponent(PlayerState player)
    {// 누구의 턴인지 확인하는거
        if (player == player1)
        {
            return player2;
        }

        return player1;
    }

}
