using System;
using System.Collections.Generic;
using UnityEngine;

public class Villagers : MonoBehaviour
{
    public GameObject villagerPrefab;

    public List<Villager> villagers;

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

    public void Eat(Action<GameEffect> ApplyEffect)
    {
        for (int i = villagers.Count - 1; i >= 0; i--)
        {
            Villager vil = villagers[i];
            if (Game.instance.meals <= 0)
            {
                vil.Hunger();

                if (vil.IsDead())
                {
                    RemoveVillager(vil);
                }
            }
            else
            {
                var eff = ScriptableObject.CreateInstance<GameEffect>();
                eff.type = GameEffect.EffectType.Food;
                eff.value = -1;

                ApplyEffect(eff);

                vil.Heal();

                Destroy(eff);
            }
        }

    }
}