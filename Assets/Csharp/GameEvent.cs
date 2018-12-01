using System;
using UnityEngine;

[CreateAssetMenu(menuName = "LD43/GameEvent", fileName = "Event", order = 0)]
public class GameEvent : ScriptableObject
{
    public string eventDescription;
    public Sprite image;

    public GameEffect[] effects;

    public string GetText()
    {
        string textEffects = "";

        Array.ForEach(effects, effect =>
        {
            textEffects += effect.GetText() + "\n";
        });

        return eventDescription + "\n\n" + textEffects;
    }
}