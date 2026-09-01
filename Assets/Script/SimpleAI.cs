using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    [SerializeField] private float actionDelay = 1f;

    private bool isPlaying;
    private void AddSummonActions(List<AIAction> actions)
    {
        int emptySlot = aiPlayer.Field.FindEmptySlot();
        if (emptySlot == -1)
            return;
        foreach (CardSetting card in aiPlayer.Hand.Cards)
        {
            if (card.CardType != CardType.Unit)
                continue;
            if (card.Cost > aiPlayer.CurrentLight)
                continue;
            CardView cardView = aiPlayer.Hand.FindCardView(card);
            if (cardView == null)
                continue;
            int score = card.Attack * 2 + card.MaxHealth - card.Cost;
            if (card.HasSummonEffect)
            {
                AddSummonEffectActions(actions, card, cardView, emptySlot, score);
                continue;
            }
            AddSummonAction(actions, card, cardView, emptySlot, score);
        }
    }
    private void AddSummonAction(List<AIAction> actions, CardSetting card, CardView cardView, int slotIndex, int score,
        UnitBoardCardView targetUnit = null, PlayerState targetPlayer = null)
    {
        AIAction action = new AIAction();
        action.ActionType = AIActionType.Summon;
        action.Score = score;
        action.Card = card;
        action.CardView = cardView;
        action.SlotIndex = slotIndex;
        action.TargetUnit = targetUnit;
        action.TargetPlayer = targetPlayer;
        actions.Add(action);
    }
    private void AddSummonEffectActions(List<AIAction> actions, CardSetting card, CardView cardView, int slotIndex, int score)
    {
        switch (card.CardId)
        {
            case "U006":
                AddDamageSummonActions(actions, card, cardView, slotIndex, score);
                break;

            case "U004":
                AddHealSummonActions(actions, card, cardView, slotIndex, score);
                break;
        }
    }
    private void AddDamageSummonActions(List<AIAction> actions, CardSetting card, CardView cardView, int slotIndex, int score)
    {//소환 능력 공격에 대한 점수계산 
        int playerScore = score + card.EffectValue * 5;
        if (card.EffectValue >= enemyPlayer.CurrentHealth)
        {
            playerScore += 1000000;
        }
        AddSummonAction(actions,card,cardView,slotIndex,playerScore,targetPlayer: enemyPlayer);
        foreach (UnitBoardCardView target in enemyPlayer.Field.Units)
        {
            if (target == null)
                continue;
            int unitScore = score + card.EffectValue * 3;
            unitScore += target.CurrentAttack * 4;
            if (card.EffectValue >= target.CurrentHealth)
            {
                unitScore += 100;
            }
            AddSummonAction(actions,card,cardView,slotIndex,unitScore,targetUnit: target);
        }
    }

    private void AddHealSummonActions(List<AIAction> actions, CardSetting card, CardView cardView, int slotIndex, int score)
    {//소환 능력 회복에대한 점수계산
        int playerScore = score + card.EffectValue * 3;
        if (aiPlayer.CurrentHealth <= 10)
        {
            playerScore += 100;
        }
        AddSummonAction(actions, card, cardView, slotIndex, playerScore, targetPlayer: aiPlayer);
        foreach (UnitBoardCardView target in aiPlayer.Field.Units)
        {
            if (target == null)
                continue;
            int unitScore = score + card.EffectValue * 3;
            unitScore += target.CurrentAttack * 2;
            if (target.CurrentHealth <= 3)
            {
                unitScore += 50;
            }
            AddSummonAction(actions, card, cardView, slotIndex, unitScore, targetUnit: target);
        }
    }
    private void AddSkillActions(List<AIAction> actions)
    {
        foreach (CardSetting card in aiPlayer.Hand.Cards)
        {
            if (card.CardType != CardType.Skill)
                continue;
            if (card.Cost > aiPlayer.CurrentLight)
                continue;
            CardView cardView = aiPlayer.Hand.FindCardView(card);
            if (cardView == null)
                continue;
            switch (card.CardId)
            {
                case "M002":
                    {
                        int playerScore = card.EffectValue * 5;
                        if (card.EffectValue >= enemyPlayer.CurrentHealth)
                        {
                            playerScore += 1000000;
                        }
                        AIAction playerAction = new AIAction();
                        playerAction.ActionType = AIActionType.UseSkillOnPlayer;
                        playerAction.Score = playerScore;
                        playerAction.Card = card;
                        playerAction.CardView = cardView;
                        playerAction.TargetPlayer = enemyPlayer;
                        actions.Add(playerAction);
                        foreach (UnitBoardCardView target in enemyPlayer.Field.Units)
                        {
                            if (target == null)
                                continue;
                            int unitScore = card.EffectValue * 3;
                            unitScore += target.CurrentAttack * 4;
                            if (card.EffectValue >= target.CurrentHealth)
                            {
                                unitScore += 100;
                            }
                            AIAction unitAction = new AIAction();
                            unitAction.ActionType = AIActionType.UseSkillOnUnit;
                            unitAction.Score = unitScore;
                            unitAction.Card = card;
                            unitAction.CardView = cardView;
                            unitAction.TargetUnit = target;
                            actions.Add(unitAction);
                        }
                        break;
                    }
                case "M001":
                    {
                        int playerScore = card.EffectValue * 3;
                        if (aiPlayer.CurrentHealth <= 10)
                        {
                            playerScore += 100;
                        }
                        AIAction playerAction = new AIAction();
                        playerAction.ActionType = AIActionType.UseSkillOnPlayer;
                        playerAction.Score = playerScore;
                        playerAction.Card = card;
                        playerAction.CardView = cardView;
                        playerAction.TargetPlayer = aiPlayer;
                        actions.Add(playerAction);
                        foreach (UnitBoardCardView target in aiPlayer.Field.Units)
                        {
                            if (target == null)
                            {
                                continue;
                            }
                            int unitScore = card.EffectValue * 3;
                            unitScore += target.CurrentAttack * 2;

                            if (target.CurrentHealth <= 3)
                            {
                                unitScore += 50;
                            }
                            AIAction unitAction = new AIAction();
                            unitAction.ActionType = AIActionType.UseSkillOnUnit;
                            unitAction.Score = unitScore;
                            unitAction.Card = card;
                            unitAction.CardView = cardView;
                            unitAction.TargetUnit = target;
                            actions.Add(unitAction);
                        }
                        break;
                    }
            }
        }
    }
    private void AddAttackActions(List<AIAction> actions)
    {
        bool hasTaunt = enemyPlayer.Field.HasTauntUnit();
        if (enemyPlayer.Field.HasAnyUnit())
        {
            foreach (UnitBoardCardView attacker in aiPlayer.Field.Units)
            {
                if (attacker == null || !attacker.CanAttack)
                    continue;
                foreach (UnitBoardCardView target in enemyPlayer.Field.Units)
                {
                    if (target == null)
                        continue;
                    if (hasTaunt && !target.HasTaunt)
                        continue;
                    int score = CalculateUnitAttackScore(attacker, target);
                    if (score == int.MinValue)
                        continue;
                    AIAction action = new AIAction();
                    action.ActionType = AIActionType.AttackUnit;
                    action.Score = score;
                    action.Attacker = attacker;
                    action.TargetUnit = target;
                    actions.Add(action);
                }
            }
            return;
        }
        foreach (UnitBoardCardView attacker in aiPlayer.Field.Units)
        {
            if (attacker == null || !attacker.CanAttack)
                continue;
            int score = attacker.CurrentAttack * 5;

            if (attacker.CurrentAttack >= enemyPlayer.CurrentHealth)
            {
                score += 1000000;
            }
            AIAction action = new AIAction();
            action.ActionType = AIActionType.AttackPlayer;
            action.Score = score;
            action.Attacker = attacker;
            action.TargetPlayer = enemyPlayer;
            actions.Add(action);
        }
    }
    private List<AIAction> CreatePossibleActions()
    {
        List<AIAction> actions = new List<AIAction>();

        AddSummonActions(actions);
        AddSkillActions(actions);
        AddAttackActions(actions);
        return actions;
    }
    private AIAction FindBestAction(List<AIAction> actions)
    {
        List<AIAction> bestActions = new List<AIAction>();
        int bestScore = int.MinValue;
        foreach (AIAction action in actions)
        {
            if (action.Score > bestScore)
            {//가장 높은점수가 나오면 기존걸 지우고 새롭게 넣음
                bestScore = action.Score;
                bestActions.Clear();
                bestActions.Add(action);
            }
            else if (action.Score == bestScore)
            {//기존 점수와 같은 면 추가
                bestActions.Add(action);
            }
        }
        if (bestActions.Count == 0)
            return null;
        int randomIndex = Random.Range(0, bestActions.Count);
        return bestActions[randomIndex];
        //골라진 점수들 중에 랜덤으로 하나 골라짐
        //여러게 면 랜덤 하나면 어차피 하나라 그게 골라짐
    }
    private void ExecuteAction(AIAction action)
    {
        if (action == null)
            return;

        switch (action.ActionType)
        {
            case AIActionType.Summon:
                cardPlayManager.SelectCard(action.Card, action.CardView, aiPlayer.Hand);
                cardPlayManager.TrySummon(aiPlayer, action.SlotIndex);
                if (action.TargetUnit != null)
                    cardPlayManager.TryUseSummonEffect(action.TargetUnit);
                else if (action.TargetPlayer != null)
                    cardPlayManager.TryUseSummonEffect(action.TargetPlayer);
                break;
            case AIActionType.UseSkillOnUnit:
                cardPlayManager.SelectCard(action.Card, action.CardView, aiPlayer.Hand);
                cardPlayManager.TryUseSkill(action.TargetUnit);
                break;
            case AIActionType.UseSkillOnPlayer:
                cardPlayManager.SelectCard(action.Card, action.CardView, aiPlayer.Hand);
                cardPlayManager.TryUseSkill(action.TargetPlayer);
                break;
            case AIActionType.AttackUnit:
                battleManager.SelectUnit(action.Attacker);
                battleManager.SelectUnit(action.TargetUnit);
                break;
            case AIActionType.AttackPlayer:
                battleManager.SelectUnit(action.Attacker);
                battleManager.AttackPlayer(action.TargetPlayer);
                break;
        }
        Debug.Log($"AI 행동: {action.ActionType}, 점수: {action.Score}");
    }
    private IEnumerator PlayTurn()
    {
        isPlaying = true;
        yield return new WaitForSeconds(actionDelay);

        while (!gameManager.IsGameOver)
        {
            List<AIAction> actions = CreatePossibleActions();
            AIAction bestAction = FindBestAction(actions);
            if (bestAction == null)
                break;
            ExecuteAction(bestAction);
            yield return new WaitForSeconds(actionDelay);
        }
        if (!gameManager.IsGameOver)
        {
            turnManager.EndTurn();
            Debug.Log("AI 턴 종료");
        }
        isPlaying = false;
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
    private int CalculateUnitAttackScore(UnitBoardCardView attacker, UnitBoardCardView target)
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
    public void StartAITurn()
    {
        if (isPlaying)
        {
            return;
        }
        Debug.Log("AI턴 시작");
        StartCoroutine(PlayTurn());
    }
}




