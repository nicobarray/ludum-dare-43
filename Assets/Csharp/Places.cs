using System.Collections.Generic;
using UnityEngine;

public class Places : MonoBehaviour
{
    public GameObject placePrefab;

    public List<Place> places;

    public void AddPlace()
    {
        GameObject newPlace = Instantiate(
            placePrefab,
            Vector3.zero,
            Quaternion.identity
        );

        newPlace.transform.SetParent(transform);

        Place place = newPlace.GetComponent<Place>();
        places.Add(place);

        // places[places.Count - 1].Stable();

        ReorderPlaces();
    }

    void ReorderPlaces()
    {
        for (int i = 0; i < places.Count; i++)
        {
            places[i].transform.localPosition = new Vector3(transform.position.x + i * Mathf.Sqrt(2), 0, 0);
            places[i].name = "Place_" + i;
        }
    }

    public void RemovePlace(Place place)
    {
        Destroy(place.gameObject);
        places.Remove(place);
        ReorderPlaces();
    }
}