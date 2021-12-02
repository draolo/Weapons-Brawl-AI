using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerChestManager : MonoBehaviour
{
    public string allyToResurrect;
    public int InteractionRadius = 3;
    public bool waitingUser = false;

    public bool interactionStart = false;
    private bool bot;

    private void Start()
    {
        waitingUser = false;
        interactionStart = false;
        bot = gameObject.GetComponent<PlayerManager>().isABot;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Chest"))
        {
            TryToOpenChest();
        }

        if (interactionStart && (!waitingUser || bot))
        {
            Interact();
            interactionStart = false;
        }
    }

    public void TryToOpenChest()
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

    public void AbortInteraction()
    {
        interactionStart = false;
    }

    private void Interact()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, InteractionRadius, new Vector2(0, 0));
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.tag == "Chest")
            {
                hit.collider.gameObject.GetComponent<AbstractChest>().Interact(this);
            }
        }
    }

    public void SetAllyToResurrect(string name)
    {
        allyToResurrect = name;
    }

    public void SetAllyToResurrectBot(string name)
    {
        SetAllyToResurrect(name);
        waitingUser = false;
    }

    public void LifeChest(int life)
    {
        gameObject.GetComponent<PlayerHealth>().GetLife(life);
    }
}