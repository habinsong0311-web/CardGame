using UnityEngine;
using System.IO;

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

    [Header("게임 결과 UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    private bool isGameOver;
    public bool IsGameOver => isGameOver;
    void Start()
    {
        if (player1 == null || player2 == null || player1Deck == null || player2Deck == null ||
            player1Hand == null || player2Hand == null || turnManager == null)
        {
            Debug.LogError("GameManager: 필수 인스펙터 필드가 연결되지 않았습니다.");
            return;
        }
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GameManager: victoryPanel이 연결되지 않았습니다.");
        }
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GameManager: defeatPanel이 연결되지 않았습니다.");
        }
        int mainDeckIndex = LoadMainDeckIndex();

        if (mainDeckIndex == -1)
        {
            Debug.LogError("선택된 플레이어 덱이 없습니다.");
            return;
        }

        if (!player1Deck.InitializeSavedDeck(mainDeckIndex))
        {
            Debug.LogError("플레이어 덱을 불러오지 못했습니다.");
            return;
        }
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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
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
#endif

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
    public void EndGame(PlayerState loser)
    {
        if (isGameOver)
        {
            return;
        }
        isGameOver = true;
        PlayerState winner;
        if (loser == player1)
        {
            winner = player2;
        }
        else
        {
            winner = player1;
        }
        if (winner == player1)
        {
            defeatPanel.SetActive(false);
            victoryPanel.SetActive(true);
        }
        else if (winner == player2)
        {
            defeatPanel.SetActive(true);
            victoryPanel.SetActive(false);
        }
        Debug.Log($"{winner.PlayerName} 승리! " + $"{loser.PlayerName} 패배!");
    }
    public void CheckDeathBy(PlayerState player)
    {
        if (player != null && !player.IsAlive)
        {
            EndGame(player);
        }
    }
    public void CheckDeckOut(PlayerState player, CardSetting drawnCard)
    {
        if (player != null && drawnCard == null)
        {
            EndGame(player);
        }
    }
    private int LoadMainDeckIndex()
    {
        string filePath = Path.Combine(Application.persistentDataPath,"player_data.json");
        if (!File.Exists(filePath))
        {
            Debug.Log("플레이어 저장 파일이 없습니다.");
            return -1;
        }
        string json = File.ReadAllText(filePath);
        PlayerSaveData playerData = JsonUtility.FromJson<PlayerSaveData>(json);
        if (playerData == null)
        {
            return -1;
        }
        return playerData.mainDeckIndex;
    }
}
