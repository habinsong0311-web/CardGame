using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitBoardCardView : MonoBehaviour
{
    [Header("공격 가능 표시")]
    [SerializeField] private Image unitFrame;
    [SerializeField] private Color normalFrameColor = Color.white;
    [SerializeField] private Color attackReadyColor = Color.green;
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
    private int currentAttack;
    private int currentHealth;
    private bool canAttack;
    private PlayerState ownerPlayer;
    public PlayerState OwnerPlayer => ownerPlayer;
    public int CurrentAttack => currentAttack;
    public int CurrentHealth => currentHealth;
    public bool CanAttack => canAttack;
    public bool HasTaunt => cardSetting != null && cardSetting.HasKeyword(KeyWord.도발);
    public void Setup(CardSetting cardData , PlayerState player)
    {
        if (cardData == null || player == null)
        {
            return;
        }
        ownerPlayer = player;
        cardSetting = cardData;
        if (tauntFrame != null)
        {
            tauntFrame.SetActive(cardData.HasKeyword(KeyWord.도발));
        }//도발이있으면 것에 프레임 보임
        canAttack = cardData.HasKeyword(KeyWord.돌진);//돌진 키워드가 있으면 바로 공격할수있음
        UpdateAttackReadyUI();
        currentHealth = cardData.MaxHealth;
        currentAttack = cardData.Attack;
        artworkImage.sprite = cardData.Artwork;
        UpdateStatText();
    }
    private void UpdateAttackReadyUI()
    {
        if (unitFrame == null)
            return;

        if (canAttack)
            unitFrame.color = attackReadyColor;
        else
            unitFrame.color = normalFrameColor;
    }
    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }
        currentHealth -= damage;
        UpdateStatText();
        if (currentHealth <= 0)
        {
            ownerPlayer.Graveyard.AddCard(cardSetting);
            Destroy(gameObject);
        }
    }
    public void TakeHeal(int effectValue)
    {
        if(effectValue <= 0)
        {
            return;
        }
        currentHealth += effectValue;
        UpdateStatText();
    }
    public void UseAttack()
    {
        canAttack = false;
        UpdateAttackReadyUI();
    }
    public void ResetAttack()
    {
        canAttack = true;
        UpdateAttackReadyUI();
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
}