//기존 만들었더 AI함수 추후 삭제

    //private IEnumerator PlayTurn()
    //{
    //    isPlaying = true;
    //    yield return new WaitForSeconds(actionDelay);
    //    while (TrySummonBestUnit())
    //    {
    //        yield return new WaitForSeconds(actionDelay);
    //    }
    //    while (TryUseBestSkill())
    //    {
    //        yield return new WaitForSeconds(actionDelay);
    //    }
    //    while (TryBestAttack())
    //    {
    //        yield return new WaitForSeconds(actionDelay);
    //    }
    //    if (!gameManager.IsGameOver)
    //    {
    //        turnManager.EndTurn();
    //        Debug.Log("AI턴 종료");
    //    }

//    isPlaying = false;
//

//    private bool TrySummonBestUnit()
//    {
//        CardSetting bestCard = null;
//        int bestScore = int.MinValue; // 카드 점수 보관

//        foreach (CardSetting card in aiPlayer.Hand.Cards)
//        {
//            if (card.CardType != CardType.Unit)
//            {
//                continue;
//            }
//            if (card.Cost > aiPlayer.CurrentLight)
//            {
//                continue;
//            }
//            int score = card.Attack * 2+ card.MaxHealth- card.Cost;
//            if (score > bestScore)
//            {
//                bestScore = score;
//                bestCard = card;
//            }
//        }
//        if (bestCard == null)
//        {
//            return false;
//        }
//        int emptySlot = aiPlayer.Field.FindEmptySlot();
//        if (emptySlot == -1)
//        {
//            return false;
//        }
//        CardView cardView =aiPlayer.Hand.FindCardView(bestCard);
//        if (cardView == null)
//        {
//            return false;
//        }
//        cardPlayManager.SelectCard(bestCard,cardView,aiPlayer.Hand);
//        cardPlayManager.TrySummon(aiPlayer,emptySlot);
//        Debug.Log($"AI가 {bestCard.CardName}을 소환했습니다. 점수: {bestScore}"
//        );
//        return true;
//    }

