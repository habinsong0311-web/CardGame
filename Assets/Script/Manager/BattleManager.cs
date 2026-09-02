using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CardPlayManager cardPlayManager;
    private UnitBoardCardView selectedAttacker;
    public void SelectUnit(UnitBoardCardView unit)
    {
        if (gameManager.IsGameOver)
        {
            return;
        }
        if (unit == null)
        {
            return;
        }
        if (selectedAttacker == unit)
        {//다시한번 누르면 공격 취소
            ClearSelection();
            Debug.Log("공격자 선택을 취소했습니다.");
            return;
        }
        // 공격자가 아직 없으면 공격자로 선택
        if (selectedAttacker == null)
        {
            cardPlayManager.ClearSelection();
            SelectAttacker(unit);
            return;
        }
        // 공격자가 이미 있다면 대상으로 선택
        AttackTarget(unit);
    }

    private void SelectAttacker(UnitBoardCardView unit)
    {
        if (!turnManager.IsCurrentPlayer(unit.OwnerPlayer))
        {
            Debug.Log("현재 자신의 턴이 아닙니다.");
            return;
        }

        if (!unit.CanAttack)
        {
            Debug.Log("이 유닛은 현재 공격할 수 없습니다.");
            return;
        }
        selectedAttacker = unit;
        selectedAttacker.SetSelected(true);
        Debug.Log($"{unit.name}을 공격자로 선택했습니다.");
    }

    private void AttackTarget(UnitBoardCardView target)
    {
        if (selectedAttacker == null || target == null)
        {
            return;
        }
        if (selectedAttacker == target)
        {
            Debug.Log("자기 자신은 공격할 수 없습니다.");
            return;
        }
        if (selectedAttacker.OwnerPlayer == target.OwnerPlayer)
        {
            Debug.Log("아군 유닛은 공격할 수 없습니다.");
            return;
        }
        if (!selectedAttacker.CanAttack)
        {
            Debug.Log("이 유닛은 현재 공격할 수 없습니다.");
            ClearSelection();
            return;
        }
        if (target.OwnerPlayer.Field.HasTauntUnit() && !target.HasTaunt)
        {
            Debug.Log("도발 유닛을 먼저 공격해야 합니다.");
            return;
        }
        int attackerDamage = selectedAttacker.CurrentAttack;
        int targetDamage = target.CurrentAttack;
        string attackerName = selectedAttacker.name;
        string targetName = target.name;
        // 공격 기회를 먼저 사용합니다.
        selectedAttacker.UseAttack();
        // 서로 동시에 피해를 받습니다.
        target.TakeDamage(attackerDamage);
        selectedAttacker.TakeDamage(targetDamage);
        Debug.Log($"{attackerName}과 {targetName}이 서로 피해를 입었습니다.");
        ClearSelection();
    }
    public void AttackPlayer(PlayerState targetPlayer)
    {
        if (gameManager.IsGameOver)
        {
            return;
        }
        if (selectedAttacker == null || targetPlayer == null)
        {
            return;
        }
        if (selectedAttacker.OwnerPlayer == targetPlayer)
        {
            Debug.Log("자기 플레이어는 공격할 수 없습니다.");
            return;
        }
        if (targetPlayer.Field.HasAnyUnit())
        {
            Debug.Log("상대 필드에 유닛이 있어 플레이어를 공격할 수 없습니다.");
            return;
        }
        if (!selectedAttacker.CanAttack)
        {
            Debug.Log("이 유닛은 현재 공격할 수 없습니다.");
            ClearSelection();
            return;
        }
        targetPlayer.TakeDamage(selectedAttacker.CurrentAttack);
        selectedAttacker.UseAttack();

        Debug.Log($"{selectedAttacker.name}이 {targetPlayer.PlayerName}을 공격했습니다."
        );

        ClearSelection();
    }
    public void ClearSelection()
    {
        if (selectedAttacker != null)
        {
            selectedAttacker.SetSelected(false);
        }
        selectedAttacker = null;
    }

}
