using UnityEngine;

public class CardPlayManager : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private GameManager gameManager;

    private CardSetting selectedCard;
    private CardView selectedCardView;
    private Hand selectedHand;

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
    public void TryUseSkill(UnitBoardCardView target)
    {
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
                Debug.Log($"{target.name}에게 화염구를 사용했습니다.");
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
        selectedHand.RemoveCard(selectedCard,selectedCardView);
        player.Graveyard.AddCard(selectedCard);
        ClearSelection();
    }
    public void TryUseSkill(PlayerState target)
    {
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
                Debug.Log($"{target.name}에게 화염구를 사용했습니다.");
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
}
