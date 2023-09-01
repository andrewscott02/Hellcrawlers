using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Controller : MonoBehaviour
{
    public Camera cam;
    NavMeshAgent agent;
    Animator animController;
    public Action defaultAttackAction;

    Vector3 pos;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animController = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            PrepareAction(defaultAttackAction);
        else
            PrepareAction(null);

        #region Preparing Action

        //While preparing
        if (preparedAction != null)
        {
            //Rotate to mouse pos
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Rotate towards mouse pos
                pos = hit.point;

                Vector3 direction = pos - transform.position;
                Quaternion desiredRot = Quaternion.LookRotation(direction);

                desiredRot.x = transform.rotation.x;
                desiredRot.z = transform.rotation.z;

                transform.rotation = desiredRot;
            }

            if (Input.GetMouseButtonDown(0))
            {
                //TODO: cast action here
                Debug.Log("Shoot");
            }

            return;
        }

        #endregion

        #region Not Preparing Action

        //On left click
        if (Input.GetMouseButtonDown(0))
        {
            //Raycast from camera to mouse pos in world
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Set unit destination to mouse pos
                pos = hit.point;
                agent.SetDestination(pos);
            }
        }

        AnimateMove(Vector3.Distance(transform.position, agent.destination) > 1f);

        #endregion
    }

    void AnimateMove(bool moving)
    {
        animController.SetBool("Moving", moving);
    }

    Action preparedAction;

    public void PrepareAction(Action action)
    {
        preparedAction = action;
        animController.SetBool("Aiming", preparedAction != null);
        if (preparedAction != null)
            StopMovement();
    }

    public void StopMovement()
    {
        agent.SetDestination(transform.position);
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
