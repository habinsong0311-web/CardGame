using UnityEngine;

public class HandLayout : MonoBehaviour
{
    private bool isOpened;
    [Header("메인 패")]
    [SerializeField] private RectTransform mainHandPoint;
    [Header("카드 배치")]
    [SerializeField] private float cardSpacing = 170f; // 카드 사이 간격
    [SerializeField] private float maxHandWidth = 900f; //폭
    [SerializeField] private float curveHeight = 45f; // 가장 높은 곳
    [SerializeField] private float maxRotation = 12f; //각도
    [Header("카드 크기")]
    [SerializeField] private float cardScale = 0.6f; // 카드 사이즈
    [Header("작은 패")]
    [SerializeField] private RectTransform sideHandPoint;
    [Header("카드 배치")]
    [SerializeField] private float sideCardSpacing = 100f; // 카드 사이 간격
    [SerializeField] private float sideMaxHandWidth = 500f; //폭
    [Header("카드 크기")]
    [SerializeField] private float sideCardScale = 0.4f; // 카드 사이즈
    [Header("패 열기 설정")]
    [SerializeField] private bool canOpenHand;


    public void SideHandLayout()
    {
        RectTransform handRect = transform as RectTransform;
        if (handRect == null || sideHandPoint == null)
        {
            return;
        }
        handRect.position = sideHandPoint.position;
        int cardCount = transform.childCount;
        if (cardCount == 0)
        {
            return;
        }
        float spacing = sideCardSpacing;
        if (cardCount > 1)
        {
            spacing = Mathf.Min(sideCardSpacing, sideMaxHandWidth / (cardCount - 1));
        }
        float centerIndex = (cardCount - 1) / 2f;
        for (int i = 0; i < cardCount; i++)
        {
            RectTransform card = transform.GetChild(i) as RectTransform;
            if (card == null)
            {
                continue;
            }
            float distanceFromCenter = i - centerIndex;
            float xPosition = distanceFromCenter * spacing;
            float yPosition = 0f;
            card.anchoredPosition = new Vector2(xPosition, yPosition);
            card.localRotation = Quaternion.Euler(0, 0, 0);
            card.localScale = new Vector3(sideCardScale, sideCardScale, 1f);
        }
    }

    public void RefreshLayout()
    {
        RectTransform handRect = transform as RectTransform;

        if (handRect == null || mainHandPoint == null)
        {
            return;
        }

        handRect.position = mainHandPoint.position;
        int cardCount = transform.childCount;
        if (cardCount == 0)
        {
            return;
        }
        float spacing = cardSpacing;
        if (cardCount > 1)
        {
            spacing = Mathf.Min(cardSpacing,maxHandWidth / (cardCount - 1));
        }
        float centerIndex = (cardCount - 1) / 2f;
        for (int i = 0; i < cardCount; i++)
        {
            RectTransform card = transform.GetChild(i) as RectTransform;
            if (card == null)
            {
                continue;
            }
            float distanceFromCenter = i - centerIndex;
            float normalizedPosition =centerIndex == 0? 0 : distanceFromCenter / centerIndex;
            float xPosition = distanceFromCenter * spacing;
            float yPosition = curveHeight *(1f - normalizedPosition * normalizedPosition);
            float rotation = -normalizedPosition * maxRotation;
            card.anchoredPosition = new Vector2(xPosition, yPosition);
            card.localRotation = Quaternion.Euler(0, 0, rotation);
            card.localScale = new Vector3(cardScale, cardScale, 1f);
        }
    }
    private void Update()
    {
        if(isOpened == true)
        {
            RefreshLayout();
        }
        if(!isOpened)
        {
            SideHandLayout();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if(canOpenHand)
                return;
            if(isOpened == true)
            {
                isOpened = false;
                return;
            }
            isOpened = true;
        }
    }
}


