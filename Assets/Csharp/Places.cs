using System.Collections.Generic;
using UnityEngine;

public class Places : MonoBehaviour
{
    public GameObject placePrefab;
    public List<ScriptablePlace> scriptables;

    public List<Place> places;

    public void AddRandomPlace()
    {
        AddPlace(scriptables[Random.Range(0, scriptables.Count)]);
    }

    public void AddPlace(ScriptablePlace scriptable)
    {
        GameObject newPlace = Instantiate(
            placePrefab,
            Vector3.zero,
            Quaternion.identity
        );

        newPlace.transform.SetParent(transform);

        Place place = newPlace.GetComponent<Place>();
        place.Reset(scriptable);
        places.Add(place);

        // places[places.Count - 1].Stable();

        ReorderPlaces();
    }

    void ReorderPlaces()
    {
        for (int i = 0; i < places.Count; i++)
        {
            places[i].transform.localPosition = new Vector3(i * 2f, 0, 0);
            places[i].name = "Place_" + i;
        }
    }

    public void RemovePlace(Place place)
    {
        Destroy(place.gameObject);
        places.Remove(place);
        ReorderPlaces();
    }

    public void CollectWork()
    {
        places.ForEach(p =>
        {
            if (p.worker != null)
            {
                Game.instance.CollectResourceFrom(p);
                p.worker = null;
            }
        });
    }
}