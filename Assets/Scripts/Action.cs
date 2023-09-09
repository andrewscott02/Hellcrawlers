using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAction", menuName = "Units/Action", order = 0)]
public class Action : ScriptableObject
{
    public string actionName;
    public bool selfOnly = false;
    public int apCost;
    public float moveCost;
    public bool resetArmour;
    public Object castFX;
    public Object projectileFX;

    public int damage = 0;
    public int armourScaling;
    public int changeArmour = 0;
    public int changeMovement = 0;
    public float range = 20;

    public Animations castAnimation = Animations.Attack;

    public StatusEffect[] statuses;

    [TextArea(3, 10)]
    public string description;

    public void PrepareAction(Controller character)
    {
        character.PrepareAction(this);
    }

    public void CancelAction(Controller character)
    {
        character.PrepareAction(null);
    }

    public void UseAction(Controller character, Vector3 castPos, Health target = null)
    {
        string message = "Used " + name + " action";

        Health casterHealth = character.GetComponent<Health>();

        if (selfOnly)
        {
            target = casterHealth;
            castPos = character.transform.position;
        }

        if (target != null)
        {
            message += " on " + target.gameObject.name;
            castPos = target.transform.position;

            //Apply logic for action
            int trueDamage = (int)((damage + (armourScaling * casterHealth.armour)) * casterHealth.GetDamageScaling());
            message += " for " + trueDamage;
            target.Hit(trueDamage, changeArmour);

            foreach(var item in statuses)
            {
                item.ApplyStatus(target);
            }
        }

        Debug.Log(message);

        //Spawn fx at cast pos
        GameObject projectileObj = Instantiate(projectileFX, character.transform.position + character.castOffset, new Quaternion(0, 0, 0, 0)) as GameObject;
        ProjectileMovement projMovement = projectileObj.GetComponent<ProjectileMovement>();

        if (projMovement != null)
        {
            projMovement.StartMovement(castPos, 30, 1, castFX);
        }
        else
        {
            Destroy(projectileObj);
        }

        //Animation
        if (character.animController != null)
            character.animController.SetTrigger(castAnimation.ToString());

        if (resetArmour)
            character.GetComponent<Health>().ResetArmour();

        character.UseAP(apCost);
        character.UseMovement(moveCost - changeMovement);

        character.PrepareAction(null);
    }
}

public enum Animations
{
    Attack, Cast
}