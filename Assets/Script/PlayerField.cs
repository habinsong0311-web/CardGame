using UnityEngine;
using System.Collections.Generic;

public class PlayerField : MonoBehaviour
{

    [Header("필드 슬롯")]
    [SerializeField] private Transform[] slots;
    [SerializeField] private FieldSlot[] fieldSlots;
    [Header("필드 유닛 프리팹")]
    [SerializeField] private UnitBoardCardView unitBoardPrefab;
    private UnitBoardCardView[] units;
    public IReadOnlyList<UnitBoardCardView> Units => units;
    [Header("카드 미리보기")]
    [SerializeField] private CardPreviewRoot cardPreviewRoot;
    [Header("필드 소유자")]
    [SerializeField] private PlayerState ownerPlayer;
    [Header("연결")]
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private CardPlayManager cardPlayManager;
    private void Awake()
    {
        units = new UnitBoardCardView[slots.Length];
    }
    public bool IsSlotEmpty(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= units.Length)
        {
            return false;
        }
        return units[slotIndex] == null;
    }
    public int FindEmptySlot()
    {// 비어있는 슬롯 찾기
        for (int i = 0; i < units.Length; i++)
        {
            if (IsSlotEmpty(i))
            {
                return i;
            }
        }
        return -1;
    }
    public bool Summon(CardSetting card, int slotIndex)
    {
        if (card == null)
        {
            return false;
        }//null값 확인
        if (card.CardType != CardType.Unit)
        {
            Debug.Log("유닛 카드만 소환할 수 있습니다.");
            return false;
        }//유닛카드인지 확인
        if (!IsSlotEmpty(slotIndex))
        {
            Debug.Log("선택한 슬롯이 비어 있지 않습니다.");
            return false;
        }//빈 슬롯인지 확인
        UnitBoardCardView unit = Instantiate(unitBoardPrefab, slots[slotIndex]);
        unit.Setup(card, ownerPlayer);// 정보값을 전달 및 생성
        UnitSelect unitSelect = unit.GetComponent<UnitSelect>();
        if (unitSelect != null)
        {
            unitSelect.Setup(unit, battleManager, cardPlayManager);
        }//카드 선택 기능에서 사용
        units[slotIndex] = unit;
        CardPreview cardPreview =unit.GetComponent<CardPreview>();
        if (cardPreview != null)
        {
            cardPreview.Setup(card, cardPreviewRoot);
        }//카드 미리보기 기능에서 사용
        RectTransform unitRect = unit.GetComponent<RectTransform>();
        if (unitRect != null)
        {
            unitRect.anchoredPosition = Vector2.zero;
            unitRect.localRotation = Quaternion.identity;
            unitRect.localScale = Vector3.one;
        }//카드가 소환될떄 사용
        return true;
    }
    public void ShowAvailableSlots()
    {
        for (int i = 0; i < fieldSlots.Length; i++)
        {
            if (IsSlotEmpty(i))
            {
                fieldSlots[i].ShowHighlight();
            }
            else
            {
                fieldSlots[i].HideHighlight();
            }
        }
    }
    public void HideAllSlots()
    {
        foreach (FieldSlot fieldSlot in fieldSlots)
        {
            fieldSlot.HideHighlight();
        }
    }
    public void ResetAllUnitsAttack()
    {
        foreach (UnitBoardCardView unit in units)
        {
            if (unit != null)
            {
                unit.ResetAttack();
            }
        }
    }
    public void DisableAllUnitsAttack()
    {
        foreach (UnitBoardCardView unit in units)
        {
            if (unit != null)
            {
                unit.UseAttack();
            }
        }
    }
    public bool HasAnyUnit()
    {
        //필드에 몬스터가 남아있늕 확인하는 함수
        //필드에 하나라도 몬스터가 존재하면 True
        foreach (UnitBoardCardView unit in units)
        {
            if (unit != null)
            {
                return true;
            }
        }
        return false;
    }
    public bool HasTauntUnit()
    {
        foreach (UnitBoardCardView unit in units)
        {
            if (unit != null && unit.HasTaunt)
            {
                return true;
            }
        }
        return false;
    }
    public void ShowAllUnitTargets()
    {
        foreach (UnitBoardCardView unit in units)
        {
            if (unit != null)
            {
                unit.SetTargetable(true);
            }
        }
    }
    public void HideAllUnitTargets()
    {
        foreach (UnitBoardCardView unit in units)
        {
            if (unit != null)
            {
                unit.SetTargetable(false);
            }
        }
    }
}
