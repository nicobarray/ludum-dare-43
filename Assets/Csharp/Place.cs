using System;
using UnityEngine;

public class Place : MonoBehaviour
{
    public static string PlaceToString(ScriptablePlace.Type placeType)
    {
        switch (placeType)
        {
            case ScriptablePlace.Type.Woods:
                return "<b>Woods</b>: Cut some trees to build structures, pay tributes or trade. Adds <color=#55F>+1 wood</color> at the end of turn.";
            case ScriptablePlace.Type.Mines:
                return "<b>Mines</b>: Pick some rocks to build structures, pay tributes or trade. Adds <color=#55F>+1 stone</color> at the end of turn.";
            case ScriptablePlace.Type.Lake:
                return "<b>Lake</b>: Fish some food to sustain your people. Adds <color=#55F>+1 food</color> at the end of turn.";
            case ScriptablePlace.Type.BuildersHut:
                return "<b>Builder's Hut</b>";
            case ScriptablePlace.Type.House:
                return "<b>House</b>: Rest your villager at the house to keep them from exhausting. <color=#55F>Debuf dice maluses</color>.";
            case ScriptablePlace.Type.MarketPlace:
                return "<b>Market Place</b>";
            case ScriptablePlace.Type.WatchTower:
                return "<b>Watch Tower</b>: Keep watch of incoming threats. Adds <color=#55F>+1 block point</color> at the end of turn. <i>(Block point are used to protect from villager's loss during the Morning events)</i>";
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