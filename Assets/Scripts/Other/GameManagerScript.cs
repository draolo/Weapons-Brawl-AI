using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class GameManagerScript : MonoBehaviour
{
    public MatchManager matchManager;
    public MatchInfo matchInfo;
    public Transform spawnPointContainer;
    public List<Transform> availableSpawnPoints = new List<Transform>();

    private void Awake()
    {
        matchManager = FindObjectOfType<MatchManager>();
        matchInfo = FindObjectOfType<MatchInfo>();
        Transform[] t = spawnPointContainer.GetComponentsInChildren<Transform>();
        availableSpawnPoints = new List<Transform>(t);
    }

    private void Start()
    {
        matchManager.waiting = matchManager.turnDuration;
        matchManager.gameIsOver = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            matchManager.waiting = 0.01f;
    }
}