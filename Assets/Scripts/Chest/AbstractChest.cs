using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public abstract class AbstractChest : MonoBehaviour, AvailabilityNotificator
{
    public enum ChestType
    {
        Health,
        Revive,
        Upgrade
    }

    public ChestType type;
    public int level;
    public CircleCollider2D playerNextToRay;
    public SpriteRenderer PressRImg;
    private UnityEvent<bool> _availabilityEvent;

    public UnityEvent<bool> AvailabilityEvent
    {
        get
        {
            return _availabilityEvent;
        }
    }

    private void Start()
    {
        if (_availabilityEvent == null)
            _availabilityEvent = new UnityEvent<bool>();
    }

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
                DestroyChest();
    }

    public void DestroyChest()
    {
        Destroy(gameObject);
    }

    public virtual bool IsInteractable(PlayerChestManager p)
    {
        Color team = p.gameObject.GetComponent<PlayerManager>().GetTeam();
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

        bool bol1 = true; //todo is NOT a bot
        bool bol2 = IsInteractable(player.GetComponent<PlayerChestManager>());

        if (bol1 && bol2)
        {
            PressRImg.enabled = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Transform player = collision.transform;

        if (player.CompareTag("Player") == false)
            return;

        bool bol1 = true; // TODO IS NOT A BOT
        bool bol2 = IsInteractable(player.GetComponent<PlayerChestManager>());

        if (bol1 && bol2)
        {
            PressRImg.enabled = false;
        }
    }

    private void OnDestroy()
    {
        _availabilityEvent.Invoke(false);
    }
}