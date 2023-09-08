using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (cam == null) return;

        //Rotate towards active camera
        Vector3 direction = transform.position - cam.transform.position;
        Quaternion desiredRot = Quaternion.LookRotation(direction);

        if (lockX) desiredRot.x = transform.rotation.x;
        if (lockY) desiredRot.y = transform.rotation.y;
        if (lockZ) desiredRot.z = transform.rotation.z;

        transform.rotation = desiredRot;
    }
}