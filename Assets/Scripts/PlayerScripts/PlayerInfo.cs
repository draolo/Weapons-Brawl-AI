using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInfo : NetworkBehaviour {
    public enum Status {dead, alive};
    [SyncVar]
    public string pname;
    [SyncVar]
    public Color team;
    [SyncVar]
    public Status status;
    [SyncVar]
    public GameObject physicalPlayer;
    [SyncVar]
    public int kills=0;
    [SyncVar]
    public int damageToEnemy = 0;
    [SyncVar]
    public int resurrectedAlly = 0;
    [SyncVar]
    public bool win = false;
    [SyncVar]
    public int deaths = 0;
    [SyncVar]
    public int damageToAlly = 0;
    [SyncVar]
    public int allyEliminated = 0;


    // Use this for initialization
    void Start () {
        MatchManager._instance.AddPlayer(this);
        if (isLocalPlayer)
        {
            FindObjectOfType<EndGameScreemUI>().localPlayer = this;
        }
	}
	

    [Command]
    internal void CmdResurrect(Vector3 position)
    {
        PlayerHealth ph= physicalPlayer.GetComponent<PlayerHealth>();
        ph.CmdGetLife(ph.maxHealth);
        RpcRestoreUser(position.x, position.y, position.z);
    }

    internal string KDRatio()
    {
        if (deaths == 0)
        {
            return kills.ToString() + "*";
        }
        else
        {
            return (kills / deaths).ToString("F2");
        }
    }

    [ClientRpc]
    public void RpcRestoreUser(float x, float y, float z)
    {
        Vector3 pos = new Vector3(x, y, z);
        physicalPlayer.transform.position = pos;
        physicalPlayer.SetActive(true);
    }

    public int GetPoints()
    {
        float points = 0;
        points += 0.5f * damageToEnemy;
        points += 20f * kills;
        points += 20f * resurrectedAlly;
        points -= 0.2f * damageToAlly;
        points -= 20f * deaths;
        points -= 20f * allyEliminated;
        if (win)
            points += 50/2;
        points = Mathf.Max(points, 0);
        return Mathf.CeilToInt(points);       
    }

    public char getRank()
    {
        int points = GetPoints();
        if (points >= 190 / 2)
        {
            return 'S';
        }
        if (points >= (130 / 2))
        {
            return 'A';
        }
        if (points >= (100 / 2))
        {
            return 'B';
        }
        if (points >= (80 / 2))
        {
            return 'C';
        }
        if (points >= (50 / 2))
        {
            return 'D';
        }
        if (points >= (30 / 2))
        {
            return 'E';
        }
        return 'F';
    }
}
