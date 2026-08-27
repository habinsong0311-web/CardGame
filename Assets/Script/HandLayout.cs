using UnityEngine;

public class HandLayout : MonoBehaviour
{
    [Header("카드 배치")]
    [SerializeField] private float cardSpacing = 170f; // 카드 사이 간격
    [SerializeField] private float maxHandWidth = 900f; //폭
    [SerializeField] private float curveHeight = 45f; // 가장 높은 곳
    [SerializeField] private float maxRotation = 12f; //각도
    [Header("카드 크기")]
    [SerializeField] private float cardScale = 0.6f; // 카드 사이즈
    public void RefreshLayout()
    {
        int cardCount = transform.childCount;

        if (cardCount == 0)
        {
            return;
        }

        float spacing = cardSpacing;

        if (cardCount > 1)
        {
            spacing = Mathf.Min(
                cardSpacing,
                maxHandWidth / (cardCount - 1)
            );
        }
        float centerIndex = (cardCount - 1) / 2f;
        for (int i = 0; i < cardCount; i++)
        {
            RectTransform card =
                transform.GetChild(i) as RectTransform;
            if (card == null)
            {
                continue;
            }
            float distanceFromCenter = i - centerIndex;
            float normalizedPosition =
                centerIndex == 0? 0 : distanceFromCenter / centerIndex;
            float xPosition = distanceFromCenter * spacing;
            float yPosition =
                curveHeight *
                (1f - normalizedPosition * normalizedPosition);
            float rotation = -normalizedPosition * maxRotation;
            card.anchoredPosition = new Vector2(xPosition, yPosition);
            card.localRotation = Quaternion.Euler(0, 0, rotation);
            card.localScale = new Vector3(cardScale, cardScale, 1f);
        }
    }
    private void Update()
    {
        RefreshLayout();
    }
}


