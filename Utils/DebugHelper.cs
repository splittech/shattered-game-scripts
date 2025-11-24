using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
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

    public static void LogWithObject(GameObject gameObject, string name, string value)
    {
        Debug.Log(gameObject.name + " => " + name + ": " + value);
    }

    public static TMP_Text FloatingText(string value, GameObject parentGameobject, TMP_Text textObject, GameObject floatingTextPrefab)
    {
        if (textObject == null)
        {
            GameObject canvas = Object.Instantiate(floatingTextPrefab, parentGameobject.transform);
            canvas.transform.position = parentGameobject.transform.position + new Vector3(0, 1, 0);
            textObject = canvas.GetComponentInChildren<TMP_Text>();
        }
        textObject.text = value;
        return textObject;
    }
}
