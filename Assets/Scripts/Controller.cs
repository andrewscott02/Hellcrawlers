using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Controller : MonoBehaviour
{
    #region Setup

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

    #endregion

    public void Unselect()
    {
        StopMovement();
        pos = transform.position;
        AnimateMove(false);
        PrepareAction(null);
        controlled = false;
        targetEffect.StopHighlight();
    }

    Vector3 startPos;
    Vector3 endPos;

    // Update is called once per frame
    void Update()
    {
        if (controlled && InputManager.inputAvailable)
        {
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetMouseButtonDown(1))
                PrepareAction(null);

            //While preparing
            if (preparedAction != null)
            {
                #region Preparing Action

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

                #endregion
            }
            else
            {
                #region Not Preparing Action

                //On left click
                if (Input.GetMouseButtonDown(0) && movementLeft > 0)
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

                #endregion
            }
        }
        else
        {
            StopMovement();
        }
        
        bool moving = Vector3.Distance(transform.position, agent.destination) > 1f;
        AnimateMove(moving);

        if (moving)
        {
            endPos = transform.position;
            float moved = Vector3.Distance(startPos, endPos);
            movementLeft -= moved;
            ActionPointsUI.instance.DisplayMovement(movementLeft / maxMovement);

            if (movementLeft <= 0)
            {
                StopMovement();
            }
        }

        startPos = endPos;
    }

    void AnimateMove(bool moving)
    {
        animController.SetBool("Moving", moving);
    }

    Action preparedAction;
    public int actionsLeft { get; private set; } = 0;
    int maxActions = 3;

    public float movementLeft = 0;
    public float maxMovement = 30;

    public void StartTurn()
    {
        endPos = transform.position;
        actionsLeft = maxActions;
        movementLeft = maxMovement;
        ActionPointsUI.instance.DisplayAP(actionsLeft);
        ActionPointsUI.instance.DisplayMovement(movementLeft / maxMovement);
    }

    public void UseAP(int cost)
    {
        actionsLeft -= cost;
        ActionPointsUI.instance.DisplayAP(actionsLeft);
    }

    public void PrepareAction(Action action)
    {
        if (action != null)
            if (action.cost > actionsLeft) return;

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
