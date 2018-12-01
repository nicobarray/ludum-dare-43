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
    public int dicePoint = 0;
    public int blockPoint = 0;
    public int dicesCount = 3;

    Dices dices;

    void UpkeepStep()
    {
        gameCanvas.throwDices.gameObject.SetActive(false);
        gameCanvas.add1Dice.gameObject.SetActive(false);

        dicePoint = 0;
        gameCanvas.phaseText.text = "Upkeep";
        gameCanvas.dicePointText.text = "DP: " + dicePoint;
        gameCanvas.blockPointText.text = "BP: " + blockPoint;
    }

    void DicesStep()
    {
        gameCanvas.throwDices.gameObject.SetActive(true);
        gameCanvas.nextStep.gameObject.SetActive(false);

        gameCanvas.phaseText.text = "Throw dices!";
    }

    void DiceStep_UpdateDiceCount()
    {
        for (int i = 0; i < dicesCount; i++)
        {
            dices.AddDice();
        }
    }

    void DicesStep_ThrowDices()
    {
        dices.Shuffle();
        dices.dices.ForEach(dice => dicePoint += dice.value);
        gameCanvas.dicePointText.text = "DP: " + dicePoint;
        NextStep();

        gameCanvas.throwDices.gameObject.SetActive(false);
    }

    void MainStep()
    {
        gameCanvas.nextStep.gameObject.SetActive(true);
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
        gameCanvas.throwDices.onClick.AddListener(new UnityEngine.Events.UnityAction(DicesStep_ThrowDices));
        gameCanvas.add1Dice.onClick.AddListener(new UnityEngine.Events.UnityAction(dices.AddDice));
        gameCanvas.nextStep.onClick.AddListener(new UnityEngine.Events.UnityAction(NextStep));

        DiceStep_UpdateDiceCount();
        UpkeepStep();
    }

    void OnDisable()
    {
        gameCanvas.throwDices.onClick.RemoveAllListeners();

        Destroy(dices.gameObject);
    }
}
