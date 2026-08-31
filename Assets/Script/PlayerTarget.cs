using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerTarget : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private PlayerState player;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private CardPlayManager cardPlayManager;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (player == null || battleManager == null || cardPlayManager == null)
        {
            return;
        }

        if (cardPlayManager.IsSkillSelected)
        {
            cardPlayManager.TryUseSkill(player);
            return;
        }

        battleManager.AttackPlayer(player);
    }
}

