using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public Sprite[] faces;

    public SpriteRenderer spriteRenderer;

    public int value = 1;

    Coroutine unstableCoroutine = null;

    public void Shuffle()
    {
        value = UnityEngine.Random.Range(1, 7);
        RefreshSpriteRenderer();
    }

    public void Unstable()
    {
        if (unstableCoroutine != null)
        {
            StopCoroutine(unstableCoroutine);
        }

        unstableCoroutine = StartCoroutine(UnstableEnumerator());
    }

    public void Stable()
    {
        if (unstableCoroutine != null)
        {
            StopCoroutine(unstableCoroutine);
        }

        Shuffle();
    }

    IEnumerator UnstableEnumerator()
    {
        while (true)
        {
            Shuffle();
            yield return new WaitForSeconds(.25f);
        }
    }

    void RefreshSpriteRenderer()
    {
        spriteRenderer.sprite = faces[value - 1];
    }


    // Use this for initialization
    void Start()
    {
        RefreshSpriteRenderer();
    }
}
