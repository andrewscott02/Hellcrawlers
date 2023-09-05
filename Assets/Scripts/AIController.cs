using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Controller
{
    public List<Controller> playerCharacters;

    // Update is called once per frame
    protected override void Update()
    {
        bool moving = Vector3.Distance(transform.position, agent.destination) > 1f;
        AnimateMove(moving);

        if (moving)
        {
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

        //TODO: AI considerations here
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
                closestCharacter = null;
                closestDistance = distance;
            }
        }

        return closestCharacter;
    }

    Action DetermineAction()
    {
        //TODO:AI for actions
        return actions[0];
    }
}