//    private bool TryBestAttack()
//    {
//        if (gameManager.IsGameOver)
//        {
//            return false;
//        }
//        UnitBoardCardView bestAttacker = null;
//        UnitBoardCardView bestTarget = null;
//        int bestScore = int.MinValue;
//        foreach (UnitBoardCardView attacker in aiPlayer.Field.Units)
//        {
//            if (attacker == null || !attacker.CanAttack)
//            {
//                continue;
//            }
//            foreach (UnitBoardCardView target in enemyPlayer.Field.Units)
//            {//AI몬스터 한마리 한마리 전부 각각 상대 몬스터와 대조

//                if (target == null)
//                {
//                    continue;
//                }
//                int score =CalculateUnitAttackScore(attacker,target); // 대조하고 점수를 확인
//                if (score > bestScore)
//                {//점수가 더 높은 놈을 선택
//                    bestScore = score;
//                    bestAttacker = attacker;
//                    bestTarget = target;
//                }
//            }
//        }
//        if (bestAttacker == null || bestTarget == null)
//        {
//            return TryAttackPlayer();
//        }
//        battleManager.SelectUnit(bestAttacker);
//        battleManager.SelectUnit(bestTarget);

//        Debug.Log($"AI가 유닛을 공격했습니다. 공격 점수: {bestScore}");
//        return true;
//    }
//    private bool TryAttackPlayer()
//    {
//        if (enemyPlayer.Field.HasAnyUnit())
//        {
//            return false;
//        }
//        UnitBoardCardView bestAttacker = null;
//        int bestScore = int.MinValue;
//        foreach (UnitBoardCardView attacker in aiPlayer.Field.Units)//상대 필드 검사
//        {
//            if (attacker == null || !attacker.CanAttack)
//            {
//                continue;
//            }
//            int score = attacker.CurrentAttack * 5;//5는 임의의값 공격력이 높을수록 직접공격을 함
//            if (attacker.CurrentAttack >= enemyPlayer.CurrentHealth)
//            {//승리할수있으면 무조건 이걸 하기위해서 점수를 높게줌
//                score += 1000000;
//            }
//            if (score > bestScore)
//            {
//                bestScore = score;
//                bestAttacker = attacker;
//            }
//        }
//        if (bestAttacker == null)
//        {
//            return false;
//        }
//        battleManager.SelectUnit(bestAttacker);
//        battleManager.AttackPlayer(enemyPlayer);
//        Debug.Log($"AI가 플레이어를 공격했습니다. 공격 점수: {bestScore}");
//        return true;
//    }

