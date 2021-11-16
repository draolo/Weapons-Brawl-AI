using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChestScript : AbstractChest {

    public GameObject Weapon;
    void Awake()
    {
        type = ChestType.Upgrade;
    }

    override
    internal bool DoSomething(PlayerChestManager p)
    {
        p.gameObject.GetComponent<PlayerWeaponManager_Inventory>().CmdAddWeapon(Weapon, p.gameObject);
        return true;
    }
}
