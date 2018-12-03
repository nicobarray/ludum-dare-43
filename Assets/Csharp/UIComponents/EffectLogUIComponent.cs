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

    public void AddEffect(GameEffect effect)
    {
        EffectUIComponent effectUIComponent = Instantiate(effectUIPrefab, stack);
        effectUIComponent.Reset(effect);
        _effects.Push(effectUIComponent);
    }

    public void ApplyEffects(Action onAfterEffect)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(ApplyEffectAsync(onAfterEffect));
    }

    IEnumerator ApplyEffectAsync(Action onAfterEffect)
    {
        while (_effects.Count > 0)
        {
            EffectUIComponent effectUIComponent = _effects.Pop();

            effectUIComponent.ApplyEffect();

            yield return new WaitForSeconds(.33f);
            Destroy(effectUIComponent.gameObject);
        }

        onAfterEffect();
    }
}