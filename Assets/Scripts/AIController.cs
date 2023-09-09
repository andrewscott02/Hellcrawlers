using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : Controller
{
    protected override void Start()
    {
        base.Start();
        agent.destination = transform.position;
    }

    float moveTime = 0;
    float maxMoveTime = 5;

    // Update is called once per frame
    protected override void Update()
    {
        bool moving = false;
        if (agent != null)
            moving = Vector3.Distance(transform.position, agent.destination) > 1f;
        AnimateMove(moving);

        if (target != null && preparedAction != null)
        {
            bool lineOfSight = false;
            RaycastHit perspectiveInfo;
            Vector3 lineOfSightOrigin = transform.position;
            lineOfSightOrigin.y += 2;

            if (Physics.Linecast(lineOfSightOrigin, target.transform.position, out perspectiveInfo))
            {
                if (perspectiveInfo.collider.GetComponentInParent<Health>() != null)
                {
                    Debug.Log("Line of Sight: " + target.gameObject.name + " | " + perspectiveInfo.collider.GetComponentInParent<Health>().gameObject.name);
                    lineOfSight = target.gameObject == perspectiveInfo.collider.GetComponentInParent<Health>().gameObject;
                }
            }

            float distance = Vector3.Distance(transform.position, target.transform.position);
            bool inRange = distance <= preparedAction.range;
            canHit = inRange && lineOfSight;
        }
        else
        {
            canHit = false;
        }

        if (moving)
        {
            moveTime += Time.deltaTime;

            if (moveTime >= maxMoveTime)
            {
                //TODO: Not working - fix
                Debug.Log("No path");
                movementLeft = 0;
            }

            endPos = transform.position;
            float moved = Vector3.Distance(startPos, endPos);
            movementLeft -= moved;
            DisplayValues();

            if (movementLeft <= 0)
            {
                StopMovement();
            }
        }

        startPos = endPos;
    }

    public override void StartTurn()
    {
        base.StartTurn();

        takingTurn = true;

        canHit = false;

        AssessAction();
    }

    public override void StopMovement()
    {
        base.StopMovement();
        moveTime = 0;
    }

    public void AssessAction()
    {
        if (!takingTurn) return;

        StopAllCoroutines();

        //AI considerations
        target = DetermineTarget();
        preparedAction = DetermineAction(target);

        if (preparedAction != null)
            Debug.Log(gameObject.name + " is starting turn with " + movementLeft + " and planning to use " + preparedAction.name + " on " + target);
        else
            Debug.Log(gameObject.name + " is starting turn with no action but moving to " + target);

        targetPos = target.transform.position;
        agent.SetDestination(targetPos);
        StartCoroutine(IDelayAction());
    }

    public bool takingTurn = false;
    Controller target;
    Vector3 targetPos;
    bool canHit = false;

    IEnumerator IDelayAction()
    {
        yield return new WaitUntil(() => canHit || movementLeft <= 0);

        if (canHit)
        {
            //target within range now
            Debug.Log(gameObject.name + " is ending turn using: " + preparedAction.actionName);
            preparedAction.UseAction(this, target.transform.position, target.GetComponent<Health>());
        }
        else
        {
            Debug.Log(gameObject.name + " is ending turn, doing nothing");
            preparedAction = DetermineAction(target);
            bool lineOfSight = false;

            if (preparedAction != null)
            {
                RaycastHit perspectiveInfo;
                Vector3 lineOfSightOrigin = transform.position;
                lineOfSightOrigin.y += 2;

                if (Physics.Linecast(lineOfSightOrigin, target.transform.position, out perspectiveInfo))
                {
                    if (perspectiveInfo.collider.GetComponentInParent<Health>() != null)
                    {
                        Debug.Log("Line of Sight: " + target.gameObject.name + " | " + perspectiveInfo.collider.GetComponentInParent<Health>().gameObject.name);
                        lineOfSight = target.gameObject == perspectiveInfo.collider.GetComponentInParent<Health>().gameObject;
                    }
                }

                float distance = Vector3.Distance(transform.position, target.transform.position);
                bool inRange = distance <= preparedAction.range;
                canHit = inRange && lineOfSight;
                if (canHit)
                {
                    //target within range now
                    Debug.Log(gameObject.name + " is ending turn using: " + preparedAction.actionName);
                    preparedAction.UseAction(this, target.transform.position, target.GetComponent<Health>());
                }
            }
        }

        StopMovement();
        TurnFinished();
        StopAllCoroutines();
    }

    public override void DisplayValues()
    {
        //Do nothing, overrides base behaviour
    }

    Controller DetermineTarget()
    {
        Controller closestCharacter = null;
        float closestDistance = Mathf.Infinity;

        foreach(var item in CharacterSelect.instance.availableCharacters)
        {
            if (item != null)
            {
                float distance = Vector3.Distance(transform.position, item.transform.position);
                if (distance < closestDistance)
                {
                    closestCharacter = item;
                    closestDistance = distance;
                }
            }
        }

        if (closestCharacter != null)
            Debug.Log(closestCharacter.gameObject.name);
        return closestCharacter;
    }

    Action DetermineAction(Controller target)
    {
        if (target == null)
        {
            AssessAction();
            return null;
        }

        Health targetHealth = target.GetComponent<Health>();

        Action bestSpell = null;
        float highestPriority = Mathf.NegativeInfinity;

        foreach(var item in actions)
        {
            float priority = Mathf.NegativeInfinity;
            
            if (Vector3.Distance(transform.position, target.transform.position) < item.range + movementLeft)
            {
                priority = 0;

                if (item.damage >= targetHealth.armour)
                {
                    priority += Mathf.Infinity;
                }
                else
                {
                    priority += item.damage * 0.2f;
                    priority -= item.changeArmour;

                    foreach (var status in item.statuses)
                    {
                        if (!targetHealth.statuses.ContainsKey(status))
                        {
                            Debug.Log("Doesn't contain status " + status.name);
                            priority += 10;
                        }
                    }
                }
            }

            if (priority > highestPriority)
            {
                bestSpell = item;
                highestPriority = priority;
            }
        }

        //TODO:AI for actions
        return bestSpell;
    }

    void TurnFinished()
    {
        takingTurn = false;
        StopMovement();
        EndTurn.instance.EndEnmyTurn();
    }

    private void OnDrawGizmos()
    {
        if (target == null)
        {
            return;
        }

        Vector3 lineOfSightOrigin = gameObject.transform.position;
        lineOfSightOrigin.y += 2;

        Gizmos.color = Color.green;
        Physics.Linecast(transform.position, target.transform.position);
    }
}
