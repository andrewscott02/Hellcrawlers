using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int baseArmour = 5;
    public int armour { get; private set; } = 5;

    private void Start()
    {
        ResetArmour();
    }

    public void ResetArmour()
    {
        armour = baseArmour;
    }

    public void Hit(int damage, int changeArmour = 0, bool ignoreHit = false)
    {
        armour += changeArmour;
        if (damage >= armour)
            Kill();

        if (ignoreHit) return;

        if (OnHit != null)
        {
            Debug.Log("Call on hit delegates");
            OnHit(this);
        }
        else
        {
            Debug.Log("No delegate");
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    #region Statuses

    Dictionary<StatusEffect, int> statuses = new Dictionary<StatusEffect, int>();
    public delegate void StatusDelegates(Health target);
    public StatusDelegates OnHit;

    public bool ApplyStatus(StatusEffect status)
    {
        if (statuses.ContainsKey(status)) return false;

        statuses.Add(status, 0);

        return true;
    }

    public void ClearStatuses()
    {
        if (statuses == null || statuses.Count <= 0) return;

        Dictionary<StatusEffect, int> newStatuses = new Dictionary<StatusEffect, int>();

        foreach (var item in statuses)
        {
            if (item.Key != null)
            {
                if (item.Key.duration > item.Value)
                {
                    //keep, increase duration
                    newStatuses.Add(item.Key, item.Value+1);
                }
                else
                {
                    item.Key.RemoveStatus(this);
                }
            }
        }

        statuses = newStatuses;
    }

    public float GetDamageScaling()
    {
        float damageScaling = 1;

        foreach (var item in statuses)
        {
            damageScaling += item.Key.damageScaling;
        }

        return damageScaling;
    }

    #endregion
}
