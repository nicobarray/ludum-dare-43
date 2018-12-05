using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour
{
    public TMPro.TextMeshProUGUI tooltipName;
    public EventSystem eventSystem;
    public GraphicRaycaster raycaster;

    void Raycast(Action<RaycastHit[]> onRaycast)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (hits.Length > 0)
        {
            onRaycast(hits);
        }
    }


    void Update()
    {
        tooltipName.text = "";

        var pointerEventData = new PointerEventData(eventSystem);
        //Set the Pointer Event Position to that of the mouse position
        pointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        raycaster.Raycast(pointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        foreach (RaycastResult result in results)
        {
            var tooltipable = result.gameObject.GetComponent<Tooltipable>();
            if (tooltipable != null)
            {
                tooltipName.text = tooltipable.GetText();
                break;
            }
        }

        if (tooltipName.text != "")
        {
            return;
        }

        Raycast((hits) =>
        {
            foreach (RaycastHit item in hits)
            {
                Tooltipable tooltipable = item.transform.GetComponent<Tooltipable>();

                if (tooltipable != null)
                {
                    tooltipName.text = tooltipable.GetText();
                    break;
                }
            }
        });
    }
}