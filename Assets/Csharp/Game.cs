using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public enum TurnSteps
    {
        Upkeep = 0,
        Dices,
        Main,
        Event,
        NEXT_TURN
    }

    [Header("Prefabs")]
    public GameObject dicesPrefab;

    [Header("Scene References")]
    public Transform dicesOrigin;
    public GameCanvas gameCanvas;

    [Header("Game data (Read only)")]
    public TurnSteps currentStep = TurnSteps.Upkeep;
    public int gameTurn;
    public bool nextStep = false;

    Dices dices;

    void UpkeepStep()
    {
        gameCanvas.phaseText.text = "Upkeep";
    }

    void DicesStep()
    {
        gameCanvas.phaseText.text = "Throw dices!";
    }

    void MainStep()
    {
        gameCanvas.phaseText.text = "Act!";
    }

    void EndStep()
    {
        gameCanvas.phaseText.text = "Selected among 3";
    }

    void NextStep()
    {
        currentStep = (TurnSteps)(currentStep + 1);

        if (currentStep == TurnSteps.NEXT_TURN)
        {
            gameTurn++;
            currentStep = (TurnSteps)0;
        }

        switch (currentStep)
        {
            case TurnSteps.Upkeep:
                UpkeepStep();
                break;
            case TurnSteps.Dices:
                DicesStep();
                break;
            case TurnSteps.Main:
                MainStep();
                break;
            case TurnSteps.Event:
                EndStep();
                break;
            default:
                Debug.LogError("Should not happen: " + currentStep);
                break;
        }
    }

    void OnEnable()
    {
        Debug.Assert(dicesOrigin != null, "Place the dices origin somewhere on the scene !");

        // ? Instantiate what needs to be initiated before game start here. 
        dices = Instantiate(dicesPrefab, dicesOrigin.position, Quaternion.identity).GetComponent<Dices>();

        // ? Link object between them here.
        gameCanvas.throwDices.onClick.AddListener(new UnityEngine.Events.UnityAction(dices.Shuffle));
        gameCanvas.add1Dice.onClick.AddListener(new UnityEngine.Events.UnityAction(dices.AddDice));
        gameCanvas.nextStep.onClick.AddListener(new UnityEngine.Events.UnityAction(NextStep));
    }

    void OnDisable()
    {
        gameCanvas.throwDices.onClick.RemoveAllListeners();

        Destroy(dices.gameObject);
    }
}
