using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class AbstractWeaponGeneric : MonoBehaviour, IComparable<AbstractWeaponGeneric>
{
    protected Transform firePoint;

    public Transform Player;

    public WeaponInfo info;

    protected void Awake()
    {
        if (Player)
            SetPlayer(Player.gameObject);
    }

    public void SetPlayer(GameObject player)
    {
        Player = player.transform;
        firePoint = Player.Find("FirePointPivot/FirePoint");
    }

    public abstract void Attack(int charge);

    public abstract int GetDamage();

    public abstract int GetFling();

    public int CompareTo(AbstractWeaponGeneric other)
    {
        int damageCmp = this.GetDamage().CompareTo(other.GetDamage());
        if (damageCmp == 0)
        {
            return this.GetFling().CompareTo(other.GetFling());
        }
        else
        {
            return damageCmp;
        }
    }
}