//    private bool TryUseBestSkill()
//    {
//        if (gameManager.IsGameOver)
//        {
//            return false;
//        }
//        //행동 보관
//        CardSetting bestCard = null;
//        UnitBoardCardView bestUnitTarget = null;
//        PlayerState bestPlayerTarget = null;
//        int bestScore = int.MinValue;


//        foreach (CardSetting card in aiPlayer.Hand.Cards)
//        {//패 확인으로 스킬만 확인
//            if (card.CardType != CardType.Skill)
//            {
//                continue;
//            }
//            if (card.Cost > aiPlayer.CurrentLight)
//            {//빛 부족 제외
//                continue;
//            }
//            switch (card.CardId)
//            {
//                case "M002": // 화염구 
//                    int playerDamageScore = card.EffectValue * 5;

//                    if (card.EffectValue >= enemyPlayer.CurrentHealth)
//                    {//명치시 승리할수있으면
//                        playerDamageScore += 1000000;
//                    }

//                    if (playerDamageScore > bestScore)
//                    {//플레이어 공격이 좋은지 몬스터 공격이 좋은지 판별
//                        bestScore = playerDamageScore;
//                        bestCard = card;
//                        bestPlayerTarget = enemyPlayer;
//                        bestUnitTarget = null;
//                    }
//                    foreach (UnitBoardCardView target in enemyPlayer.Field.Units)
//                    {
//                        if (target == null)
//                        {
//                            continue;
//                        }
//                        int unitDamageScore = card.EffectValue * 3;
//                        unitDamageScore += target.CurrentAttack * 4;
//                        if (card.EffectValue >= target.CurrentHealth)
//                        {
//                            unitDamageScore += 100;
//                        }
//                        if (unitDamageScore > bestScore)
//                        {
//                            bestScore = unitDamageScore;
//                            bestCard = card;
//                            bestUnitTarget = target;
//                            bestPlayerTarget = null;
//                        }
//                    }
//                    break;
//                case "M001":
//                    int playerHealScore = card.EffectValue * 3;

