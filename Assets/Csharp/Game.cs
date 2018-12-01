using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public enum TurnSteps
    {
        Upkeep,
        Dices,
        Main,
        Event
    }

    [Header("Prefabs")]
    public GameObject dicesPrefab;

    [Header("Scene References")]
    public Transform dicesOrigin;
    public GameCanvas gameCanvas;

    [Header("Game data (Read only)")]
    public TurnSteps steps;
    public int gameTurn;

    Dices dices;

    void OnEnable()
    {
        Debug.Assert(dicesOrigin != null, "Place the dices origin somewhere on the scene !");

        // ? Instantiate what needs to be initiated before game start here. 
        dices = Instantiate(dicesPrefab, dicesOrigin.position, Quaternion.identity).GetComponent<Dices>();

        // ? Link object between them here.
        gameCanvas.throwDices.onClick.AddListener(new UnityEngine.Events.UnityAction(dices.Shuffle));
        gameCanvas.add1Dice.onClick.AddListener(new UnityEngine.Events.UnityAction(dices.AddDice));
    }

    void OnDisable()
    {
        gameCanvas.throwDices.onClick.RemoveAllListeners();

        Destroy(dices.gameObject);
    }
}
