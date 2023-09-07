using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Controller
{
    List<Controller> playerCharacters = new List<Controller>();

    protected override void Start()
    {
        base.Start();
        agent.destination = transform.position;

        foreach (var item in GameObject.FindObjectsOfType<Controller>())
        {
            if (item is AIController == false)
                playerCharacters.Add(item);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        bool moving = false;
        if (agent != null)
            moving = Vector3.Distance(transform.position, agent.destination) > 1f;
        AnimateMove(moving);

        if (moving)
        {
            distance = Vector3.Distance(transform.position, targetPos);
            endPos = transform.position;
            float moved = Vector3.Distance(startPos, endPos);
            movementLeft -= moved;
            DisplayValues();

            if (movementLeft <= 0)
            {
                StopMovement();
                StopAllCoroutines();
            }
        }

        startPos = endPos;
    }

    public override void StartTurn()
    {
        base.StartTurn();

        //TODO: AI considerations here
        Controller target = DetermineTarget();
        preparedAction = DetermineAction(target);

        if (Vector3.Distance(transform.position, target.transform.position) < preparedAction.range)
        {
            //target within range now
            preparedAction.UseAction(this, target.transform.position, target.GetComponent<Health>());
            TurnFinished();
        }
        else
        {
            //movement gets within range
            targetPos = target.transform.position;
            agent.SetDestination(targetPos);
            StartCoroutine(IDelayAction(preparedAction, target));
        }
    }

    Vector3 targetPos;
    float distance = 0;

    IEnumerator IDelayAction(Action action, Controller target)
    {
        yield return new WaitUntil(() => (Vector3.Distance(transform.position, target.transform.position) < action.range) || movementLeft <= 0);
        if (Vector3.Distance(transform.position, target.transform.position) < action.range)
        {
            //target within range now
            action.UseAction(this, target.transform.position, target.GetComponent<Health>());
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

        foreach(var item in playerCharacters)
        {
            float distance = Vector3.Distance(transform.position, item.transform.position);
            if (distance < closestDistance)
            {
                closestCharacter = item;
                closestDistance = distance;
            }
        }

        Debug.Log(closestCharacter.gameObject.name);
        return closestCharacter;
    }

    Action DetermineAction(Controller target)
    {
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
        Debug.Log(bestSpell.actionName);
        return bestSpell;
    }

    void TurnFinished()
    {
        EndTurn.instance.EndEnmyTurn();
    }
}
