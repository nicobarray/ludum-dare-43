using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villagers : MonoBehaviour
{
    public GameObject villagerPrefab;

    public List<Villager> villagers;

    private Comparison<Villager> hpSorter = new Comparison<Villager>((left, right) =>
            {
                return left.hp - right.hp;
            });
    private Coroutine _coroutine;

    public void AddVillager()
    {
        GameObject newVillager = Instantiate(
            villagerPrefab,
            Vector3.zero,
            Quaternion.identity
        );

        newVillager.transform.SetParent(transform);

        Villager villager = newVillager.GetComponent<Villager>();
        villagers.Add(villager);

        // villagers[villagers.Count - 1].Stable();

        ReorderVillagers();
    }

    public void ReorderVillagers()
    {
        for (int i = 0; i < villagers.Count; i++)
        {
            villagers[i].transform.localPosition = new Vector3(i * Mathf.Sqrt(2), 0, 0);
            villagers[i].name = "Villager_" + i;
        }
    }

    public void RemoveVillager(Villager villager)
    {
        Destroy(villager.gameObject);
        villagers.Remove(villager);
        ReorderVillagers();
    }

    public void Reset()
    {
        villagers.ForEach(vil =>
        {
            vil.canAct = false;
        });
    }

    public void Eat()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(EatAsync());
    }

    public IEnumerator EatAsync()
    {
        List<Villager> sortedVillagers = new List<Villager>(villagers);
        sortedVillagers.Sort(hpSorter);

        List<Villager> dead = new List<Villager>();
        int food = Game.instance.meals;
        foreach (var vil in sortedVillagers)
        {
            if (food > 0)
            {
                food--;

                Game.instance.ApplyEffect(GameEffect.EffectType.Food, -1);

                vil.Heal();
                Game.instance.gameCanvas.PopFloatingText(vil.transform.position, "Tasty", TextKind.Neutral);
            }
            else
            {
                vil.Hunger();
                Game.instance.gameCanvas.PopFloatingText(vil.transform.position, "Hungry", TextKind.Negative);

                if (vil.IsDead())
                {
                    Game.instance.ApplyEffect(GameEffect.EffectType.Villager, -1, vil);
                }
            }

            yield return new WaitForSeconds(1.5f);
        }

        _coroutine = null;
    }
}