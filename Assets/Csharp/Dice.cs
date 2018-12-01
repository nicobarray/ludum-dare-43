using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{

    public Sprite[] faces;

    public SpriteRenderer spriteRenderer;

    public int value = 1;

    void RefreshSpriteRenderer()
    {
        spriteRenderer.sprite = faces[value - 1];
    }

    public void Shuffle()
    {
        value = UnityEngine.Random.Range(1, 7);
        RefreshSpriteRenderer();
    }

    // Use this for initialization
    void Start()
    {
        RefreshSpriteRenderer();
    }
}
