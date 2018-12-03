using System;
using UnityEngine;

[CreateAssetMenu(menuName = "LD43/GameEvent", fileName = "Event", order = 0)]
public class GameEvent : ScriptableObject
{
    public string eventDescription;
    public Sprite image;
    public int turns = 1;

    public GameEffect[] effects;

    public string GetText()
    {
        string textEffects = turns > 1 ? ("<i>Take effect for " + turns + " turns (end of turn)</i>") : "<i>Take effect now.</i>";

        Array.ForEach(effects, effect =>
        {
            textEffects += "\n" + effect.GetText();
        });

        return eventDescription + "\n\n" + textEffects;
    }
}