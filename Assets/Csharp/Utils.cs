using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static IEnumerator Chain(List<IEnumerator> coroutines, Action then = null)
    {
        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }

        if (then != null)
        {
            then();
        }
    }

    public static void Raycast(Action<RaycastHit[]> onRaycast)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (hits.Length > 0)
        {
            onRaycast(hits);
        }
    }

}