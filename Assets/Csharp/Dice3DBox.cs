using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice3DBox : MonoBehaviour
{
    public Dice3D dicePrefab;

    public Transform spawnOrigin;
    public Transform floor;

    List<Dice3D> dices;
    Coroutine coroutine;

    public void Throw(int count, Action<int> onDiceStabilize, Action<int> onTotal)
    {
        Reset();
        coroutine = StartCoroutine(ThrowDices(count, onDiceStabilize, onTotal));
    }

    IEnumerator ThrowDices(int count, Action<int> onDiceStabilize, Action<int> onTotal)
    {
        int stabilizedDices = 0;
        int score = 0;
        for (int i = 0; i < count; i++)
        {
            Dice3D dice = Instantiate(dicePrefab, Vector3.zero, Quaternion.identity);
            dice.Initialize(spawnOrigin, floor);
            dices.Add(dice);
            dice.onDiceStabilize += (value) =>
            {
                stabilizedDices++;
                score += value;
                onDiceStabilize(value);
            };
            yield return new WaitForSeconds(1);
        }

        yield return new WaitUntil(() => stabilizedDices == count);

        onTotal(score);
        coroutine = null;
    }

    public void Reset()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        for (int i = dices.Count - 1; i >= 0; i--)
        {
            Destroy(dices[i].gameObject);
            dices.RemoveAt(i);
        }
    }
}