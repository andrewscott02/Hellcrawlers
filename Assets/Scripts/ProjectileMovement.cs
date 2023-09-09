using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    Vector3 target;
    float speed;
    float distanceAllowance;
    Object impactEffect;

    public void StartMovement(Vector3 target, float speed, float distanceAllowance, Object impactEffect)
    {
        this.target = target;
        this.speed = speed;
        this.distanceAllowance = distanceAllowance;
        this.impactEffect = impactEffect;
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 dir = target - transform.position;
        transform.Translate(dir * speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) <= distanceAllowance)
        {
            Instantiate(impactEffect, transform.position, new Quaternion(0, 0, 0, 0));
            Destroy(this.gameObject);
        }
    }
}
