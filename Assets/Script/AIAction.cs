using UnityEngine;
public enum AIActionType
{
    Summon,
    UseSkillOnUnit,
    UseSkillOnPlayer,
    AttackUnit,
    AttackPlayer
}
public class AIAction
{
    public AIActionType ActionType;
    public int Score;
    public CardSetting Card;
    public CardView CardView;
    public int SlotIndex = -1;
    public UnitBoardCardView Attacker;
    public UnitBoardCardView TargetUnit;
    public PlayerState TargetPlayer;
}
