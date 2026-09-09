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
        CardSetting drawnCard = currentPlayer.Deck.DrawCard();
        gameManager.CheckDeckOut(currentPlayer, drawnCard);
        if (drawnCard == null)
        {
            return;
        }
        currentPlayer.Hand.AddCard(drawnCard);
        currentPlayer.LightInitialize();
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
        if (gameManager.IsResolvingAction)
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
    {
        if (player == player1)
        {
            return player2;
        }
        if (player == player2)
        {
            return player1;
        }
        Debug.LogWarning("등록되지 않은 플레이어입니다.");
        return null;
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
            return;
        if (gameManager.IsResolvingAction)
            return;
        if (currentPlayer == aiPlayer)
            return;
        EndTurn();
    }
}
