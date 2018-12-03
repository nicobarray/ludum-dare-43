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
    public ScriptableEventPair[] eventPairs;

    [Header("Scene References")]
    public GameCanvas gameCanvas;
    public Dice3DBox dice3DBox;
    public Transform placesOrigin;
    public Transform villagersOrigin;

    [Header("Game data (Read only)")]
    public TurnSteps currentStep = TurnSteps.Upkeep;
    public int gameTurn = 0;
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

        gameCanvas.resourceTexts.dice.text = dicePoint.ToString();
    }

    void UpkeepStep()
    {
        if (gameTurn > 1)
        {
            gameCanvas.PopFloatingText(gameCanvas.timers.turnText.transform, -1);
        }
        gameCanvas.timers.turnText.text = (TURNS_BEFORE_WINTER - gameTurn) + " B.W.";

        gameCanvas.ChangeStep(-125);
        gameCanvas.throwDices.gameObject.SetActive(false);
        gameCanvas.nextStep.gameObject.SetActive(false);

        dice3DBox.Reset();
        dice3DBox.gameObject.SetActive(false);
        places.gameObject.SetActive(false);
        villagers.gameObject.SetActive(false);

        villagers.ReorderVillagers();

        dicePoint = 0;

        gameCanvas.resourceTexts.villager.text = villagersCount.ToString();
        gameCanvas.resourceTexts.dice.text = dicesCount.ToString();
        gameCanvas.resourceTexts.food.text = meals.ToString();
        gameCanvas.resourceTexts.wood.text = wood.ToString();
        gameCanvas.resourceTexts.stone.text = stone.ToString();

        gameCanvas.blockPointText.text = "BP: " + blockPoint.ToString();

        if (eventPairs.Length == 0)
        {
            NextStep();
        }

        ScriptableEventPair options = eventPairs[UnityEngine.Random.Range(0, eventPairs.Length)];

        currentSelection[0] = options.option1;
        gameCanvas.eventOption1Image.sprite = currentSelection[0].image;
        gameCanvas.eventOption1Text.text = currentSelection[0].GetText();

        currentSelection[1] = options.option2;
        gameCanvas.eventOption2Image.sprite = currentSelection[1].image;
        gameCanvas.eventOption2Text.text = currentSelection[1].GetText();

        gameCanvas.eventFrame.SetActive(true);
    }

    void DicesStep()
    {
        gameCanvas.ChangeStep(0);
        gameCanvas.eventFrame.SetActive(false);
        gameCanvas.throwDices.gameObject.SetActive(true);
        gameCanvas.nextStep.gameObject.SetActive(false);

        dice3DBox.gameObject.SetActive(true);

        places.gameObject.SetActive(false);
        villagers.gameObject.SetActive(false);
    }

    void DicesStep_ThrowDices()
    {
        gameCanvas.throwDices.gameObject.SetActive(false);

        dice3DBox.Throw(dicesCount, (diceValue) => { Debug.Log("Got " + diceValue + " !"); }, (total) =>
        {
            dicePoint = total;
            gameCanvas.resourceTexts.dice.text = dicePoint.ToString();
            gameCanvas.PopFloatingText(gameCanvas.resourceTexts.dice.transform, dicePoint);

            NextStep();

            gameCanvas.throwDices.gameObject.SetActive(false);
            dice3DBox.Reset();
            dice3DBox.gameObject.SetActive(false);
        });
    }

    void MainStep()
    {
        gameCanvas.ChangeStep(0);
        gameCanvas.nextStep.gameObject.SetActive(true);
        villagers.gameObject.SetActive(true);
        places.gameObject.SetActive(true);

        villagers.villagers.ForEach(vil =>
        {
            vil.canAct = true;
        });
    }

    public IEnumerator WaitX(float time, Action callback)
    {
        yield return new WaitForSeconds(time);

        callback();
    }

    void EndStep()
    {
        gameCanvas.ChangeStep(115);
        gameCanvas.nextStep.gameObject.SetActive(false);
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

        dice3DBox.Reset();
        dice3DBox.gameObject.SetActive(false);

        // places.CollectWork();
        villagers.Eat();
        gameCanvas.effectLog.ApplyEffects(() => StartCoroutine(WaitX(1, NextStep)));
    }

    void CheckForVictoryOrDefeat()
    {
        if (villagers.villagers.Count > 0)
        {
            SceneManager.LoadScene("victory");
        }
        else
        {
            SceneManager.LoadScene("defeat");
        }
    }

    void CheckForDefeat()
    {
        if (villagers.villagers.Count <= 0)
        {
            SceneManager.LoadScene("defeat");
        }
    }

    void NextStep()
    {
        currentStep = (TurnSteps)(currentStep + 1);

        if (currentStep == TurnSteps.NEXT_TURN)
        {
            gameTurn++;

            if (TURNS_BEFORE_WINTER - gameTurn <= 0)
            {
                CheckForVictoryOrDefeat();
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

        CheckForDefeat();
    }

    public void ApplyEffect(GameEffect eff)
    {
        Action<int> updateDices = (value) =>
        {
            dicesCount += value;
            if (dicesCount < 0)
            {
                dicesCount = 0;
            }

            gameCanvas.resourceTexts.dice.text = dicesCount.ToString();
            gameCanvas.PopFloatingText(gameCanvas.resourceTexts.dice.transform, value);
        };

        Action<int> updateBlock = (value) =>
        {
            blockPoint += value;
            if (blockPoint < 0)
            {
                blockPoint = 0;
            }

            gameCanvas.blockPointText.text = blockPoint.ToString();
            gameCanvas.PopFloatingText(gameCanvas.blockPointText.transform, value);
        };

        switch (eff.type)
        {
            case GameEffect.EffectType.Wood:
                wood += eff.value;
                if (wood < 0)
                {
                    wood = 0;
                }
                gameCanvas.resourceTexts.wood.text = wood.ToString();
                gameCanvas.PopFloatingText(gameCanvas.resourceTexts.wood.transform, eff.value);
                break;
            case GameEffect.EffectType.Stone:
                stone += eff.value;
                if (stone < 0)
                {
                    stone = 0;
                }
                gameCanvas.resourceTexts.stone.text = stone.ToString();
                gameCanvas.PopFloatingText(gameCanvas.resourceTexts.stone.transform, eff.value);
                break;
            case GameEffect.EffectType.Food:
                meals += eff.value;
                if (meals < 0)
                {
                    meals = 0;
                }
                gameCanvas.resourceTexts.food.text = meals.ToString();
                gameCanvas.PopFloatingText(gameCanvas.resourceTexts.food.transform, eff.value);
                break;
            case GameEffect.EffectType.Dice:
                updateDices(eff.value);

                break;
            case GameEffect.EffectType.Villager:
                int value = eff.value;

                // Block points take for the villager.
                if (blockPoint > 0 && value < 0)
                {
                    value = eff.value - blockPoint;
                    updateBlock(eff.value);
                }

                villagersCount += value;

                if (villagersCount < 0)
                {
                    villagersCount = 0;
                }

                gameCanvas.resourceTexts.villager.text = villagersCount.ToString();
                gameCanvas.PopFloatingText(gameCanvas.resourceTexts.villager.transform, value);

                while (villagers.villagers.Count > villagersCount)
                {
                    villagers.RemoveVillager(villagers.villagers[0]);
                }

                while (villagers.villagers.Count < villagersCount)
                {
                    villagers.AddVillager();
                }

                updateDices(eff.value);
                break;
            case GameEffect.EffectType.Block:
                updateBlock(eff.value);
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
        // ? Instantiate what needs to be initiated before game start here. 
        places = Instantiate(placesPrefab, placesOrigin.position, Quaternion.identity).GetComponent<Places>();
        villagers = Instantiate(villagersPrefab, villagersOrigin.position, Quaternion.identity).GetComponent<Villagers>();

        // ? Link object between them here.
        gameCanvas.throwDices.onClick.AddListener(new UnityEngine.Events.UnityAction(DicesStep_ThrowDices));
        gameCanvas.nextStep.onClick.AddListener(new UnityEngine.Events.UnityAction(NextStep));
        gameCanvas.eventOption1.onClick.AddListener(new UnityEngine.Events.UnityAction(NextStep));
        gameCanvas.eventOption1.onClick.AddListener(new UnityEngine.Events.UnityAction(() => PickEffect(0)));
        gameCanvas.eventOption2.onClick.AddListener(new UnityEngine.Events.UnityAction(NextStep));
        gameCanvas.eventOption2.onClick.AddListener(new UnityEngine.Events.UnityAction(() => PickEffect(1)));
    }

    void Start()
    {
        gameCanvas.timers.stepClock.transform.localEulerAngles = new Vector3(0, 0, -125);

        for (int i = 0; i < villagersCount; i++)
        {
            villagers.AddVillager();
        }

        for (int i = 0; i < placesCount; i++)
        {
            places.AddPlace(places.scriptables[i]);
        }

        UpkeepStep();
    }

    void OnDisable()
    {
        gameCanvas.throwDices.onClick.RemoveAllListeners();
        gameCanvas.nextStep.onClick.RemoveAllListeners();
        gameCanvas.eventOption1.onClick.RemoveAllListeners();
        gameCanvas.eventOption2.onClick.RemoveAllListeners();
    }
}
