using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Controller : MonoBehaviour
{
    public Camera cam;
    NavMeshAgent agent;
    Animator animController;

    Vector3 pos;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animController = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("mouse down");
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("hit " + hit.point);
                pos = hit.point;
                agent.SetDestination(pos);
            }
        }

        AnimateMove(Vector3.Distance(transform.position, agent.destination) > 1f);
    }

    void AnimateMove(bool moving)
    {
        animController.SetBool("Moving", moving);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(pos, 0.1f);

        if (cam != null)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(ray.origin, hit.point);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, hit.point);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(ray.origin, ray.direction * 50);
            }
        }
    }
}
