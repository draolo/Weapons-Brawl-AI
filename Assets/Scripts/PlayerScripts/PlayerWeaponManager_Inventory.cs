﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerWeaponManager_Inventory : MonoBehaviour
{
    //Inventory Stuff
    public List<AbstractWeaponGeneric> Weapons = new List<AbstractWeaponGeneric>();

    public int currentWeaponID;

    public delegate void OnItemChanged();

    public OnItemChanged onItemChangedCallBack;
    public bool _inturn = true;

    //Weapon Manager Stuff
    public GameObject throwingChargeBar;

    public int AxeSpeed = 10;
    public int timeToRepairAfterAttack = 5;
    public bool canAttack = true;

    private InventoryUI inventoryUI;
    private AbstractWeaponGeneric CurrentWeapon;
    private GameObject Axe;
    private GameObject FirePoint;
    private GameObject Pivot;
    private bool isABot;

    public void Add(AbstractWeaponGeneric weapon)
    {
        Weapons.Add(weapon);
        if (onItemChangedCallBack != null)
            onItemChangedCallBack.Invoke();
    }

    private void Start()
    {
        CmdSwitchWeapon(0);
        SwitchWeapon(0);
        throwingChargeBar.SetActive(false);

        FirePoint = transform.Find("FirePointPivot/FirePoint").gameObject;
        Pivot = transform.Find("FirePointPivot").gameObject;
        inventoryUI = FindObjectOfType<InventoryUI>();
        isABot = GetComponent<PlayerManager>().isABot;
    }

    protected void Update()
    {
        CmdSetActiveWeapon(true);
    }

    public void ShowChargeBar()
    {
        if (canAttack)
        {
            throwingChargeBar.GetComponent<ThrowingPowerBarScript>().Charge = 0;
            throwingChargeBar.SetActive(true);
        }
    }

    public void HideBarAndShoot()
    {
        if (canAttack)
        {
            CmdAttack(throwingChargeBar.GetComponent<ThrowingPowerBarScript>().Charge);
            throwingChargeBar.SetActive(false);
        }
    }

    public void PreviousWeapon()
    {
        int numberOfWeapons = Weapons.Count;
        int newWeapon = currentWeaponID - 1;
        newWeapon = newWeapon < 0 ? numberOfWeapons - 1 : newWeapon;
        CmdSwitchWeapon(newWeapon);
    }

    public void NextWeapon()
    {
        int numberOfWeapons = Weapons.Count;
        CmdSwitchWeapon((currentWeaponID + 1) % numberOfWeapons);
    }

    public void SwitchWeapon(int id)
    {
        if (CurrentWeapon)
            CurrentWeapon.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        CurrentWeapon = Weapons[id];
        CurrentWeapon.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        currentWeaponID = id;
    }

    private void AddWeapon(GameObject weaponToAdd, GameObject player)
    {
        foreach (AbstractWeaponGeneric w in Weapons)
            if (w.name == weaponToAdd.name)
                return;

        GameObject localFirePoint = player.transform.Find("FirePointPivot/FirePoint").gameObject;
        GameObject localPivot = player.transform.Find("FirePointPivot").gameObject;
        weaponToAdd.transform.SetParent(localFirePoint.transform);
        weaponToAdd.transform.localScale = Vector3.one;
        weaponToAdd.transform.rotation = localPivot.transform.parent.gameObject.transform.rotation;
        weaponToAdd.transform.position = localFirePoint.transform.position;
        weaponToAdd.GetComponent<AbstractWeaponGeneric>().SetPlayer(player);

        Weapons.Add(weaponToAdd.GetComponent<AbstractWeaponGeneric>());
    }

    public GameObject GetCurrentWeapon()
    {
        return CurrentWeapon.gameObject;
    }

    private void SetActiveWeapon(bool active)
    {
        if (CurrentWeapon)
            CurrentWeapon.gameObject.SetActive(active);
    }

    public void CmdSetActiveWeapon(bool active)
    {
        RpcSetActiveWeapon(active);
    }

    private void RpcSetActiveWeapon(bool active)
    {
        SetActiveWeapon(active);
    }

    public void CmdAddWeapon(GameObject weaponToAdd, GameObject player)
    {
        RpcAddWeapon(weaponToAdd, player);
    }

    private void RpcAddWeapon(GameObject weaponToAdd, GameObject player)
    {
        AddWeapon(weaponToAdd, player);
    }

    public void CmdAttack(int charge)
    {
        if (!canAttack)
        {
            return;
        }
        CurrentWeapon.Attack(charge);
        canAttack = false;
        StartCoroutine(gameObject.GetComponent<PlayerManager>().LockAfterSec(timeToRepairAfterAttack));
    }

    public void CmdSwitchWeapon(int id)
    {
        RpcSwitchWeapon(id);
    }

    private void RpcSwitchWeapon(int id)
    {
        SwitchWeapon(id);
    }

    private void OnDisable()
    {
        throwingChargeBar.SetActive(false);
    }
}