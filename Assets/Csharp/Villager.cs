using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    public bool canAct = true;

    bool drag = false;
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
    }

    void Raycast(Action<RaycastHit[]> onRaycast)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (hits.Length > 0)
        {
            onRaycast(hits);
        }
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

        if (drag)
        {
            if (Input.GetMouseButton(0))
            {
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
            }
            else
            {
                drag = false;

                Raycast((hits) =>
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
                    }

                    if (p == null || p.worker != null)
                    {
                        transform.position = positionBeforeDrag;
                    }
                    else
                    {
                        int points = p.scriptable.diceCost;
                        if (Game.instance.dicePoint >= points)
                        {
                            Game.instance.RemoveDicePoints(points);
                            canAct = false;
                            p.worker = this;
                            transform.position = p.transform.GetChild(0).position;
                        }
                        else
                        {
                            transform.position = positionBeforeDrag;
                        }
                    }
                });
            }
        }
        else
        {
            Raycast((hits) =>
            {
                foreach (var item in hits)
                {
                    Villager villager = item.transform.GetComponent<Villager>();
                    if (villager == this)
                    {
                        if (Input.GetMouseButton(0))
                        {
                            drag = true;
                            positionBeforeDrag = transform.position;
                        }
                    }
                }
            });
        }
    }
}
