using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPointsUI : MonoBehaviour
{
    public static ActionPointsUI instance;
    public Image[] images;

    private void Start()
    {
        instance = this;
        DisplayAP(0);
    }

    public void DisplayAP(int ap)
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = ap > i ? Color.green : Color.red;
        }
    }
}
