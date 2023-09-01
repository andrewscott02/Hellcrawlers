using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandPosition : MonoBehaviour
{
    public Transform target;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.position = LerpVector(transform.position, target.position, speed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + target.position, 0.1f);
    }

    Vector3 LerpVector(Vector3 a, Vector3 b, float t)
    {
        Vector3 lerpPos = new Vector3();

        lerpPos.x = Mathf.Lerp(a.x, b.x, t);
        lerpPos.y = Mathf.Lerp(a.y, b.y, t);
        lerpPos.z = Mathf.Lerp(a.z, b.z, t);

        return lerpPos;
    }
}
