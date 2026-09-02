using DG.Tweening;
using UnityEngine;

public class SkillEffectView : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveDuration = 0.4f;
    [SerializeField] private float rotationOffset = 180f;
    [Header("크기 설정")]
    private RectTransform effectRect;
    private void Awake()
    {
        effectRect = GetComponent<RectTransform>();
    }
    public void Play(RectTransform start, RectTransform target, System.Action onHit)
        {
        if (effectRect == null || start == null || target == null)
            return;
        effectRect.position = start.position;
        Vector3 direction = target.position - start.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        effectRect.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
        effectRect.DOMove(target.position, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            onHit?.Invoke();
            Destroy(gameObject);
        });
    }
}