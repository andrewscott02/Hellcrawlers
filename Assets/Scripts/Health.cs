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

    public void Hit(int damage, int changeArmour = 0)
    {
        armour += changeArmour;
        if (damage >= armour)
            Kill();
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
