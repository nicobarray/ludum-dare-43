using System;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public TMPro.TextMeshProUGUI tooltipName;

    void Raycast(Action<RaycastHit[]> onRaycast)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (hits.Length > 0)
        {
            onRaycast(hits);
        }
    }

    string GetTooltipText(Place.Type placeType)
    {
        return Place.PlaceToString(placeType) + " (" + Place.BaseDicePoint(placeType) + "dp)";
    }

    void Update()
    {
        tooltipName.text = "";
        Raycast((hits) =>
        {
            Place p = null;
            foreach (var item in hits)
            {
                Place place = item.transform.GetComponent<Place>();

                if (place != null)
                {
                    p = place;
                    break;
                }
            }

            if (p != null)
            {
                tooltipName.text = GetTooltipText(p.type);
            }
        });
    }
}