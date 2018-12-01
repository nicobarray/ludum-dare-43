using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dices : MonoBehaviour
{

    public GameObject dicePrefab;

    public List<Dice> dices;

    public void AddDice()
    {
        GameObject newDice = Instantiate(
            dicePrefab,
            Vector3.zero,
            Quaternion.AngleAxis(UnityEngine.Random.value * 360, Vector3.forward)
        );
        dices.Add(newDice.GetComponent<Dice>());
        dices[dices.Count - 1].Shuffle();
		ReorderDices();
    }

    void ReorderDices()
    {
        for (int i = 0; i < dices.Count; i++)
        {
            dices[i].transform.position = new Vector3(transform.position.x - i * Mathf.Sqrt(2), 0, 0);
        }
    }

    public void RemoveDice(int index)
    {
		dices.RemoveAt(index);
		ReorderDices();
    }

    public void Shuffle()
    {
        dices.ForEach(dice => dice.Shuffle());
    }

	void Start() {
		ReorderDices();
	}
}
