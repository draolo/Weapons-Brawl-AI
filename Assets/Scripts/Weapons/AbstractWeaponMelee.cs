using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class AbstractWeaponMelee : AbstractWeaponGeneric
{
    public float attackRange;
    public int damagePower;
    public LayerMask PlayerLayer;
    public int FlingIntensity;

    public override int GetDamage()
    {
        return damagePower;
    }

    public override int GetFling()
    {
        return FlingIntensity;
    }
}