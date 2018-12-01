using System;
using UnityEngine;

public class Place : MonoBehaviour
{
    public static int BaseDicePoint(ScriptablePlace.Type placeType)
    {
        switch (placeType)
        {
            case ScriptablePlace.Type.Woods:
                return 1;
            case ScriptablePlace.Type.Mines:
                return 2;
            case ScriptablePlace.Type.Lake:
                return 2;
            case ScriptablePlace.Type.BuildersHut:
                return 4;
            case ScriptablePlace.Type.House:
                return 4;
            case ScriptablePlace.Type.MarketPlace:
                return 2;
            case ScriptablePlace.Type.WatchTower:
                return 2;
            default:
                return 100;
        }
    }

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
}