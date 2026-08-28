using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private TurnManager turnManager;
    private UnitBoardCardView selectedAttacker;
    public void SelectUnit(UnitBoardCardView unit)
    {
        if (unit == null)
        {
            return;
        }

        // 공격자가 아직 없으면 공격자로 선택
        if (selectedAttacker == null)
        {
            SelectAttacker(unit);
            return;
        }

        // 공격자가 이미 있다면 대상으로 선택
        AttackTarget(unit);
    }

    private void SelectAttacker(UnitBoardCardView unit)
    {
        selectedAttacker = unit;
        Debug.Log($"{unit.name}을 공격자로 선택했습니다.");
    }

    private void AttackTarget(UnitBoardCardView target)
    {
        Debug.Log($"{target.name}을 공격 대상으로 선택했습니다.");

        // 나중에 피해 계산 추가
    }

    public void ClearSelection()
    {
        selectedAttacker = null;
    }
}
