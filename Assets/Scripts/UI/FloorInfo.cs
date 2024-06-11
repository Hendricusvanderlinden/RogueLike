using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorInfo : MonoBehaviour
{
    public Text floorText;
    public Text enemiesLeftText;

    public void SetFloorText(string text)
    {
        floorText.text = text;
    }

    public void SetEnemiesLeftText(string text)
    {
        enemiesLeftText.text = text;
    }
}