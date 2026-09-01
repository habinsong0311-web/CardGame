using UnityEngine;

public class CardPlayManager : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private GameManager gameManager;


    private CardSetting selectedCard;
    private CardView selectedCardView;
    private Hand selectedHand;
    private CardSetting pendingSummonEffect;
    public bool IsSummonEffectTargeting => pendingSummonEffect != null;
    public void SelectCard(CardSetting card, CardView cardView, Hand hand)
    {
        if (gameManager.IsGameOver)
        {
            return;
        }
        if (card == null || cardView == null || hand == null)
        {
            return;
        }
        PlayerState Player = hand.Player;
        if (Player == null)
        {
            Debug.Log("패의 소유 플레이어가 연결되지 않았습니다.");
            return;
        }
        if (!turnManager.IsCurrentPlayer(Player))
        {
            Debug.Log("현재 자신의 턴이 아닙니다.");
            return;
        }
        if (Player.CurrentLight < card.Cost)
        {
            Debug.Log("빛이 부족해서 선택할 수 없습니다.");
            return;
        }

        selectedCard = card;
        selectedCardView = cardView;
        selectedHand = hand;
        if (selectedCard.CardType == CardType.Unit)
        {
            selectedHand.Player.Field.ShowAvailableSlots();
        }
        Debug.Log($"{selectedCard.CardName} 카드를 선택했습니다.");
    }
    public bool IsSkillSelected => selectedCard != null && selectedCard.CardType == CardType.Skill;
    public void TrySummon(PlayerState player, int slotIndex)
    {
        if (gameManager.IsGameOver)
        {
            return;
        }
        if (selectedCard == null)
        {
            Debug.Log("선택한 카드가 없습니다.");
            return;
        }
        if (!turnManager.IsCurrentPlayer(player))
        {
            Debug.Log("현재 자신의 턴이 아닙니다.");
            return;
        }
        if (selectedHand.Player != player)
        {
            Debug.Log("상대 필드에는 소환할 수 없습니다.");
            return;
        }
        if (selectedCard.CardType != CardType.Unit)
        {
            Debug.Log("유닛 카드만 필드에 소환할 수 있습니다.");
            return;
        }
        if (player.CurrentLight < selectedCard.Cost)
        {
            Debug.Log("빛이 부족합니다.");
            return;
        }
        if (!player.Field.IsSlotEmpty(slotIndex))
        {
            Debug.Log("이미 유닛이 있는 슬롯입니다.");
            return;
        }
        bool summonSuccess = player.Field.Summon(selectedCard, slotIndex);
        if (!summonSuccess)
        {
            return;
        }
        if (selectedCard.HasSummonEffect)
        {
            pendingSummonEffect = selectedCard;
            Debug.Log($"{selectedCard.CardName}의 효과를 적용할 대상을 선택하세요.");
        }
        player.SpendLight(selectedCard.Cost);
        selectedHand.RemoveCard(selectedCard, selectedCardView);
        ClearSelection();
    }
    public void ClearSelection()
    {
        if (selectedHand != null)
        {
            selectedHand.Player.Field.HideAllSlots();
        }
        selectedCard = null;
        selectedCardView = null;
        selectedHand = null;
    }
    public void TryUseSummonEffect(UnitBoardCardView target)
    {
        if (gameManager.IsGameOver)
            return;

        if (pendingSummonEffect == null || target == null)
            return;

        switch (pendingSummonEffect.CardId)
        {
            case "U006":
                target.TakeDamage(pendingSummonEffect.EffectValue);
                Debug.Log($"{target.name}에게 피해를 {pendingSummonEffect.EffectValue} 줬습니다.");
                break;

            case "U004":
                target.TakeHeal(pendingSummonEffect.EffectValue);
                Debug.Log($"{target.name}을 {pendingSummonEffect.EffectValue}만큼 회복했습니다.");
                break;

            default:
                Debug.Log($"등록되지 않은 소환 효과입니다: {pendingSummonEffect.CardId}");
                return;
        }

        ClearSummonEffect();
    }
    public void TryUseSummonEffect(PlayerState target)
    {
        if (gameManager.IsGameOver)
            return;
        if (pendingSummonEffect == null || target == null)
            return;
        switch (pendingSummonEffect.CardId)
        {
            case "U006":
                target.TakeDamage(pendingSummonEffect.EffectValue);
                Debug.Log($"{target.PlayerName}에게 피해를 {pendingSummonEffect.EffectValue} 줬습니다.");
                break;
            case "U004":
                target.TakeHeal(pendingSummonEffect.EffectValue);
                Debug.Log($"{target.PlayerName}을 {pendingSummonEffect.EffectValue}만큼 회복했습니다.");
                break;
            default:
                Debug.Log($"등록되지 않은 소환 효과입니다: {pendingSummonEffect.CardId}");
                return;
        }

        ClearSummonEffect();
    }
    public void TryUseSkill(UnitBoardCardView target)
    {//유닛에게 스킬을 사용하는 함수
        if (gameManager.IsGameOver)
            return;
        if (selectedCard == null || selectedHand == null || target == null)
            return;
        if (selectedCard.CardType != CardType.Skill)
        {
            Debug.Log("마법 카드가 아닙니다.");
            return;
        }
        PlayerState player = selectedHand.Player;
        if (!turnManager.IsCurrentPlayer(player))
        {
            Debug.Log("현재 자신의 턴이 아닙니다.");
            return;
        }
        if (player.CurrentLight < selectedCard.Cost)
        {
            Debug.Log("빛이 부족합니다.");
            return;
        }
        switch (selectedCard.CardId)
        {
            case "M002":
                target.TakeDamage(selectedCard.EffectValue);
                break;
            case "M001":
                target.TakeHeal(selectedCard.EffectValue);
                break;
            default:
                Debug.Log($"등록되지 않은 마법입니다: {selectedCard.CardId}");
                return;
        }
        player.SpendLight(selectedCard.Cost);
        selectedHand.RemoveCard(selectedCard,selectedCardView);
        player.Graveyard.AddCard(selectedCard);
        ClearSelection();
    }

    public void TryUseSkill(PlayerState target)
    {//영웅에게 스킬 사용함수
        if (gameManager.IsGameOver)
            return;
        if (selectedCard == null || selectedHand == null || target == null)
            return;
        if (selectedCard.CardType != CardType.Skill)
        {
            Debug.Log("마법 카드가 아닙니다.");
            return;
        }
        PlayerState player = selectedHand.Player;
        if (!turnManager.IsCurrentPlayer(player))
        {
            Debug.Log("현재 자신의 턴이 아닙니다.");
            return;
        }
        if (player.CurrentLight < selectedCard.Cost)
        {
            Debug.Log("빛이 부족합니다.");
            return;
        }
        switch (selectedCard.CardId)
        {
            case "M002":
                target.TakeDamage(selectedCard.EffectValue);
                break;
            case "M001":
                target.TakeHeal(selectedCard.EffectValue);
                Debug.Log($"{target.name}에게 치유의 빛을 사용했습니다.");
                break;
            default:
                Debug.Log($"등록되지 않은 마법입니다: {selectedCard.CardId}");
                return;
        }
        player.SpendLight(selectedCard.Cost);
        selectedHand.RemoveCard(selectedCard, selectedCardView);
        player.Graveyard.AddCard(selectedCard);
        ClearSelection();
    }
    private void ClearSummonEffect()
    {
        pendingSummonEffect = null;
    }
}
