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
    public SFXManager sfxManager;
    public Dice3DBox dice3DBox;
    public Transform placesOrigin;
    public Transform villagersOrigin;

    [Header("Game data (Read only)")]
    public TurnSteps currentStep = TurnSteps.Upkeep;
    public int gameTurn = 0;
    public bool nextStep = false;
    public int actionPoint = 0;

    public int dicesCount = 2;
    public int villagersCount = 1;
    public int placesCount = 3;
    public int wood = 0;
    public int meals = 0;
    public int stone = 0;
    public int shield = 0;

    public int TURNS_BEFORE_WINTER = 15;

    Places places;
    Villagers villagers;

    List<GameEvent> selectedEvents = new List<GameEvent>();
    GameEvent[] currentSelection = new GameEvent[2];
    List<GameEvent> repeatableEvents = new List<GameEvent>();

    List<ScriptableEventPair> eventPairSequence = new List<ScriptableEventPair>();

    public void CollectResourceFrom(Place p)
    {
        ApplyEffect(p.scriptable.effect);
    }

    public void RemoveDicePoints(int points)
    {
        actionPoint -= points;
        if (actionPoint < 0)
        {
            actionPoint = 0;
        }

        gameCanvas.resourceTexts.dice.text = actionPoint.ToString();
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

        actionPoint = 0;

        gameCanvas.resourceTexts.villager.text = villagersCount.ToString();
        gameCanvas.resourceTexts.dice.text = "0";
        gameCanvas.resourceTexts.food.text = meals.ToString();
        gameCanvas.resourceTexts.wood.text = wood.ToString();
        gameCanvas.resourceTexts.stone.text = stone.ToString();
        gameCanvas.resourceTexts.shield.text = shield.ToString();

        if (eventPairs.Length == 0)
        {
            NextStep();
        }

        if (eventPairSequence.Count == 0)
        {
            eventPairSequence.AddRange(eventPairs);
            for (int i = 0; i < 5; i++)
            {
                eventPairSequence.Sort(new Comparison<ScriptableEventPair>((a, b) => UnityEngine.Random.Range(-1, 1)));
            }
        }

        ScriptableEventPair options = eventPairSequence[0];
        eventPairSequence.RemoveAt(0);

        currentSelection[0] = options.option1;
        gameCanvas.eventOption1Image.sprite = currentSelection[0].image;
        gameCanvas.eventOption1Text.text = currentSelection[0].GetText();

        currentSelection[1] = options.option2;
        gameCanvas.eventOption2Image.sprite = currentSelection[1].image;
        gameCanvas.eventOption2Text.text = currentSelection[1].GetText();

        gameCanvas.eventFrame.SetActive(true);

        // Apply repeatable effects.
        for (int i = repeatableEvents.Count - 1; i >= 0; i--)
        {
            GameEvent ev = repeatableEvents[i];

            Game.instance.gameCanvas.effectLog.AddEvent(ev);

            if (ev.turns <= 0)
            {
                repeatableEvents.RemoveAt(i);
            }
        }
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
            actionPoint = total;
            gameCanvas.resourceTexts.dice.text = actionPoint.ToString();
            gameCanvas.PopFloatingText(gameCanvas.resourceTexts.dice.transform, actionPoint);

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
        dice3DBox.Reset();
        dice3DBox.gameObject.SetActive(false);

        gameCanvas.ChangeStep(115);
        gameCanvas.nextStep.gameObject.SetActive(false);

        villagers.Reset();
        places.Reset();

        StartCoroutine(Utils.Chain(new List<IEnumerator>() {
            gameCanvas.effectLog.ApplyEffectsAsync(),
            villagers.EatAsync(),
            WaitX(1.5f, NextStep)
        }));

    }

    void CheckForVictoryOrDefeat()
    {
        Debug.Log("villagers.villagers.Count: " + villagers.villagers.Count);
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
        GameEvent ev = currentSelection[which];

        if (ev.turns > 1)
        {
            // The event applies at the end of the turn.
            repeatableEvents.Add(ev);
            gameCanvas.effectLog.AddEvent(ev);
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

    public void ApplyEffect(GameEffect effect, object target = null)
    {
        ApplyEffect(effect.type, effect.value, target);
    }

    public void ApplyEffect(GameEffect.EffectType effectType, int effectValue, object target = null)
    {
        Action<int> updateDices = (value) =>
        {
            dicesCount += value;
            if (dicesCount < 0)
            {
                dicesCount = 0;
            }
        };

        Action<int> updateShield = (value) =>
        {
            shield += value;
            if (shield < 0)
            {
                shield = 0;
            }

            gameCanvas.resourceTexts.shield.text = shield.ToString();
            gameCanvas.PopFloatingText(gameCanvas.resourceTexts.shield.transform, value);
        };

        switch (effectType)
        {
            case GameEffect.EffectType.Wood:
                wood += effectValue;
                if (wood < 0)
                {
                    wood = 0;
                }
                gameCanvas.resourceTexts.wood.text = wood.ToString();
                gameCanvas.PopFloatingText(gameCanvas.resourceTexts.wood.transform, effectValue);
                break;
            case GameEffect.EffectType.Stone:
                stone += effectValue;
                if (stone < 0)
                {
                    stone = 0;
                }
                gameCanvas.resourceTexts.stone.text = stone.ToString();
                gameCanvas.PopFloatingText(gameCanvas.resourceTexts.stone.transform, effectValue);
                break;
            case GameEffect.EffectType.Food:
                meals += effectValue;
                if (meals < 0)
                {
                    meals = 0;
                }
                gameCanvas.resourceTexts.food.text = meals.ToString();
                gameCanvas.PopFloatingText(gameCanvas.resourceTexts.food.transform, effectValue);
                break;
            case GameEffect.EffectType.Dice:
                updateDices(effectValue);

                break;
            case GameEffect.EffectType.Villager:
                int value = effectValue;

                // Shield points take for the villager.
                if (shield > 0 && value < 0)
                {
                    value = effectValue + shield;

                    if (value > 0)
                    {
                        value = 0;
                    }

                    updateShield(effectValue);
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
                    Villager targetVillager = (Villager)target;
                    villagers.RemoveVillager(targetVillager != null ? targetVillager : villagers.villagers[0]);
                }

                while (villagers.villagers.Count < villagersCount)
                {
                    villagers.AddVillager();
                }

                CheckForDefeat();

                updateDices(effectValue);
                break;
            case GameEffect.EffectType.Shield:
                updateShield(effectValue);
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

        for (int i = 0; i < placesCount / 2; i++)
        {
            places.AddPlace(places.scriptables[i]);
        }

        for (int i = placesCount / 2; i < placesCount; i++)
        {
            places.AddRandomPlace();
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
