using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int hp = 100;
    public int maxHealth = 100;
    public GameObject healthBar;

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
    }
}