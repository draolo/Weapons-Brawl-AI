using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {
    public GameObject controller;
    public GameObject spriteObject;

    public bool isInTurn;
    public List<MonoBehaviour> scriptToDisable;

    private void Start()
    {

        SetSpriteColor();
    }

    void SetSpriteColor()
    {

        spriteObject.GetComponent<SpriteRenderer>().color = GetTeam();
    }
    
    public IEnumerator LockAfterSec(int sec)
    {
        TimerAfterAttackScript.SetTimer(sec);
        yield return new WaitForSeconds(sec);
        ChangeActiveStatus(false);
    }



    public void ChangeActiveStatus(bool active)
    {
        Debug.Log("Status fucked up");
        GetComponent<PlayerWeaponManager_Inventory>().canAttack = active;
        CmdActiveInTurn(active);
        isInTurn = active;
        foreach (MonoBehaviour c in scriptToDisable)
        {
            c.enabled = active;
        }
    }

    private void CmdActiveInTurn(bool active)
    {
        isInTurn = active;
    }

    internal Color GetTeam()
    {
        return Color.white;
        // TODO REAL GET COLOR
        //return controller.GetComponent<PlayerInfo>().team;
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

            controller.GetComponent<PlayerInfo>().status=PlayerInfo.Status.dead;

        
    }


    private void SetVelocity(float velx, float vely)
    {
        //print("Setting velocity of " + name + "to : velx= " + velx + " vely= " + vely);
        GetComponent<Rigidbody2D>().velocity = new Vector2(velx, vely);
    }

    public void CmdSetVelocity(float velx, float vely)
    {
        RpcSetVelocity(velx, vely);
    }

    private void RpcSetVelocity(float velx, float vely)
    {
        SetVelocity(velx, vely);
    }
}
