using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    private static Villager dragged = null;

    public Sprite maleSprite;
    public Sprite femaleSprite;

    public bool canAct = true;
    public TMPro.TextMeshPro hpText;

    Vector3 positionBeforeDrag;
    Vector3 initialPosition;

    public int hp = 2;

    public bool IsDead()
    {
        return hp <= 0;
    }

    public void Hunger()
    {
        hp--;
        hpText.text = hp + "/2";
        hpText.color = Color.red;
    }

    public void Heal()
    {
        hp = 2;
        hpText.text = "2/2";
        hpText.color = Color.green;
    }

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = UnityEngine.Random.value > 0.5f ? femaleSprite : maleSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.instance.currentStep != Game.TurnSteps.Main)
        {
            return;
        }

        if (!canAct)
        {
            return;
        }

        if (dragged == this)
        {
            if (Input.GetMouseButton(0))
            {
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
            }
            else
            {
                dragged = null;

                Utils.Raycast((hits) =>
                {
                    Place p = null;
                    foreach (var item in hits)
                    {
                        Place place = item.transform.GetComponent<Place>();
                        if (place != null)
                        {
                            p = place;
                            break;
                        }

                        if (item.transform.name == "PlayerSlot")
                        {
                            p = item.transform.GetComponentInParent<Place>();
                        }
                    }

                    if (p == null || p.worker != null)
                    {
                        transform.position = positionBeforeDrag;
                        return;
                    }

                    int points = p.scriptable.diceCost;
                    if (Game.instance.dicePoint < points)
                    {
                        transform.position = positionBeforeDrag;
                        return;

                    }

                    Game.instance.RemoveDicePoints(points);
                    Game.instance.gameCanvas.effectLog.AddEffect(p.scriptable.effect);
                    canAct = false;
                    p.worker = this;
                    transform.position = p.transform.GetChild(0).position;
                });
            }
        }
        else if (dragged == null)
        {
            Utils.Raycast((hits) =>
            {
                foreach (var item in hits)
                {
                    Villager villager = item.transform.GetComponent<Villager>();
                    if (villager == this)
                    {
                        if (Input.GetMouseButton(0))
                        {
                            Villager.dragged = this;
                            positionBeforeDrag = transform.position;
                        }
                    }
                }
            });
        }
    }
}
