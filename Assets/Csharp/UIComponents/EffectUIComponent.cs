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

    private GameEffect.EffectType effectType;
    private int effectValue;
    private object effectTarget;

    public void Reset(GameEffect effect, object target = null)
    {
        this.effectType = effect.type;
        this.effectValue = effect.value;
        this.effectTarget = target;

        icon.sprite = IconSpriteFromType(effect.type);
        text.text = (effect.value > 0 ? "+" : "") + effect.value + " " + effect.type;
    }

    public void ApplyEffect()
    {
        Game.instance.ApplyEffect(effectType, effectValue, effectTarget);
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
            case GameEffect.EffectType.Stone:
                return stoneIcon;
            default:
                return woodIcon;
        }
    }
}