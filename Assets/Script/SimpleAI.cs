using UnityEngine;
using System.Collections;

public class SimpleAI : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private PlayerState aiPlayer;
    [SerializeField] private PlayerState enemyPlayer;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CardPlayManager cardPlayManager;
    [SerializeField] private BattleManager battleManager;

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
        while (TrySummonBestUnit())
        {
            yield return new WaitForSeconds(actionDelay);
        }
        while (TryBestAttack())
        {
            yield return new WaitForSeconds(actionDelay);
        }
        if (!gameManager.IsGameOver)
        {
            turnManager.EndTurn();
            Debug.Log("AI턴 종료");
        }

        isPlaying = false;
    }
    private bool TrySummonBestUnit()
    {
        CardSetting bestCard = null;
        int bestScore = int.MinValue; // 카드 점수 보관

        foreach (CardSetting card in aiPlayer.Hand.Cards)
        {
            if (card.CardType != CardType.Unit)
            {
                continue;
            }
            if (card.Cost > aiPlayer.CurrentLight)
            {
                continue;
            }
            int score = card.Attack * 2+ card.MaxHealth- card.Cost;
            if (score > bestScore)
            {
                bestScore = score;
                bestCard = card;
            }
        }
        if (bestCard == null)
        {
            return false;
        }
        int emptySlot = aiPlayer.Field.FindEmptySlot();
        if (emptySlot == -1)
        {
            return false;
        }
        CardView cardView =aiPlayer.Hand.FindCardView(bestCard);
        if (cardView == null)
        {
            return false;
        }
        cardPlayManager.SelectCard(bestCard,cardView,aiPlayer.Hand);
        cardPlayManager.TrySummon(aiPlayer,emptySlot);
        Debug.Log($"AI가 {bestCard.CardName}을 소환했습니다. 점수: {bestScore}"
        );
        return true;
    }
    private int CalculateUnitAttackScore(UnitBoardCardView attacker,UnitBoardCardView target)
    {
        int score = 0;
        score += target.CurrentAttack * 4;
        score += target.CurrentHealth * 2;
        bool canKillTarget = attacker.CurrentAttack >= target.CurrentHealth;
        bool attackerSurvives = attacker.CurrentHealth > target.CurrentAttack;
        int totalAvailableAttack = GetTotalAvailableAttack();

        bool canDefeatTogether = totalAvailableAttack >= target.CurrentHealth;

        if (!canKillTarget && !attackerSurvives && !canDefeatTogether)
        {
            return int.MinValue;
        }
        if (canKillTarget)
        {
            score += 100;
        }
        if (attackerSurvives)
        {
            score += 40;
        }
        else
        {
            score -= attacker.CurrentAttack * 3;
            score -= attacker.CurrentHealth * 2;
        }
        return score;
    }
    private bool TryBestAttack()
    {
        if (gameManager.IsGameOver)
        {
            return false;
        }
        UnitBoardCardView bestAttacker = null;
        UnitBoardCardView bestTarget = null;
        int bestScore = int.MinValue;
        foreach (UnitBoardCardView attacker in aiPlayer.Field.Units)
        {
            if (attacker == null || !attacker.CanAttack)
            {
                continue;
            }
            foreach (UnitBoardCardView target in enemyPlayer.Field.Units)
            {//AI몬스터 한마리 한마리 전부 각각 상대 몬스터와 대조
                if (target == null)
                {
                    continue;
                }
                int score =CalculateUnitAttackScore(attacker,target); // 대조하고 점수를 확인
                if (score > bestScore)
                {//점수가 더 높은 놈을 선택
                    bestScore = score;
                    bestAttacker = attacker;
                    bestTarget = target;
                }
            }
        }
        if (bestAttacker == null || bestTarget == null)
        {
            return TryAttackPlayer();
        }
        battleManager.SelectUnit(bestAttacker);
        battleManager.SelectUnit(bestTarget);

        Debug.Log($"AI가 유닛을 공격했습니다. 공격 점수: {bestScore}");
        return true;
    }
    private bool TryAttackPlayer()
    {
        if (enemyPlayer.Field.HasAnyUnit())
        {
            return false;
        }
        UnitBoardCardView bestAttacker = null;
        int bestScore = int.MinValue;
        foreach (UnitBoardCardView attacker in aiPlayer.Field.Units)//상대 필드 검사
        {
            if (attacker == null || !attacker.CanAttack)
            {
                continue;
            }
            int score = attacker.CurrentAttack * 5;//5는 임의의값 공격력이 높을수록 직접공격을 함
            if (attacker.CurrentAttack >= enemyPlayer.CurrentHealth)
            {//승리할수있으면 무조건 이걸 하기위해서 점수를 높게줌
                score += 1000000;
            }
            if (score > bestScore)
            {
                bestScore = score;
                bestAttacker = attacker;
            }
        }
        if (bestAttacker == null)
        {
            return false;
        }
        battleManager.SelectUnit(bestAttacker);
        battleManager.AttackPlayer(enemyPlayer);
        Debug.Log($"AI가 플레이어를 공격했습니다. 공격 점수: {bestScore}");
        return true;
    }
    private int GetTotalAvailableAttack()
    {
        int totalAttack = 0;
        foreach (UnitBoardCardView unit in aiPlayer.Field.Units)
        {
            if (unit == null || !unit.CanAttack)
            {
                continue;
            }
            totalAttack += unit.CurrentAttack;
        }

        return totalAttack;
    }
}
