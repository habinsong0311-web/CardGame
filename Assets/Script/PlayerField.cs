using UnityEngine;

public class PlayerField : MonoBehaviour
{
    
    [Header("필드 슬롯")]
    [SerializeField] private Transform[] slots;
    [SerializeField] private FieldSlot[] fieldSlots;
    [Header("필드 유닛 프리팹")]
    [SerializeField] private UnitBoardCardView unitBoardPrefab;
    private UnitBoardCardView[] units;
    [Header("카드 미리보기")]
    [SerializeField] private CardPreviewRoot cardPreviewRoot;
    [Header("필드 소유자")]
    [SerializeField] private PlayerState ownerPlayer;
    [Header("연결")]
    [SerializeField] private BattleManager battleManager;
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
    public bool Summon(CardSetting card, int slotIndex)
    {
        if (card == null)
        {
            return false;
        }
        if (card.CardType != CardType.Unit)
        {
            Debug.Log("유닛 카드만 소환할 수 있습니다.");
            return false;
        }
        if (!IsSlotEmpty(slotIndex))
        {
            Debug.Log("선택한 슬롯이 비어 있지 않습니다.");
            return false;
        }
        UnitBoardCardView unit = Instantiate(unitBoardPrefab, slots[slotIndex]);
        unit.Setup(card, ownerPlayer);
        UnitSelect unitSelect = unit.GetComponent<UnitSelect>();
        if (unitSelect != null)
        {
            unitSelect.Setup(unit, battleManager);
        }
        units[slotIndex] = unit;

        CardPreview cardPreview =unit.GetComponent<CardPreview>();
        if (cardPreview != null)
        {
            cardPreview.Setup(card, cardPreviewRoot);
        }
        RectTransform unitRect = unit.GetComponent<RectTransform>();
        if (unitRect != null)
        {
            unitRect.anchoredPosition = Vector2.zero;
            unitRect.localRotation = Quaternion.identity;
            unitRect.localScale = Vector3.one;
        }
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
}
