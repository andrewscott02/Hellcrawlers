using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetEffect : MonoBehaviour
{
    Light light;

    private void Start()
    {
        light = GetComponent<Light>();
    }

    public void Highlight(Vector3 position, Color color)
    {
        transform.position = position;
        light.color = color;
        light.enabled = true;
    }

    public void StopHighlight()
    {
        light.enabled = false;
    }
}
