using UnityEngine;

[CreateAssetMenu(menuName = "LD43/Places", fileName = "Place", order = 0)]
public class ScriptablePlace : ScriptableObject
{
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
    public GameEffect effect;
    public int diceCost;

    public Sprite sprite;
    public GameObject visualPrefab;
}
