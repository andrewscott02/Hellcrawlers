using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAction", menuName = "Units/Action", order = 0)]
public class Action : ScriptableObject
{
    public Object castFX;

    public void PrepareAction(Controller character)
    {
        character.PrepareAction(this);
    }

    public void CancelAction(Controller character)
    {
        character.PrepareAction(null);
    }

    public void UseAction(Controller character, Vector3 castPos, Controller target = null)
    {
        Debug.Log("Used " + name + " action");

        if (target != null)
        {
            castPos = target.transform.position;
        }

        //TODO: Spawn fx at cast pos
        Instantiate(castFX, castPos, new Quaternion(0, 0, 0, 0));

        //TODO: Apply logic for action

        character.PrepareAction(null);
    }
}
