using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPosition : MonoBehaviour
{
    public LayerMask terrainLayer = default;
    public Transform root = default;
    public FootPosition otherFoot = default;
    public float speed = 1;
    public float stepDistance = 4;
    public float stepLength = 4;
    public float stepHeight = 1;
    public Vector3 footOffset = default;
    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;

    private void Start()
    {
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1;
    }

    // Update is called once per frame

    void Update()
    {
        transform.position = currentPosition;
        transform.up = currentNormal;

        Ray ray = new Ray(root.position + (root.right * footSpacing), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer.value))
        {
            if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = root.InverseTransformPoint(info.point).z > root.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = info.point + (root.forward * stepLength * direction) + footOffset;
                newNormal = info.normal;
            }
        }

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

    private void OnDrawGizmos()
    {
        Ray ray = new Ray(root.position + (root.right * footSpacing), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer.value))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(root.position + (root.right * footSpacing), info.point);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(root.position + (root.right * footSpacing), root.position + (root.right * footSpacing) + (Vector3.down * 5));
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.1f);
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }
}