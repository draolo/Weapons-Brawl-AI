using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

//List of players in the match
public class MatchManager : NetworkBehaviour
{

    [SyncVar]
    public Color turn;
    [SyncVar]
    public float waiting = 30;
    public float turnDuration = 30;
    public static MatchManager _instance = null;
    public List<PlayerInfo> _players = new List<PlayerInfo>();

    public List<PlayerInfo> RedTeam = new List<PlayerInfo>();
    public List<PlayerInfo> BlueTeam = new List<PlayerInfo>();
    public bool gameIsOver;
    public bool gameIsStart; 

    public void Start()
    {
        gameIsStart = false;
        _instance = this;
        turn = Color.red;
        waiting = turnDuration;
        DontDestroyOnLoad(this.gameObject);


    }


    private void Update()
    {
        if (isServer && FindObjectOfType<Prototype.NetworkLobby.LobbyTopPanel>().isInGame)
        {
            if (RedTeam.Count > 0  && BlueTeam.Count>0 && !gameIsOver) 
            {
                gameIsStart = true;
            }
            try
            {

                if (_players.Count > 0 && AllPlayerHasEnded(this.turn))
                    waiting = 0;
            }
            catch(Exception e)
            {
                UpdateRedAndBlueTeams();
                print("Eccezione prevista, GO ahead, no problem" + e);
            }

            waiting = waiting - Time.deltaTime;

            if (waiting < 0)
            {
                if (AllPlayerIsDead(turn) && gameIsStart)
                {
                    bool redWin = false;
                    if (turn != Color.red)
                        redWin = true;
                    RpcNotifyGameIsOver(redWin);
                }

                UpdateRedAndBlueTeams();
                waiting = turnDuration;
                ChangeTurn();
                RpcChangeTurn(turn);
            }
        }

    }

    [ClientRpc]
    private void RpcNotifyGameIsOver(bool redWin)
    {
        gameIsOver = true;
        EndGameScreemUI endScreen = FindObjectOfType<EndGameScreemUI>();
        PlayerInfo localPlayer = endScreen.localPlayer;
        if (redWin)
        {
            if (localPlayer.team == Color.red)
                localPlayer.win = true;
            else
                localPlayer.win = false;
        }
        else
        {
            if(localPlayer.team == Color.blue)
                localPlayer.win = true;
            else
                localPlayer.win = false;
        }

        if (!endScreen.isActive)
            endScreen.Open();
        gameIsStart = false;
    }

    private bool AllPlayerIsDead(Color turn)
    {
        bool result = true;
        List<PlayerInfo> CurrentTeam;

        if (turn == Color.red)
            CurrentTeam = RedTeam;
        else
            CurrentTeam = BlueTeam;

        foreach (PlayerInfo player in CurrentTeam)
            if (player.status == PlayerInfo.Status.alive)
                result = false;

        return result;
    }

    private bool AllPlayerHasEnded(Color turn)
    {
        bool result = true;
        List<PlayerInfo> CurrentTeam;

        if (turn == Color.red)
            CurrentTeam = RedTeam;
        else
            CurrentTeam = BlueTeam;

        foreach (PlayerInfo player in CurrentTeam)
        {
            if (player.physicalPlayer.GetComponent<PlayerManager>().isInTurn && player.status == PlayerInfo.Status.alive)
                result = false;
        }

        return result;
    }

    //TODO: very simple just for the prototype
    [Server]
    private void ChangeTurn()
    {
        if (turn == Color.red)
        {
            turn = Color.blue;
        }
        else
        {
            turn = Color.red;
        }

    }

    public void AddPlayer(PlayerInfo player)
    {
        if (_players.Contains(player))
            return;

        _players.Add(player);
        UpdateRedAndBlueTeams();
    }

    public void RemovePlayer(PlayerInfo player)
    {
        _players.Remove(player);
        UpdateRedAndBlueTeams();
    }

    public int PlayerAliveNumber()
    {
        int alivePlayer = 0;
        foreach (PlayerInfo p in _players)
        {
            if (p != null && p.status == PlayerInfo.Status.alive)
            {
                alivePlayer++;
            }
        }
        return alivePlayer;
    }

    public int PlayerAliveNumber(Color team)
    {
        int alivePlayer = 0;
        foreach (PlayerInfo p in _players)
        {
            if (p != null && p.status == PlayerInfo.Status.alive && p.team == team)
            {
                alivePlayer++;
            }
        }
        return alivePlayer;
    }

    public int PlayerDeadNumber()
    {
        int alivePlayer = 0;
        foreach (PlayerInfo p in _players)
        {
            if (p != null && p.status == PlayerInfo.Status.dead)
            {
                alivePlayer++;
            }
        }
        return alivePlayer;
    }

    public int PlayerDeadNumber(Color team)
    {
        int alivePlayer = 0;
        foreach (PlayerInfo p in _players)
        {
            if (p != null && p.status == PlayerInfo.Status.dead && p.team == team)
            {
                alivePlayer++;
            }
        }
        return alivePlayer;
    }

    public void Reset()
    {
        _players = new List<PlayerInfo>();
        UpdateRedAndBlueTeams();
    }

    public List<PlayerInfo> DeadPlayerList()
    {
        List<PlayerInfo> dead = new List<PlayerInfo>();
        foreach (PlayerInfo p in _players)
        {
            if (p.status == PlayerInfo.Status.dead)
            {
                dead.Add(p);
            }
        }
        return dead;
    }

    public List<PlayerInfo> DeadPlayerList(Color team)
    {
        List<PlayerInfo> dead = new List<PlayerInfo>();
        foreach (PlayerInfo p in _players)
        {
            if (p.status == PlayerInfo.Status.dead && p.team == team)
            {
                dead.Add(p);
            }
        }
        return dead;
    }



    [ClientRpc]
    void RpcChangeTurn(Color color)
    {
        foreach (PlayerInfo p in _players)
        {
            if (color == p.team)
            {
                SetPlayerTurn(p, true);
            }
            else
            {
                SetPlayerTurn(p, false);
            }

            if (p.hasAuthority && !gameIsOver)
            {
                if(color == p.team)
                    MessageManager.Instance.PlayYourTurnAnimation();
                else
                    MessageManager.Instance.PlayEndTurnAnimation();
            }
        }
    }

    private static void SetPlayerTurn(PlayerInfo p, bool active)
    {
        p.physicalPlayer.GetComponent<PlayerManager>().ChangeActiveStatus(active);
    }

    private void UpdateRedAndBlueTeams()
    {
        RedTeam.Clear();
        BlueTeam.Clear();
        _players.RemoveAll(item => item == null);

        foreach (PlayerInfo player in _players)
        {
            if (player.team == Color.red)
                RedTeam.Add(player);
            else
                BlueTeam.Add(player);
        }
    }

}

