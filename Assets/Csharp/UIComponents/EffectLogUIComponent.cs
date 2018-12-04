using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLogUIComponent : MonoBehaviour
{
    [Header("Prefabs")]
    public EffectUIComponent effectUIPrefab;

    [Header("References")]
    public Transform stack;

    Stack<EffectUIComponent> _effects = new Stack<EffectUIComponent>();
    Coroutine _coroutine;

    public void AddEvent(GameEvent gameEvent)
    {
        foreach (var effect in gameEvent.effects)
        {
            AddEffect(effect.type, effect.value);
        }
    }

    public void AddEffect(GameEffect.EffectType type, int value, object target = null)
    {
        GameEffect eff = GameEffect.CreateInstance<GameEffect>();
        eff.type = type;
        eff.value = value;
        AddEffect(eff, target);
        Destroy(eff);
    }

    public void AddEffect(GameEffect effect, object target = null)
    {
        EffectUIComponent effectUIComponent = Instantiate(effectUIPrefab, stack);
        effectUIComponent.transform.SetAsFirstSibling();

        effectUIComponent.Reset(effect, target);
        _effects.Push(effectUIComponent);
    }

    public void ApplyEffects()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(ApplyEffectsAsync());
    }

    public IEnumerator ApplyEffectsAsync()
    {
        while (_effects.Count > 0)
        {
            EffectUIComponent effectUIComponent = _effects.Pop();

            effectUIComponent.ApplyEffect();
            Destroy(effectUIComponent.gameObject);

            yield return new WaitForSeconds(1.5f);
        }
    }
}