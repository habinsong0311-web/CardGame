using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DamageNumbersPro;
using DG.Tweening;

public class UnitBoardCardView : MonoBehaviour
{
    [Header("공격 가능 표시")]
    [SerializeField] private Image unitFrame;
    [SerializeField] private Color normalFrameColor = Color.white;
    [SerializeField] private Color attackReadyColor = Color.green;
    [SerializeField] private Color selectedFrameColor = Color.yellow;
    [SerializeField] private Color targetableColor = Color.cyan;
    [Header("능력치 색상")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color increasedColor = Color.green;
    [SerializeField] private Color decreasedColor = Color.red;
    [Header("카드 정보")]
    [SerializeField] private CardSetting cardSetting;
    [Header("카드 내용")]
    [SerializeField] private Image artworkImage;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject tauntFrame;
    [Header("피해 및 회복 UI")]
    [SerializeField] private DamageNumber damagePopupPrefab;
    [SerializeField] private DamageNumber healPopupPrefab;
    private RectTransform unitRect;
    private RectTransform popupRoot;
    [Header("공격 애니메이션")]
    [SerializeField] private float attackStopDistance = 50f;
    [SerializeField] private float attackMoveDuration = 0.12f;
    [Header("피격 애니메이션")]
    [SerializeField] private float hitDuration = 0.25f; // 흔들리는 시간
    [SerializeField] private Vector2 hitStrength = new Vector2(10f, 0f); //흔들리는 세기 좌우
    [SerializeField] private int hitVibrato = 10;//횟수
    [Header("사망 애니메이션")]
    [SerializeField] private float deathDuration = 0.3f;

    private int currentAttack;
    private int currentHealth;
    private bool canAttack;
    private bool isSelected;
    private bool isTargetable;
    private PlayerState ownerPlayer;
    private bool isAttacking;
    public PlayerState OwnerPlayer => ownerPlayer;
    public int CurrentAttack => currentAttack;
    public int CurrentHealth => currentHealth;
    public bool CanAttack => canAttack;
    public RectTransform UnitRect => unitRect;
    public bool HasTaunt => cardSetting != null && cardSetting.HasKeyword(KeyWord.도발);
    public void Setup(CardSetting cardData , PlayerState player)
    {
        if (cardData == null || player == null)
        {
            return;
        }
        unitRect = GetComponent<RectTransform>();
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            popupRoot = canvas.GetComponent<RectTransform>();
        }
        ownerPlayer = player;
        cardSetting = cardData;
        if (tauntFrame != null)
        {
            tauntFrame.SetActive(cardData.HasKeyword(KeyWord.도발));
        }//도발이있으면 것에 프레임 보임
        canAttack = cardData.HasKeyword(KeyWord.돌진);//돌진 키워드가 있으면 바로 공격할수있음
        UpdateFrameColor();
        currentHealth = cardData.MaxHealth;
        currentAttack = cardData.Attack;
        artworkImage.sprite = cardData.Artwork;
        UpdateStatText();
    }
    private void UpdateFrameColor()
    {
        if (unitFrame == null)
            return;

        if (isSelected)
            unitFrame.color = selectedFrameColor;
        else if (isTargetable)
        {
            unitFrame.color = targetableColor;
        }
        else if (canAttack)
            unitFrame.color = attackReadyColor;
        else
            unitFrame.color = normalFrameColor;
    }
    public void SetTargetable(bool targetable)
    {
        isTargetable = targetable;
        UpdateFrameColor();
    }
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateFrameColor();
    }
    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }
        currentHealth -= damage;
        ShowDamageNumber(damage);
        UpdateStatText();
        if (currentHealth <= 0)
        {
            ownerPlayer.Graveyard.AddCard(cardSetting);
            PlayDeathAnimation();
        }
        PlayHitAnimation();
    }
    public void TakeHeal(int effectValue)
    {
        if(effectValue <= 0)
        {
            return;
        }
        currentHealth += effectValue;
        ShowHealNumber(effectValue);
        PlayHealAnimation();
        UpdateStatText();
    }
    public void UseAttack()
    {
        canAttack = false;
        UpdateFrameColor();
    }
    public void ResetAttack()
    {
        canAttack = true;
        UpdateFrameColor();
    }
    private Color GetStatColor(int currentValue,int originalValue)
    {
        if (currentValue > originalValue)
        {
            return increasedColor;
        }
        if (currentValue < originalValue)
        {
            return decreasedColor;
        }
        return normalColor;
    }
    private void UpdateStatText()
    {
        attackText.text = currentAttack.ToString();
        healthText.text = currentHealth.ToString();
        attackText.color = GetStatColor(currentAttack,cardSetting.Attack);
        healthText.color = GetStatColor(currentHealth,cardSetting.MaxHealth);
    }
    private void ShowDamageNumber(int damage)
    {
        if (damagePopupPrefab == null || popupRoot == null || unitRect == null)
            return;
        damagePopupPrefab.SpawnGUI(popupRoot,unitRect,Vector2.zero,damage);
    }
    private void ShowHealNumber(int heal)
    {
        if (healPopupPrefab == null || popupRoot == null || unitRect == null)
            return;
        healPopupPrefab.SpawnGUI(popupRoot,unitRect,Vector2.zero,heal);
    }
    private void PlayHitAnimation()
    {
        if (unitRect == null || isAttacking)
        {
            return;
        }

        unitRect.DOShakeAnchorPos(hitDuration,hitStrength,hitVibrato);
    }
    public void PlaySummonAnimation()
    {
        transform.localScale = Vector3.zero;

        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    private void PlayHealAnimation()
    {
        if (unitRect == null)
        {
            return;
        }
        unitRect.DOKill();
        unitRect.anchoredPosition = Vector2.zero;
        unitRect.localScale = Vector3.one;
        unitRect.DOPunchScale(new Vector3(0.15f, 0.15f, 0f),0.35f,6,0.5f);
    }
    private void PlayDeathAnimation()
    {
        transform.DOScale(Vector3.zero, deathDuration).OnComplete(() => Destroy(gameObject));
    }

    public void PlayAttackAnimation(RectTransform targetRect, System.Action onHit)
    {
        if (unitRect == null || targetRect == null)
            return;
        isAttacking = true;
        Vector3 originalPosition = unitRect.position;
        Vector3 targetPosition = targetRect.position;
        Vector3 direction = (targetPosition - originalPosition).normalized;
        Vector3 attackPosition = targetPosition - direction * attackStopDistance;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(unitRect.DOMove(attackPosition, attackMoveDuration).SetEase(Ease.OutQuad));
        sequence.AppendCallback(() => onHit?.Invoke());
        sequence.Append(unitRect.DOMove(originalPosition, attackMoveDuration).SetEase(Ease.InQuad));
        sequence.OnComplete(() => isAttacking = false);
        sequence.OnKill(() => isAttacking = false);
    }

}
