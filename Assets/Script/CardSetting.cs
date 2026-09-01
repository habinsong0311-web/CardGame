using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Data")]
public class CardSetting : ScriptableObject
{
    [SerializeField] private string cardId;
    [SerializeField] private Sprite artwork;
    [SerializeField] private CardType cardType;
    [SerializeField] private string cardName;
    [SerializeField,Min(0)] private int cost;
    [SerializeField,Min(0)] private int attack;
    [SerializeField, Min(0)] private int maxHealth;
    [SerializeField, Min(1)] private int effectValue;
    [SerializeField] private string explanation;
    [SerializeField] private List<KeyWord> keywords = new List<KeyWord>();
    [Header("소환 효과")]
    [SerializeField] private bool hasSummonEffect;


    public string CardId => cardId;
    public Sprite Artwork => artwork;
    public CardType CardType => cardType;
    public string CardName => cardName;
    public int Cost => cost;
    public int Attack => attack;
    public int MaxHealth => maxHealth;
    public int EffectValue => effectValue;
    public string Explanation => explanation;
    public IReadOnlyList<KeyWord> Keywords => keywords;
    public bool HasSummonEffect => hasSummonEffect;
    public bool HasKeyword(KeyWord keyword)
    {
        return keywords.Contains(keyword);
    }
}
