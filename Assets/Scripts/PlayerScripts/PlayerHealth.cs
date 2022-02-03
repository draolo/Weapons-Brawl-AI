using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, AvailabilityNotificator
{
    public int hp = 100;
    public int maxHealth = 100;
    public GameObject healthBar;

    private UnityEvent<bool> _availabilityEvent;

    public UnityEvent<bool> AvailabilityEvent
    {
        get
        {
            return _availabilityEvent;
        }
    }

    private void Awake()
    {
        if (_availabilityEvent == null)
            _availabilityEvent = new UnityEvent<bool>();
    }

    public void TakeDamage(int damage, GameObject fromWho)
    {
        if (hp <= 0)
        {
            return;
        }
        damage = Math.Min(hp, damage);
        hp -= damage;
        hp = Math.Max(0, hp);
        RefreshHealth();
        gameObject.GetComponent<PlayerManager>().ChangeActiveStatus(false);

        PlayerInfo hittedInfo = gameObject.GetComponent<PlayerManager>().controller;
        PlayerInfo hitterInfo = fromWho.GetComponent<PlayerManager>().controller;
        if (hittedInfo.team == hitterInfo.team)
        {
            if (hitterInfo != hittedInfo)
            {
                fromWho.GetComponent<PlayerManager>().ChangeActiveStatus(false);
                hitterInfo.damageToAlly += damage;
            }
        }
        else
        {
            hitterInfo.damageToEnemy += damage;
        }

        if (hp <= 0)
        {
            PlayerDie();

            hittedInfo.deaths += 1;
            if (hittedInfo.team == hitterInfo.team)
            {
                if (hitterInfo != hittedInfo)
                {
                    hitterInfo.allyEliminated += 1;
                }
            }
            else
            {
                hitterInfo.kills += 1;
            }
        }
    }

    public void GetLife(int life)
    {
        if (hp <= 0)
        {
            _availabilityEvent.Invoke(true);
        }
        hp += life;
        hp = Math.Min(hp, maxHealth);
        RefreshHealth();
    }

    private void RefreshHealth()
    {
        healthBar.GetComponent<HealthBarScript>().SetHealth(hp);
    }

    private void PlayerDie()
    {
        this.gameObject.GetComponent<PlayerManager>().ChangeActiveStatus(false);
        this.gameObject.SetActive(false);
        this.gameObject.transform.position = new Vector3(0, 0);
        gameObject.GetComponent<PlayerManager>().PlayerDie();
        _availabilityEvent.Invoke(false);
    }
}