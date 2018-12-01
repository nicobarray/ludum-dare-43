using UnityEngine;

[CreateAssetMenu(menuName = "LD43/GameEffect", fileName = "Event", order = 0)]
public class GameEffect : ScriptableObject
{
    public enum EffectType
    {
        Villager,
        Dice,
        Wood,
        Food
    }

    public int value;
    public EffectType type;
    public bool instant;

    public string GetText()
    {
        return value > 0 ? "+" : "" + value + " " + type;
    }
}