//                    if (aiPlayer.CurrentHealth <= 10)
//                    {
//                        playerHealScore += 100;
//                    }

//                    if (playerHealScore > bestScore)
//                    {
//                        bestScore = playerHealScore;
//                        bestCard = card;
//                        bestPlayerTarget = aiPlayer;
//                        bestUnitTarget = null;
//                    }
//                    foreach (UnitBoardCardView target in aiPlayer.Field.Units)
//                    {
//                        if (target == null)
//                        {
//                            continue;
//                        }
//                        int unitHealScore = card.EffectValue * 3;
//                        unitHealScore += target.CurrentAttack * 2;
//                        if (target.CurrentHealth <= 3)
//                        {
//                            unitHealScore += 50;
//                        }
//                        if (unitHealScore > bestScore)
//                        {
//                            bestScore = unitHealScore;
//                            bestCard = card;
//                            bestUnitTarget = target;
//                            bestPlayerTarget = null;
//                        }
//                    }
//                    break;
//            }
//        }
//        if (bestCard == null)
//        {//스킬카드가 없으면 끝
//            return false;
//        }
//        CardView cardView = aiPlayer.Hand.FindCardView(bestCard);
//        if (cardView == null)
//        {//선택한 카드를 찾고 없으면 취소(없으면 오류)
//            Debug.Log("대상을 찾기 못했습니다");
//            return false;
//        }
//        cardPlayManager.SelectCard(bestCard, cardView, aiPlayer.Hand);
//        if (bestUnitTarget != null)
//        {
//            cardPlayManager.TryUseSkill(bestUnitTarget);
//        }
//        else if (bestPlayerTarget != null)
//        {
//            cardPlayManager.TryUseSkill(bestPlayerTarget);
//        }
//        else
//        {
//            cardPlayManager.ClearSelection();
//            return false;
//        }
//        Debug.Log($"AI가 {bestCard.CardName}을 사용했습니다. 마법 점수: {bestScore}");
//        return true;
//    }
//}
