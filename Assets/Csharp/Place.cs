using System;
using UnityEngine;

public class Place : MonoBehaviour
{
    public static string PlaceToString(ScriptablePlace.Type placeType)
    {
        switch (placeType)
        {
            case ScriptablePlace.Type.Woods:
                return "Woods";
            case ScriptablePlace.Type.Mines:
                return "Mines";
            case ScriptablePlace.Type.Lake:
                return "Lake";
            case ScriptablePlace.Type.BuildersHut:
                return "Builder's Hut";
            case ScriptablePlace.Type.House:
                return "House";
            case ScriptablePlace.Type.MarketPlace:
                return "Market Place";
            case ScriptablePlace.Type.WatchTower:
                return "Watch Tower";
            default:
                return "Do nothing";
        }
    }

    public ScriptablePlace scriptable;
    public Villager worker = null;

    public void Reset(ScriptablePlace scriptable)
    {
        this.scriptable = scriptable;
        if (scriptable.sprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = scriptable.sprite;
        }
        else if (scriptable.visualPrefab != null)
        {
            GameObject newVisualObject = Instantiate(scriptable.visualPrefab, Vector3.zero, Quaternion.identity);
            newVisualObject.transform.SetParent(transform);
        }
    }
}