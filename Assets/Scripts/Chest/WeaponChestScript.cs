using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChestScript : AbstractChest {

    public GameObject Weapon;

    override
    internal bool DoSomething(PlayerChestManager p)
    {
        p.gameObject.GetComponent<PlayerWeaponManager_Inventory>().CmdAddWeapon(Weapon, p.gameObject);
        return true;
    }
}
