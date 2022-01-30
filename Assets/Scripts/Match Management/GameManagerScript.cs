using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class GameManagerScript : MonoBehaviour
{
    public MatchManager matchManager;
    [SerializeField] private Transform spawnPointContainer;
    private List<Transform> availableSpawnPoints = new List<Transform>();

    [SerializeField]
    private GameObject playerObj;

    //public PlayerInfo redPlayer, bluPlayer;
    private int botCounter = 0;

    public int playerPerTeam = 3;
    public int numberOfBlueTeamRealPlayer = 0;
    public int numberOfRedTeamRealPlayer = 0;

    public Dictionary<Color, PlayerInfo> localPlayers = new Dictionary<Color, PlayerInfo>();

    public static GameManagerScript _instance = null;

    private void Awake()
    {
        _instance = this;
        matchManager = MatchManager._instance;
        MatchInfo matchInfo = FindObjectOfType<MatchInfo>();
        if (matchInfo != null)
        {
            playerPerTeam = matchInfo.playerPerTeam;
            numberOfBlueTeamRealPlayer = matchInfo.realBluePlayer;
            numberOfRedTeamRealPlayer = matchInfo.realRedPlayer;
        }
        else
        {
            playerPerTeam = 3;
            numberOfBlueTeamRealPlayer = 0;
            numberOfRedTeamRealPlayer = 0;
        }

        Transform[] t = spawnPointContainer.GetComponentsInChildren<Transform>();
        availableSpawnPoints = new List<Transform>(t);
        for (int i = 0; i < playerPerTeam; i++)
        {
            int index = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
            Transform transform = availableSpawnPoints[index];
            availableSpawnPoints.RemoveAt(index);
            GameObject player = (GameObject)Instantiate(playerObj, transform);
            PlayerInfo info = player.GetComponent<PlayerInfo>();
            info.team = Color.red;
            info.status = PlayerInfo.Status.alive;
            if (index < numberOfRedTeamRealPlayer)
            {
                info.pname = "RED PLAYER" + (numberOfRedTeamRealPlayer > 1 ? ("" + (index + 1)) : "");
                localPlayers.Add(Color.red, info);
            }
            else
            {
                info.pname = "BOT " + (++botCounter);
                player.GetComponentInChildren<PlayerManager>().isABot = true;
            }
            player.name = info.pname;
        }
        for (int i = 0; i < playerPerTeam; i++)
        {
            int index = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
            Transform transform = availableSpawnPoints[index];
            availableSpawnPoints.RemoveAt(index);
            GameObject player = (GameObject)Instantiate(playerObj, transform);
            PlayerInfo info = player.GetComponent<PlayerInfo>();
            info.team = Color.blue;
            info.status = PlayerInfo.Status.alive;
            if (index < numberOfBlueTeamRealPlayer)
            {
                info.pname = "BLUE PLAYER" + (numberOfBlueTeamRealPlayer > 1 ? ("" + (index + 1)) : "");
                localPlayers.Add(Color.blue, info);
            }
            else
            {
                info.pname = "BOT " + (++botCounter);
                player.GetComponentInChildren<PlayerManager>().isABot = true;
            }
            player.name = info.pname;
        }
    }

    private void Start()
    {
        //matchManager.waiting = matchManager.turnDuration;
        matchManager.gameIsOver = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            matchManager.waiting = 0.01f;
    }
}