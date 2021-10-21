using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerWeaponManager_Inventory : NetworkBehaviour {

    //Inventory Stuff
    public List<AbstractWeaponGeneric> Weapons = new List<AbstractWeaponGeneric>();
    public int currentWeaponID;
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallBack;

    //Weapon Manager Stuff
    public GameObject throwingChargeBar;
    public int AxeSpeed=10;
    public int timeToRepairAfterAttack = 5;
    public bool canAttack = true;
    public bool idleByBuilding = false;

    public BuildingController buildingController;
    private InventoryUI inventoryUI;
    private AbstractWeaponGeneric CurrentWeapon;
    private GameObject Axe;
    private GameObject FirePoint;
    private GameObject Pivot;


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

        Axe = transform.Find("FirePointPivot/Axe").gameObject;
        FirePoint = transform.Find("FirePointPivot/FirePoint").gameObject;
        Pivot = transform.Find("FirePointPivot").gameObject;
        inventoryUI = FindObjectOfType<InventoryUI>();
    }


    protected void Update()
    {
        if (hasAuthority && !idleByBuilding)
        {
            if (canAttack && buildingController.isBuilding == false)
            {
                if (Input.GetButtonDown("Fire1"))
                    throwingChargeBar.SetActive(true);

                if (Input.GetButtonUp("Fire1"))
                {
                    CmdAttack(throwingChargeBar.GetComponent<ThrowingPowerBarScript>().Charge);
                    canAttack = false;
                    StartCoroutine(gameObject.GetComponent<PlayerManager>().LockAfterSec(timeToRepairAfterAttack));
                    throwingChargeBar.SetActive(false);
                }
            }

            if (Input.GetButtonDown("Switch Left"))
            {
                int numberOfWeapons = Weapons.Count;
                int switchTo = currentWeaponID - 1;
                if (switchTo < 0)
                {
                    switchTo += numberOfWeapons;
                }
                CmdSwitchWeapon(switchTo);
            }                
            if (Input.GetButtonDown("Switch Right"))
            {
                int numberOfWeapons = Weapons.Count;
                CmdSwitchWeapon((currentWeaponID+1)%numberOfWeapons);
            }
                
        }

        if (hasAuthority)
        {
            if (buildingController.isBuilding)
                CmdSetActiveWeapon(false);
            else
                CmdSetActiveWeapon(true);
        }

        if (hasAuthority && gameObject.GetComponent<PlayerManager>().isInTurn)
        {
            if (Input.GetButtonDown("Axe"))
            {
                CmdActivateAxe(true);
                StartCoroutine(SwingAxe());
            }
        }
    }

    void ActivateAxe(bool active)
    {
        Axe.SetActive(active);
        FirePoint.SetActive(!active);
    }

    public void SwitchWeapon(int id)
    {
        if(CurrentWeapon)
            CurrentWeapon.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        CurrentWeapon = Weapons[id];
        CurrentWeapon.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        currentWeaponID = id;
    }

    IEnumerator SwingAxe()
    {
        Quaternion previousRotation = Pivot.transform.localRotation;
        Pivot.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        while (Pivot.transform.localRotation.eulerAngles.z <= 90 || Pivot.transform.localRotation.eulerAngles.z > 300)
        {
            Pivot.transform.Rotate(0f, 0f, -10f * AxeSpeed * Time.deltaTime);
            yield return 0;
        }
        CmdActivateAxe(false);
        Pivot.transform.localRotation = previousRotation;
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
        inventoryUI.UpdateUI();
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













    [Command]
    public void CmdSetActiveWeapon(bool active)
    {
        RpcSetActiveWeapon(active);
    }

    [ClientRpc]
    private void RpcSetActiveWeapon(bool active)
    {
        SetActiveWeapon(active);
    }


    [Command]
    public void CmdAddWeapon(GameObject weaponToAdd, GameObject player)
    {
        RpcAddWeapon(weaponToAdd, player);
    }

    [ClientRpc]
    private void RpcAddWeapon(GameObject weaponToAdd, GameObject player)
    {
        AddWeapon(weaponToAdd, player);
    }


    [Command]
    public void CmdAttack(int charge)
    {
        CurrentWeapon.Attack(charge);
    }

    [Command]
    void CmdActivateAxe(bool active)
    {
        RpcActivateAxe(active);
    }

    [ClientRpc]
    void RpcActivateAxe(bool active)
    {
        ActivateAxe(active);
    }



    [Command]
    public void CmdSwitchWeapon(int id)
    {
        RpcSwitchWeapon(id);
    }

    [ClientRpc]
    private void RpcSwitchWeapon(int id)
    {
        SwitchWeapon(id);
    }

    void OnDisable()
    {
        throwingChargeBar.SetActive(false);
    }

    void OnEnable()
    {
        canAttack = true;
    }

}
