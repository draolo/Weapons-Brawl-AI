﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class AbstractWeaponGeneric : MonoBehaviour {

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
}