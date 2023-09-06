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

    List<StatusEffect> statuses = new List<StatusEffect>();
    public delegate void StatusDelegates(Health target);
    public StatusDelegates OnHit;

    public bool ApplyStatus(StatusEffect status)
    {
        if (statuses.Contains(status)) return false;

        statuses.Add(status);

        return true;
    }

    public void ClearStatuses()
    {
        foreach(var item in statuses)
        {
            item.RemoveStatus(this);
        }

        statuses.Clear();
    }

    public float GetDamageScaling()
    {
        float damageScaling = 1;

        foreach (var item in statuses)
        {
            damageScaling += item.damageScaling;
        }

        return damageScaling;
    }

    #endregion
}
