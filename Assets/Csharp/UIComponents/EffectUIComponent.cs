using UnityEngine;
using UnityEngine.UI;

public class EffectUIComponent : MonoBehaviour
{
    [Header("Assets")]
    public Sprite woodIcon;
    public Sprite stoneIcon;
    public Sprite foodIcon;
    public Sprite villagerIcon;
    public Sprite diceIcon;

    [Header("References")]
    public Image icon;
    public TMPro.TextMeshProUGUI text;

    private GameEffect effect;

    public void Reset(GameEffect effect)
    {
        this.effect = effect;

        icon.sprite = IconSpriteFromType(effect.type);
        text.text = (effect.value > 0 ? "+" : "") + effect.value + " " + effect.type;
    }

    public void ApplyEffect()
    {
        Game.instance.ApplyEffect(effect);
    }

    Sprite IconSpriteFromType(GameEffect.EffectType type)
    {
        switch (type)
        {
            case GameEffect.EffectType.Wood:
                return woodIcon;
            case GameEffect.EffectType.Food:
                return foodIcon;
            case GameEffect.EffectType.Villager:
                return villagerIcon;
            case GameEffect.EffectType.Dice:
                return diceIcon;
            default:
                return woodIcon;
        }
    }
}