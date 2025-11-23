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

    public static GameObject FloatingText(string value, GameObject parentGameobject, GameObject textObject = null)
    {
        TMP_Text text = null;
        if (textObject == null)
        {
            textObject = new GameObject("Debug Floating Text", typeof(TMP_Text));
            textObject.transform.position = parentGameobject.transform.position + new Vector3(0, 1, 0);
            textObject.transform.SetParent(parentGameobject.transform);
        }
        text = textObject.GetComponent<TMP_Text>();
        text.text = value;
        return textObject;
    }
}
