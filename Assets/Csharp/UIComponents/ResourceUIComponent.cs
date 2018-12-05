using UnityEngine;

public class ResourceUIComponent : MonoBehaviour, Tooltipable
{
    public TMPro.TextMeshProUGUI text;

    public GameEffect.EffectType type;

    public string GetText()
    {
        string result = "";
        switch (type)
        {
            case GameEffect.EffectType.Dice:
                result = "Villager spent action points to work in buildings during the day. Action points are obtained when throwing the dices.";
                break;
            case GameEffect.EffectType.Villager:
                result = "Villager population count. Just hope that everybody survive until winter...";
                break;
            case GameEffect.EffectType.Wood:
                result = "Amount of wood in storage. Wood is primarly obtained by sending a villager in the woods. It can be trade(todo) or used to create and repare(todo) buildings.";
                break;
            case GameEffect.EffectType.Stone:
                result = "Amount of stone in storage. Wood is primarly obtained by sending a villager in the woods. It can be trade(todo) or used to create and repare(todo) buildings.";
                break;
            case GameEffect.EffectType.Food:
                result = "Amount of food in storage. Each villager consumes 1 food at the end of turn. If no food is available, the villagers are going to slowly die from hunger.";
                break;
            case GameEffect.EffectType.Shield:
                result = "Each shield protects one villager when fate decides otherwise. Send villagers to the watchtower to accumulate shields.";
                break;
            default:
                break;
        }
        return name + ": " + text.text + ". " + result;
    }
}