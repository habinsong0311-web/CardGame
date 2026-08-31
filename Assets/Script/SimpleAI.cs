using UnityEngine;
using System.Collections;

public class SimpleAI : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private PlayerState aiPlayer;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private GameManager gameManager;

    [Header("설정")]
    [SerializeField]private float actionDelay = 1f;

    private bool isPlaying;
    public void StartAITurn()
    {
        if (isPlaying)
        {
            return;
        }
        Debug.Log("AI턴 시작");
        StartCoroutine(PlayTurn());
    }
    private IEnumerator PlayTurn()
    {
        isPlaying = true;

        yield return new WaitForSeconds(actionDelay);

        // 나중에 AI 행동을 이곳에서 반복 실행

        if (!gameManager.IsGameOver)
        {
            turnManager.EndTurn();
            Debug.Log("AI턴 종료");
        }

        isPlaying = false;
    }
}
