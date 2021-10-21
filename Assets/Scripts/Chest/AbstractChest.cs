using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public abstract class AbstractChest : NetworkBehaviour {

    public int level;
    public CircleCollider2D playerNextToRay;
    public SpriteRenderer PressRImg;

    internal abstract bool DoSomething(PlayerChestManager p);

    public virtual void ClientPreInteract(PlayerChestManager p)
    {
        p.waitingUser = false;
        return;
    }

    public void Interact(PlayerChestManager p)
    {
        if (IsInteractable(p))
            if (DoSomething(p))
                RpcDestroy();
            
    }

    [ClientRpc]
    public void RpcDestroy()
    {
        Destroy(gameObject);
    }

    public virtual bool IsInteractable(PlayerChestManager p)
    {
        Color team= p.gameObject.GetComponent<PlayerManager>().GetTeam();
        if (NumberOfPlayerIntheRay(team) >= level)
        {
            return true;
        }
        return false;
    }

    private int NumberOfPlayerIntheRay(Color team)
    {
        return playerNextToRay.GetComponent<PlayerCounter>().GetPlayerCounter(team);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Transform player = collision.transform;


        if (player.CompareTag("Player") == false)
            return;

        bool bol1 = player.GetComponent<NetworkIdentity>().hasAuthority;
        bool bol2 = IsInteractable(player.GetComponent<PlayerChestManager>());

        if ( bol1 && bol2 )
        {
            PressRImg.enabled = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        Transform player = collision.transform;

        if (player.CompareTag("Player") == false)
            return;

        bool bol1 = player.GetComponent<NetworkIdentity>().hasAuthority;
        bool bol2 = IsInteractable(player.GetComponent<PlayerChestManager>());

        if (bol1 && bol2)
        {
            PressRImg.enabled = false;
        }
    }

}
