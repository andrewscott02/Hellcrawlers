using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int armour = 5;

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
