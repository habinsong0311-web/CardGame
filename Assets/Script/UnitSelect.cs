using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelect :MonoBehaviour, IPointerClickHandler
{
    private UnitBoardCardView unit;
    private BattleManager battleManager;
    public void Setup(UnitBoardCardView unitView,BattleManager manager)
    {
        unit = unitView;
        battleManager = manager;
    }
    public void OnPointerClick(
        PointerEventData eventData)
    {
        if (unit == null || battleManager == null)
        {
            return;
        }

        battleManager.SelectUnit(unit);
    }
}
