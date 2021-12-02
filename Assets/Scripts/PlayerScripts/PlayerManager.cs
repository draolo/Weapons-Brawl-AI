using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public GameObject controller;
    public GameObject spriteObject;

    public bool isInTurn;
    public List<MonoBehaviour> scriptToDisable;

    public bool isABot = false;

    private void Start()
    {
        SetSpriteColor();
    }

    private void SetSpriteColor()
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
        GetComponent<PlayerWeaponManager_Inventory>().canAttack = active;
        isInTurn = active;
        foreach (MonoBehaviour c in scriptToDisable)
        {
            c.enabled = active;
        }
    }

    internal Color GetTeam()
    {
        return controller.GetComponent<PlayerInfo>().team;
    }

    public void PlayerDie()
    {
        controller.GetComponent<PlayerInfo>().status = PlayerInfo.Status.dead;
    }

    public void SetVelocity(float velx, float vely)
    {
        //print("Setting velocity of " + name + "to : velx= " + velx + " vely= " + vely);
        GetComponent<Rigidbody2D>().velocity = new Vector2(velx, vely);
    }
}