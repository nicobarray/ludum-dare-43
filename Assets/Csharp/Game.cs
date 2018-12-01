using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game instance;

    public enum TurnSteps
    {
        Upkeep = 0,
        Dices,
        Main,
        Event,
        NEXT_TURN
    }

    [Header("Assets")]
    public GameObject dicesPrefab;
    public GameObject villagersPrefab;
    public GameObject placesPrefab;
    public GameEvent[] eventPool;

    [Header("Scene References")]
    public GameCanvas gameCanvas;
    public Transform dicesOrigin;
    public Transform placesOrigin;
    public Transform villagersOrigin;

    [Header("Game data (Read only)")]
    public TurnSteps currentStep = TurnSteps.Upkeep;
    public int gameTurn;
    public bool nextStep = false;
    public int dicePoint = 0;
    public int blockPoint = 0;

    public int dicesCount = 2;
    public int villagersCount = 1;
    public int placesCount = 3;
    public int wood = 0;
    public int meals = 0;
    public int stone = 0;

    public int TURNS_BEFORE_WINTER = 15;

    Dices dices;
    Places places;
    Villagers villagers;

    List<GameEvent> selectedEvents = new List<GameEvent>();
    GameEvent[] currentSelection = new GameEvent[2];
    List<GameEvent> repeatableEvents = new List<GameEvent>();

    public void CollectResourceFrom(Place p)
    {
        ApplyEffect(p.scriptable.effect);
    }

    public void RemoveDicePoints(int points)
    {
        dicePoint -= points;
        if (dicePoint < 0)
        {
            dicePoint = 0;
        }

        gameCanvas.dicePointText.text = "DP: " + dicePoint;
    }

    void UpkeepStep()
    {
        gameCanvas.throwDices.gameObject.SetActive(false);
        gameCanvas.add1Dice.gameObject.SetActive(false);
        gameCanvas.nextStep.gameObject.SetActive(false);
        dices.gameObject.SetActive(false);

        villagers.villagers.ForEach(vil =>
        {
            vil.ResetPosition();
        });

        dicePoint = 0;
        gameCanvas.phaseText.text = "Upkeep";
        gameCanvas.turnText.text = (TURNS_BEFORE_WINTER - gameTurn) + " left B.W.";
        gameCanvas.dicePointText.text = "DP: " + dicePoint;
        gameCanvas.blockPointText.text = "BP: " + blockPoint;
        gameCanvas.mealsText.text = "Meals: " + meals;
        gameCanvas.woodText.text = "Wood: " + wood;
        gameCanvas.stoneText.text = "Stone: " + stone;

        if (eventPool.Length < 2)
        {
            NextStep();
        }

        int option1Index = UnityEngine.Random.Range(0, eventPool.Length);
        int option2Index = UnityEngine.Random.Range(0, eventPool.Length);
        while (option1Index == option2Index)
        {
            option2Index = UnityEngine.Random.Range(0, eventPool.Length);
        }

        currentSelection[0] = eventPool[option1Index];
        gameCanvas.eventOption1Image.sprite = currentSelection[0].image;
        gameCanvas.eventOption1Text.text = currentSelection[0].GetText();

        currentSelection[1] = eventPool[option2Index];
        gameCanvas.eventOption2Image.sprite = currentSelection[1].image;
        gameCanvas.eventOption2Text.text = currentSelection[1].GetText();

        gameCanvas.eventFrame.SetActive(true);
    }

    void DicesStep()
    {
        gameCanvas.eventFrame.SetActive(false);
        gameCanvas.throwDices.gameObject.SetActive(true);
        gameCanvas.nextStep.gameObject.SetActive(false);
        dices.gameObject.SetActive(true);
        dices.Unstable();

        gameCanvas.phaseText.text = "Throw dices!";
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

        villagers.villagers.ForEach(vil =>
        {
            vil.canAct = true;
        });
    }

    void EndStep()
    {
        villagers.villagers.ForEach(vil =>
        {
            vil.canAct = false;
        });

        // Apply repeatable effects.
        for (int i = repeatableEvents.Count - 1; i >= 0; i--)
        {
            GameEvent ev = repeatableEvents[i];

            ApplyEvent(ev);

            if (ev.turns <= 0)
            {
                repeatableEvents.RemoveAt(i);
            }
        }

        dices.gameObject.SetActive(false);
        gameCanvas.phaseText.text = "Gathering time";
    }

    void NextStep()
    {
        currentStep = (TurnSteps)(currentStep + 1);

        if (currentStep == TurnSteps.NEXT_TURN)
        {
            gameTurn++;

            if (TURNS_BEFORE_WINTER - gameTurn <= 0)
            {
                if (villagers.GetComponentsInChildren<Villager>().Length > 0)
                {
                    SceneManager.LoadScene("victory");
                }
                else
                {
                    SceneManager.LoadScene("defeat");
                }

                return;
            }

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

    void PickEffect(int which)
    {
        Debug.Log("Apply effect !");
        GameEvent ev = currentSelection[which];

        if (ev.turns > 1)
        {
            // The event applies at the end of the turn.
            repeatableEvents.Add(ev);
        }
        else
        {
            ApplyEvent(ev);
        }

        selectedEvents.Add(currentSelection[which]);
    }

    void ApplyEvent(GameEvent ev)
    {
        Array.ForEach(ev.effects, eff =>
        {
            ApplyEffect(eff);
        });
    }

    void ApplyEffect(GameEffect eff)
    {
        switch (eff.type)
        {
            case GameEffect.EffectType.Wood:
                wood += eff.value;
                if (wood < 0)
                {
                    wood = 0;
                }
                gameCanvas.woodText.text = "Wood: " + wood;
                break;
            case GameEffect.EffectType.Stone:
                stone += eff.value;
                if (stone < 0)
                {
                    stone = 0;
                }
                gameCanvas.stoneText.text = "Stone: " + stone;
                break;
            case GameEffect.EffectType.Food:
                meals += eff.value;
                if (meals < 0)
                {
                    meals = 0;
                }
                gameCanvas.mealsText.text = "Meals: " + meals;
                break;
            case GameEffect.EffectType.Dice:
                dicesCount += eff.value;
                if (dicesCount < 0)
                {
                    dicesCount = 0;
                }

                while (dices.dices.Count > dicesCount)
                {
                    dices.RemoveDice(dices.dices[0]);
                }

                while (dices.dices.Count < dicesCount)
                {
                    dices.AddDice();
                }

                break;
            case GameEffect.EffectType.Villager:
                villagersCount += eff.value;
                if (villagersCount < 0)
                {
                    villagersCount = 0;
                }

                while (villagers.villagers.Count > villagersCount)
                {
                    villagers.RemoveVillager(villagers.villagers[0]);
                }

                while (villagers.villagers.Count < villagersCount)
                {
                    villagers.AddVillager();
                }

                break;
        }
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void OnEnable()
    {
        Debug.Assert(dicesOrigin != null, "Place the dices origin somewhere on the scene !");

        // ? Instantiate what needs to be initiated before game start here. 
        dices = Instantiate(dicesPrefab, dicesOrigin.position, Quaternion.identity).GetComponent<Dices>();
        places = Instantiate(placesPrefab, placesOrigin.position, Quaternion.identity).GetComponent<Places>();
        villagers = Instantiate(villagersPrefab, villagersOrigin.position, Quaternion.identity).GetComponent<Villagers>();

        // ? Link object between them here.
        gameCanvas.throwDices.onClick.AddListener(new UnityEngine.Events.UnityAction(DicesStep_ThrowDices));
        gameCanvas.add1Dice.onClick.AddListener(new UnityEngine.Events.UnityAction(dices.AddDice));
        gameCanvas.nextStep.onClick.AddListener(new UnityEngine.Events.UnityAction(NextStep));
        gameCanvas.eventOption1.onClick.AddListener(new UnityEngine.Events.UnityAction(NextStep));
        gameCanvas.eventOption1.onClick.AddListener(new UnityEngine.Events.UnityAction(() => PickEffect(0)));
        gameCanvas.eventOption2.onClick.AddListener(new UnityEngine.Events.UnityAction(NextStep));
        gameCanvas.eventOption2.onClick.AddListener(new UnityEngine.Events.UnityAction(() => PickEffect(1)));
    }

    void Start()
    {
        for (int i = 0; i < dicesCount; i++)
        {
            dices.AddDice();
        }

        for (int i = 0; i < villagersCount; i++)
        {
            villagers.AddVillager();
        }

        for (int i = 0; i < placesCount; i++)
        {
            places.AddPlace();
        }

        UpkeepStep();
    }

    void OnDisable()
    {
        gameCanvas.throwDices.onClick.RemoveAllListeners();
        gameCanvas.add1Dice.onClick.RemoveAllListeners();
        gameCanvas.nextStep.onClick.RemoveAllListeners();
        gameCanvas.eventOption1.onClick.RemoveAllListeners();
        gameCanvas.eventOption2.onClick.RemoveAllListeners();
    }
}
