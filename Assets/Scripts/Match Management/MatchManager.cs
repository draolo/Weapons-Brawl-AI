using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using Random = UnityEngine.Random;

//List of players in the match
public class MatchManager : MonoBehaviour
{
    public Color turn;
    private CameraController activeCamera;
    [SerializeField] private CameraController freeRoamCamera;

    public List<Color> teams = new List<Color>(
    new Color[]{
        Color.red,Color.blue
    });

    public Color blankColor = Color.white;
    public int numberOfTeams;
    public int turnIndex = 0;
    public float waiting = 30;
    public float turnDuration = 30;
    public float inBetweenTime = 5;
    public bool isWaiting;
    public static MatchManager _instance = null;
    public List<PlayerInfo> _players = new List<PlayerInfo>();
    public Color winner;

    public Dictionary<Color, List<PlayerInfo>> teamMembers = new Dictionary<Color, List<PlayerInfo>>();

    public bool gameIsOver;
    public bool gameHasStart;

    private PauseMenuScript UIManager;

    public void Awake()
    {
        gameHasStart = false;
        isWaiting = false;
        numberOfTeams = teams.Count;
        activeCamera = freeRoamCamera;
        turnIndex = Random.Range(0, numberOfTeams);
        UIManager = FindObjectOfType<PauseMenuScript>();
        _instance = this;
        turn = blankColor;
        waiting = 5;
        foreach (Color c in teams)
        {
            teamMembers.Add(c, new List<PlayerInfo>());
        }
    }

    private void Start()
    {
        ChangeTurn();
    }

    private void Update()
    {
        if (teamMembers[teams[0]].Count > 0 && !gameIsOver)
        {
            gameHasStart = true;
        }
        try
        {
            if (AllPlayerHasEnded(this.turn))
            {
                waiting = 0;
            }
        }
        catch (Exception e)
        {
            UpdateTeams();
            print("Eccezione prevista, GO ahead, no problem" + e);
        }

        waiting -= Time.deltaTime;

        if (waiting < 0)
        {
            List<Color> availableTeams = teams.FindAll(c => !AllPlayerAreDead(c));
            if (availableTeams.Count <= 1 && gameHasStart)
            {
                winner = availableTeams.Count > 0 ? availableTeams[0] : blankColor;
                GameOver(winner);
            }
            if (!gameIsOver)
            {
                ChangeTurn();
            }
        }
    }

    private void GameOver(Color Winner)
    {
        gameIsOver = true;
        winner = Winner;
        EndGameScreemUI endScreen = FindObjectOfType<EndGameScreemUI>();
        foreach (PlayerInfo playerInfo in _players)
        {
            playerInfo.win = (playerInfo.team == Winner);
        }

        if (!endScreen.isActive)
            endScreen.ResetPlayerAndOpen();
        gameHasStart = false;
    }

    private bool AllPlayerAreDead(Color team)
    {
        bool result = true;
        List<PlayerInfo> CurrentTeam;

        CurrentTeam = teamMembers[team];

        foreach (PlayerInfo player in CurrentTeam)
            if (player.status == PlayerInfo.Status.alive)
                result = false;

        return result;
    }

    private bool AllPlayerHasEnded(Color turn)
    {
        if (isWaiting)
        {
            return false;
        }
        bool result = true;
        List<PlayerInfo> CurrentTeam;

        CurrentTeam = teamMembers[turn];

        foreach (PlayerInfo player in CurrentTeam)
        {
            if (player.physicalPlayer.GetComponent<PlayerManager>().isInTurn && player.status == PlayerInfo.Status.alive)
                result = false;
        }

        return result;
    }

    private void ChangeTurn()
    {
        UIManager.CloseAllOtherMenu();
        if (isWaiting == false)
        {
            isWaiting = true;

            waiting = inBetweenTime;
            if (!gameIsOver && GameManagerScript._instance.localPlayers.ContainsKey(turn))
            {
                if (GameManagerScript._instance.localPlayers[turn].status == PlayerInfo.Status.alive)
                {
                    MessageManager.Instance.PlayEndTurnAnimation();
                }
            }
            turn = blankColor;
        }
        else
        {
            //GameOver(Color.blue);
            int startIndex = turnIndex;
            do
            {
                turnIndex = ++turnIndex % numberOfTeams;
                turn = teams[turnIndex];
                waiting = turnDuration;
                isWaiting = false;
            } while (AllPlayerAreDead(turn) && turnIndex != startIndex);
            if (!gameIsOver && (GameManagerScript._instance.localPlayers.ContainsKey(turn)) && (GameManagerScript._instance.localPlayers[turn].status == PlayerInfo.Status.alive))
            {
                activeCamera.StopUsingCamera();
                activeCamera = GameManagerScript._instance.localPlayers[turn].m_camera;
                activeCamera.UseCamera();
                MessageManager.Instance.PlayYourTurnAnimation();
            }
            else
            {
                activeCamera.StopUsingCamera();
                activeCamera = freeRoamCamera;
                activeCamera.UseCamera();
            }
        }

        SetActivePlayers(turn);
    }

    public void AddPlayer(PlayerInfo player)
    {
        if (_players.Contains(player))
            return;

        _players.Add(player);
        UpdateTeams();
    }

    public void RemovePlayer(PlayerInfo player)
    {
        _players.Remove(player);
        UpdateTeams();
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
        UpdateTeams();
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

    private void SetActivePlayers(Color color)
    {
        foreach (PlayerInfo p in _players)
        {
            SetPlayerTurn(p, (color == p.team));
        }
    }

    private static void SetPlayerTurn(PlayerInfo p, bool active)
    {
        p.physicalPlayer.GetComponent<PlayerManager>().ChangeActiveStatus(active);
    }

    private void UpdateTeams()
    {
        teamMembers = new Dictionary<Color, List<PlayerInfo>>();
        foreach (Color c in teams)
        {
            teamMembers.Add(c, new List<PlayerInfo>());
        }

        _players.RemoveAll(item => item == null);

        foreach (PlayerInfo player in _players)
        {
            teamMembers[player.team].Add(player);
        }
    }
}