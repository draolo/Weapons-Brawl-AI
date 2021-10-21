using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerChestManager : NetworkBehaviour {

    [SyncVar]
    public string allyToResurrect;
    public int InteractionRadius = 3;
    public bool waitingUser=false;

    public bool interactionStart=false;

    void Start () {
        waitingUser = false;
        interactionStart = false;
	}
	
	void Update () {
        if (hasAuthority)
        {
            if (Input.GetButtonDown("Chest"))
            {
                RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, InteractionRadius, new Vector2(0, 0));
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider != null && hit.collider.tag == "Chest")
                    {
                        hit.collider.gameObject.GetComponent<AbstractChest>().ClientPreInteract(this);
                        interactionStart = true;
                    }
                }
                
            }
            if(interactionStart && !waitingUser)
            {
                CmdInteract();
                interactionStart = false;
            }
        }
	}

    public void AbortInteraction()
    {
        interactionStart = false;
    }

    [Command]
    private void CmdInteract()
    {

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, InteractionRadius, new Vector2(0, 0));
        foreach(RaycastHit2D hit in hits){
            if (hit.collider != null && hit.collider.tag == "Chest")
            {
                hit.collider.gameObject.GetComponent<AbstractChest>().Interact(this);
            }
        }
        
    }

    [Command]
    public void CmdSetAllyToResurrect(string name)
    {
        allyToResurrect = name;
    }

    public void SelectAllyToResurrect(string name)
    {
        CmdSetAllyToResurrect(name);
        waitingUser = false;
    }

    public void LifeChest(int life)
    {
        gameObject.GetComponent<PlayerHealth>().CmdGetLife(life);
    }






}
