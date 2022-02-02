using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChestScript : AbstractChest
{
    public GameObject Weapon;

    private void Awake()
    {
        type = ChestType.Upgrade;
    }

    internal override bool DoSomething(PlayerChestManager p)
    {
        p.gameObject.GetComponent<PlayerWeaponManager_Inventory>().AddWeapon(Weapon, p.gameObject);
        return true;
    }
}