using System;
using UnityEngine;

public class Place : MonoBehaviour
{
    public static int BaseDicePoint(Type placeType)
    {
        switch (placeType)
        {
            case Type.Woods:
                return 1;
            case Type.Mines:
                return 2;
            case Type.Lake:
                return 2;
            case Type.BuildersHut:
                return 4;
            case Type.House:
                return 4;
            case Type.MarketPlace:
                return 2;
            case Type.WatchTower:
                return 2;
            default:
                return 100;
        }
    }

    public static string PlaceToString(Type placeType)
    {
        switch (placeType)
        {
            case Type.Woods:
                return "Woods";
            case Type.Mines:
                return "Mines";
            case Type.Lake:
                return "Lake";
            case Type.BuildersHut:
                return "Builder's Hut";
            case Type.House:
                return "House";
            case Type.MarketPlace:
                return "Market Place";
            case Type.WatchTower:
                return "Watch Tower";
            default:
                return "Do nothing";
        }
    }

    public enum Type
    {
        Woods,
        Mines,
        Lake,
        BuildersHut,
        House,
        MarketPlace,
        WatchTower,
    }

    public Type type;
}