using UnityEngine;
using UnityEngine.EventSystems;

public class FieldSlot : MonoBehaviour,IPointerClickHandler
{
    [Header("슬롯 정보")]
    [SerializeField] private PlayerState player;
    [SerializeField] private int slotIndex;
    [Header("연결")]
    [SerializeField] private CardPlayManager cardPlayManager;
    [SerializeField] private GameObject highlight;
    public void OnPointerClick(PointerEventData eventData)
    {
        cardPlayManager.TrySummon(player,slotIndex);
    }
    public void ShowHighlight()
    {
        highlight.SetActive(true);
    }
    public void HideHighlight()
    {
        highlight.SetActive(false);
    }
}