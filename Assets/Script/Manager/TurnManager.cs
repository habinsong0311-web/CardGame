using UnityEngine;
using TMPro;
using System.Collections;

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
    [Header("턴 UI")]
    [SerializeField] private TMP_Text turnCountText;
    [SerializeField] private GameObject myTurnBanner;
    [SerializeField] private GameObject endTurnButton;
    [SerializeField] private GameObject opponentsTurn;
    private int turnCount;
    private Coroutine myTurnBannerCoroutine;
    [Header("AI")]
    [SerializeField] private PlayerState aiPlayer;
    [SerializeField] private SimpleAI simpleAI;



    public void StartGame()
    {
        currentPlayer = player1;
        StartTurn();
    }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            OnClickEndTurn();
        }
    }
#endif

    public void StartTurn()
    {
        if (gameManager.IsGameOver)
        {
            return;
        }
        UpdateTurnUI();
        Debug.Log($"{currentPlayer.PlayerName}의 턴 시작");
        CardSetting drawnCard = currentPlayer.Deck.DrawCard();
        gameManager.CheckDeckOut(currentPlayer, drawnCard);
        if (drawnCard == null)
        {
            Debug.Log($"{currentPlayer.PlayerName}의 덱에 카드가 없어 패배했습니다.");
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
        cardPlayManager.ForceClearSummonEffect();
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
    private void UpdateTurnUI()
    {
        bool isPlayerTurn = currentPlayer != aiPlayer;
        if (endTurnButton != null)
        {
            endTurnButton.SetActive(isPlayerTurn);
        }
        if (opponentsTurn != null)
        {
            opponentsTurn.SetActive(!isPlayerTurn);
        }
        // AI 턴이면 턴 수를 올리지 않음
        if (currentPlayer == aiPlayer)
        {
            if (myTurnBannerCoroutine != null)
            {
                StopCoroutine(myTurnBannerCoroutine);
                myTurnBannerCoroutine = null;
            }
            myTurnBanner.SetActive(false);
            return;
        }
        // 여기부터는 내 턴일 때만 실행
        turnCount++;

        if (turnCountText != null)
        {
            turnCountText.text = $"TURN : {turnCount}";
        }

        if (myTurnBannerCoroutine != null)
        {
            StopCoroutine(myTurnBannerCoroutine);
        }

        myTurnBannerCoroutine =StartCoroutine(ShowMyTurnBanner());
    }
    private IEnumerator ShowMyTurnBanner()
    {
        myTurnBanner.SetActive(true);
        yield return new WaitForSeconds(1f);
        myTurnBanner.SetActive(false);
        myTurnBannerCoroutine = null;
    }
    public void OnClickEndTurn()
    {
        if (gameManager.IsGameOver)
        {
            return;
        }
        if (currentPlayer == aiPlayer)
        {
            Debug.Log("상대 턴에는 턴을 종료할 수 없습니다.");
            return;
        }
        EndTurn();
    }
}
