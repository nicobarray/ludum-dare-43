using UnityEngine;

[CreateAssetMenu(menuName = "LD43/EventPair", fileName = "Pair")]
public class ScriptableEventPair : ScriptableObject
{
    public GameEvent option1;
    public GameEvent option2;
}