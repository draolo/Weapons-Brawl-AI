using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerManager : NetworkBehaviour {
    [SyncVar]
    public GameObject controller;

    [SyncVar]
    public bool isInTurn;
    public List<MonoBehaviour> scriptToDisable;

    private void Start()
    {
        if (hasAuthority)
        {
            InventoryUI inventory = GetGameObjectInRoot("Canvas").GetComponent<InventoryUI>();
            BuildInterfaceUI build = GetGameObjectInRoot("Canvas").GetComponent<BuildInterfaceUI>();
            ResurrectionMenuUI resurrection = GetGameObjectInRoot("Canvas").GetComponent<ResurrectionMenuUI>();
            inventory.InitializeInventoryUI(this.gameObject);
            build.InitializeInventoryUI(this.gameObject);
            resurrection.InizializeInventoryUI(this.gameObject);

            //CmdSetTeam();
        }
        SetSpriteColor();
    }

    void SetSpriteColor()
    {
        GetComponent<SpriteRenderer>().color = GetTeam();
    }
    
    public IEnumerator LockAfterSec(int sec)
    {
        TimerAfterAttackScript.SetTimer(sec);
        yield return new WaitForSeconds(sec);
        ChangeActiveStatus(false);
    }
   
    [ClientRpc]
    public void RpcChangeActiveStatus(bool active)
    {
        ChangeActiveStatus(active);
    }

    public void ChangeActiveStatus(bool active)
    {
        GetComponent<PlayerWeaponManager_Inventory>().canAttack = active;
        CmdActiveInTurn(active);
        isInTurn = active;
        foreach (MonoBehaviour c in scriptToDisable)
        {
            c.enabled = active;
        }
    }

    [Command]
    private void CmdActiveInTurn(bool active)
    {
        isInTurn = active;
    }

    internal Color GetTeam()
    {
        return controller.GetComponent<PlayerInfo>().team;
    }

    private GameObject GetGameObjectInRoot(string objname)
    {
        GameObject[] root = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in root)
            if (obj.name == objname)
                return obj;
        return null;
    }

    public void PlayerDie()
    {
        if (isServer)
        {
            controller.GetComponent<PlayerInfo>().status=PlayerInfo.Status.dead;
        }
        
    }


    private void SetVelocity(float velx, float vely)
    {
        //print("Setting velocity of " + name + "to : velx= " + velx + " vely= " + vely);
        GetComponent<Rigidbody2D>().velocity = new Vector2(velx, vely);
    }

    [Command]
    public void CmdSetVelocity(float velx, float vely)
    {
        RpcSetVelocity(velx, vely);
    }

    [ClientRpc]
    private void RpcSetVelocity(float velx, float vely)
    {
        SetVelocity(velx, vely);
    }
}
