using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugHelper
{
    public static void ShowDebugRays(Vector3 position, Vector3[] directions)
    {
        Color[] rayColors = new Color[]
        {
            Color.cyan,
            Color.green,
            Color.red,
            Color.yellow,
            Color.black
        };

        for (int i = 0; i < directions.Length; i++)
        {
            float rayDist = (directions.Length - i + 2);
            Debug.DrawRay(
                position,
                directions[i].normalized * rayDist,
                rayColors[i % rayColors.Length]
            );
        }
    }
}
