using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelect :MonoBehaviour, IPointerClickHandler
{
    private UnitBoardCardView unit;
    private BattleManager battleManager;
    private CardPlayManager cardPlayManager;
    public void Setup(UnitBoardCardView unitView, BattleManager battleManager, CardPlayManager cardPlayManager)
    {
        unit = unitView;
        this.battleManager = battleManager;
        this.cardPlayManager = cardPlayManager;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (unit == null || battleManager == null || cardPlayManager == null)
        {
            return;
        }

        if (cardPlayManager.IsSkillSelected)
        {
            cardPlayManager.TryUseSkill(unit);
            return;
        }

        battleManager.SelectUnit(unit);
    }
}
