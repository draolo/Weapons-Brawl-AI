using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public abstract class AbstractWeaponBulletBased : AbstractWeaponGeneric
{
    public GameObject bulletPrefab;

    public override void Attack(int charge)
    {
        Shoot(charge);
    }


    public void Shoot(int charge)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        AbstractBulletExplosive bulletManager = bullet.GetComponent<AbstractBulletExplosive>();
        bulletManager.speed *= charge / 100f;
        bulletManager.shootedBy = Player.gameObject;
       
        NetworkServer.Spawn(bullet);
    }



}