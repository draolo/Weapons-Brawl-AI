using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public PlayerInfo controller;
    public SpriteRenderer sprite;

    public bool isInTurn;
    public List<MonoBehaviour> scriptToDisable;

    public List<MonoBehaviour> AIOnlyScript;

    public bool isABot = false;

    private void Start()
    {
        SetSpriteColor();
    }

    private void SetSpriteColor()
    {
        sprite.color = GetTeam();
    }

    public IEnumerator LockAfterSec(int sec)
    {
        if (!isABot)
        {
            TimerAfterAttackScript.SetTimer(sec);
        }
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
        if (active)
        {
            DisableAIScriptIfNotBot();
        }
        else
        {
            StopOnLanding();
        }
    }

    public void DisableAIScriptIfNotBot()
    {
        if (!isABot)
        {
            foreach (MonoBehaviour c in AIOnlyScript)
            {
                c.enabled = false;
            }
        }
    }

    internal Color GetTeam()
    {
        return controller.team;
    }

    public void PlayerDie()
    {
        controller.status = PlayerInfo.Status.dead;
    }

    public void SetVelocity(float velx, float vely)
    {
        //print("Setting velocity of " + name + "to : velx= " + velx + " vely= " + vely);
        GetComponent<Rigidbody2D>().velocity = new Vector2(velx, vely);
    }

    public void StopOnLanding()
    {
        if (controller.status != PlayerInfo.Status.alive)
        {
            return;
        }
        PlayerMovementOffline movementManager = gameObject.GetComponent<PlayerMovementOffline>();
        if (movementManager.isGrounded)
        {
            SetVelocity(0f, 0f);
        }
        else
        {
            StartCoroutine(WaitLandingAndLock(movementManager, 0.5f));
        }
    }

    public IEnumerator WaitLandingAndLock(PlayerMovementOffline movementManager, float checkingInterval)
    {
        while (movementManager.isGrounded)
        {
            yield return new WaitForSeconds(checkingInterval);
        }
        SetVelocity(0f, 0f);
    }
}