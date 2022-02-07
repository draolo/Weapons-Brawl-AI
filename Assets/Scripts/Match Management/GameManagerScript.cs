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

    public Dictionary<Color, int> nOfRealPlayers = new Dictionary<Color, int>();

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
            foreach (Color c in matchInfo.numberOfRealPlayerPerTeam.Keys)
            {
                nOfRealPlayers.Add(c, matchInfo.numberOfRealPlayerPerTeam[c]);
            }
        }
        else
        {
            playerPerTeam = 1;
            nOfRealPlayers.Add(Color.blue, 0);
            nOfRealPlayers.Add(Color.red, 0);
        }

        Transform[] t = spawnPointContainer.GetComponentsInChildren<Transform>();
        availableSpawnPoints = new List<Transform>(t);
        foreach (Color c in nOfRealPlayers.Keys)
        {
            CreateTeam(c);
        }
    }

    private void CreateTeam(Color c)
    {
        for (int i = 0; i < playerPerTeam; i++)
        {
            int index = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
            Transform transform = availableSpawnPoints[index];
            availableSpawnPoints.RemoveAt(index);
            GameObject player = (GameObject)Instantiate(playerObj, transform);
            PlayerInfo info = player.GetComponent<PlayerInfo>();
            PlayerManager playerManager = player.GetComponentInChildren<PlayerManager>();
            info.team = c;
            info.status = PlayerInfo.Status.alive;
            if (i < nOfRealPlayers[c])
            {
                info.pname = ColorPlus.ColorToName(c).ToUpper() + " PLAYER" + (nOfRealPlayers[c] > 1 ? ("" + (i + 1)) : "");
                info.isAbot = false;
                localPlayers.Add(c, info);
            }
            else
            {
                info.pname = "BOT " + (++botCounter);
                playerManager.isABot = true;
                info.isAbot = true;
            }
            player.name = info.pname;
            playerManager.ChangeActiveStatus(false);
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