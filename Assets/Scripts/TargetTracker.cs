using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTracker : MonoBehaviour
{
    public GameObject lookAtTarget;
    public LayerMask targetMask;

    public List<GameObject> targets = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == targetMask)
        {
            Debug.Log(other.name);
            targets.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == targetMask)
        {
            targets.Remove(other.gameObject);
        }
    }

    private void Update()
    {
        Vector3 lookPos = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

        foreach (var item in targets)
        {
            if (lookPos.Equals(new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity)) || Vector3.Distance(transform.position, lookPos) > Vector3.Distance(transform.position, item.transform.position))
            {
                lookPos = item.transform.position;
            }
        }

        if (lookPos.Equals(new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity)))
        {
            lookPos = transform.position;
        }

        lookAtTarget.transform.position = lookPos;
    }
}
