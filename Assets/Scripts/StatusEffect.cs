using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NeStatus", menuName = "Units/Status", order = 1)]
public class StatusEffect : ScriptableObject
{
    public int armourOnHit;
    public float damageScaling;

    public void ApplyStatus(Health target)
    {
        if (target.ApplyStatus(this))
        {
            target.OnHit += OnHit;
            Debug.Log("Added delegate");
        }
    }

    public void RemoveStatus(Health target)
    {
        target.OnHit -= OnHit;
    }

    public void OnHit(Health target)
    {
        target.Hit(0, armourOnHit, true);
    }
}
