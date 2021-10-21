using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerHealth : NetworkBehaviour {

    [SyncVar]
    public int hp = 100;
    public int maxHealth = 100;
    public GameObject healthBar;

    [Command]
    public void CmdTakeDamage(int damage, GameObject fromWho)
    {
        damage = Math.Min(hp,damage);
        hp -= damage;
        hp = Math.Max(0, hp);
        CmdRefreshHealth();
        gameObject.GetComponent<PlayerManager>().RpcChangeActiveStatus(false);
        PlayerInfo hittedInfo = gameObject.GetComponent<PlayerManager>().controller.GetComponent<PlayerInfo>();
        PlayerInfo hitterInfo =fromWho.GetComponent<PlayerManager>().controller.GetComponent<PlayerInfo>();
        if (hittedInfo.team == hitterInfo.team)
        {
            if (hitterInfo != hittedInfo)
            {
                fromWho.GetComponent<PlayerManager>().RpcChangeActiveStatus(false);
                hitterInfo.damageToAlly += damage;
            }
        }
        else
        {
            hitterInfo.damageToEnemy += damage;
        }

        if (hp <= 0)
        {
            CmdPlayerDie();
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


    [Command]
    public void CmdGetLife(int life)
    {
        hp += life;
        hp = Math.Min(hp, maxHealth);
        CmdRefreshHealth();
    }



    void CmdRefreshHealth()
    {
        RefreshHealth(hp);
        RpcRefreshHp(hp);
    }

    [ClientRpc]
    void RpcRefreshHp(int health)
    {
        RefreshHealth(health);
    }

    void RefreshHealth(int health)
    {
        healthBar.GetComponent<HealthBarScript>().SetHealth(health);
    }

    [Command]
    void CmdPlayerDie()
    {
        PlayerDie();
        gameObject.GetComponent<PlayerManager>().PlayerDie();
        RpcPlayerDie();
    }

    [ClientRpc]
    void RpcPlayerDie()
    {
       PlayerDie();
    }

    void PlayerDie()
    {
        this.gameObject.SetActive(false);
        this.gameObject.transform.position = new Vector3(0,0);
    }
}
