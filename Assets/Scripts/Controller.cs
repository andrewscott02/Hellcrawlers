using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Controller : MonoBehaviour
{
    public LayerMask layerMask = new LayerMask();

    public bool controlled = false;
    public Camera cam;
    NavMeshAgent agent;
    Animator animController;
    public Action[] actions;

    Vector3 pos;

    TargetEffect targetEffect;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animController = GetComponentInChildren<Animator>();
        targetEffect = GameObject.FindAnyObjectByType<TargetEffect>();
    }

    public void Unselect()
    {
        StopMovement();
        pos = transform.position;
        AnimateMove(false);
        PrepareAction(null);
        controlled = false;
        targetEffect.StopHighlight();
    }

    // Update is called once per frame
    void Update()
    {
        if (!controlled || InputManager.inputAvailable == false)
        {
            StopMovement();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetMouseButtonDown(1))
            PrepareAction(null);

        #region Preparing Action

        //While preparing
        if (preparedAction != null)
        {
            //Rotate to mouse pos
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                float distance = Vector3.Distance(gameObject.transform.position, hit.point);

                Debug.Log(hit.collider.gameObject.name + " | " + hit.collider.tag);
                if (hit.collider.tag == "Character")
                {
                    distance = Vector3.Distance(gameObject.transform.position, hit.collider.transform.position);
                    Color highlightColour = distance <= preparedAction.range ? Color.green : Color.red;
                    targetEffect.Highlight(hit.collider.transform.position, highlightColour);
                }
                else
                {
                    targetEffect.StopHighlight();
                }

                //Rotate towards mouse pos
                pos = hit.point;

                Vector3 direction = pos - transform.position;
                Quaternion desiredRot = Quaternion.LookRotation(direction);

                desiredRot.x = transform.rotation.x;
                desiredRot.z = transform.rotation.z;

                transform.rotation = desiredRot;

                //On left click
                if (Input.GetMouseButtonDown(0))
                {
                    if (distance <= preparedAction.range)
                    {
                        Health target = hit.collider.tag == "Character" ? hit.collider.GetComponentInParent<Health>() : null;

                        preparedAction.UseAction(this, pos, target);
                        animController.SetTrigger("Attack");
                    }
                    else
                    {
                        Debug.Log("Out of range");
                    }
                }
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

            if (Physics.Raycast(ray, out hit, 100, layerMask))
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
        targetEffect.StopHighlight();
        preparedAction = action;
        animController.SetBool("Aiming", preparedAction != null);
        if (preparedAction != null)
            StopMovement();
    }

    public void StopMovement()
    {
        agent.SetDestination(transform.position);
        AnimateMove(false);
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
