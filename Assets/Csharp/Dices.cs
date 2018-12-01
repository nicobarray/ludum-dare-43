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

        newDice.transform.SetParent(transform);

        Dice dice = newDice.GetComponent<Dice>();
        dices.Add(dice);
        dice.SetManager(this);

        dices[dices.Count - 1].Shuffle();
        ReorderDices();
    }

    void ReorderDices()
    {
        for (int i = 0; i < dices.Count; i++)
        {
            dices[i].transform.position = new Vector3(transform.position.x - i * Mathf.Sqrt(2), 0, 0);
            dices[i].name = "Dice_" + i;
        }
    }

    public void RemoveDice(Dice dice)
    {
        Destroy(dice.gameObject);
        dices.Remove(dice);
        ReorderDices();
    }

    public void Shuffle()
    {
        dices.ForEach(dice => dice.Shuffle());
    }

    void Start()
    {
        ReorderDices();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                GameObject hit = hitInfo.transform.root.gameObject;

                Dice dice = hit.GetComponent<Dice>();

                if (dice != null)
                {
                    Debug.Log(name + " removed");
                    RemoveDice(dice);
                }
            }
        }

    }
}
