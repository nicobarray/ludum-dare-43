using UnityEngine;

[CreateAssetMenu(menuName = "LD43/GameEffect", fileName = "Event", order = 0)]
public class GameEffect : ScriptableObject
{
    public enum EffectType
    {
        Villager,
        Dice,
        Wood,
        Food,
        Stone,
        Shield
    }

    public int value;
    public EffectType type;

    public string GetText()
    {
        return (value > 0 ? "+" : "") + value + " " + type;
    }
}