using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Controller : MonoBehaviour
{
    #region Setup

    public Vector3 castOffset = new Vector3(0, 2, 0);

    public LayerMask layerMask = new LayerMask();
    public LayerMask obstructions = new LayerMask();

    public bool controlled = false;
    public Camera cam;
    protected NavMeshAgent agent;
    public Animator animController { get; protected set; }
    public Action[] actions;

    protected Vector3 pos;

    protected TargetEffect targetEffect;

    protected Health health;

    protected virtual void Start()
    {
        health = GetComponent<Health>();
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

    protected Vector3 startPos;
    protected Vector3 endPos;

    // Update is called once per frame
    protected virtual void Update()
    {
        if (controlled && InputManager.inputAvailable && EndTurn.playerTurn)
        {
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetMouseButtonDown(1))
                PrepareAction(null);

            //While preparing
            if (preparedAction != null)
            {
                #region Preparing Action

                if (preparedAction.selfOnly)
                {
                    targetEffect.Highlight(transform.position, Color.green);

                    //On left click
                    if (Input.GetMouseButtonDown(0))
                    {
                        preparedAction.UseAction(this, pos, health);
                        animController.SetTrigger("Attack");
                    }
                }
                else
                {
                    //Rotate to mouse pos
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 100, layerMask))
                    {
                        bool lineOfSight = false;
                        RaycastHit perspectiveInfo;
                        Vector3 lineOfSightOrigin = gameObject.transform.position;
                        lineOfSightOrigin.y += 2;

                        if(Physics.Linecast(lineOfSightOrigin, hit.collider.transform.position, out perspectiveInfo))
                        {
                            Debug.Log("Line of Sight: " + hit.collider.gameObject.name + " | " + perspectiveInfo.collider.gameObject.name);
                            lineOfSight = hit.collider.gameObject == perspectiveInfo.collider.gameObject;
                        }

                        float distance = Vector3.Distance(gameObject.transform.position, hit.point);
                        bool inRange = distance <= preparedAction.range;
                        bool canHit = inRange && lineOfSight;

                        Debug.Log(hit.collider.gameObject.name + " | " + hit.collider.tag);
                        if (hit.collider.tag == "Character")
                        {
                            distance = Vector3.Distance(gameObject.transform.position, hit.collider.transform.position);
                            canHit = inRange && lineOfSight;
                            Color highlightColour = canHit ? Color.green : Color.red;
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
                            if (canHit)
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
            UseMovement(moved);
            DisplayValues();

            if (movementLeft <= 0)
            {
                StopMovement();
            }
        }

        startPos = endPos;
    }

    protected void AnimateMove(bool moving)
    {
        if (animController != null)
            animController.SetBool("Moving", moving);
    }

    protected Action preparedAction;
    public int actionsLeft { get; private set; } = 0;
    int maxActions = 3;

    public float movementLeft = 0;
    public float maxMovement = 30;

    public virtual void StartTurn()
    {
        endPos = transform.position;
        actionsLeft = maxActions;
        if (health == null) health = GetComponent<Health>();
        movementLeft = maxMovement * health.GetMovementScaling();
        health.ClearStatuses();
        DisplayValues();
    }

    public void UseAP(int cost)
    {
        actionsLeft -= cost;
        DisplayValues();
    }

    public void UseMovement(float cost)
    {
        movementLeft -= cost;
        movementLeft = movementLeft < 0 ? 0 : movementLeft;
        DisplayValues();
    }

    public virtual void DisplayValues()
    {
        ActionPointsUI.instance.DisplayAP(actionsLeft);
        ActionPointsUI.instance.DisplayMovement(movementLeft / maxMovement);
    }

    public void PrepareAction(Action action)
    {
        if (action != null)
            if (action.apCost > actionsLeft || (action.moveCost > movementLeft && action.moveCost > 0)) return;

        targetEffect.StopHighlight();
        preparedAction = action;
        if (animController != null)
            animController.SetBool("Aiming", preparedAction != null);
        if (preparedAction != null)
            StopMovement();
    }

    public virtual void StopMovement()
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
                Vector3 lineOfSightOrigin = gameObject.transform.position;
                lineOfSightOrigin.y += 2;

                Gizmos.color = Color.red;
                Gizmos.DrawLine(lineOfSightOrigin, hit.collider.transform.position);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(ray.origin, ray.direction * 50);
            }
        }
    }
}
