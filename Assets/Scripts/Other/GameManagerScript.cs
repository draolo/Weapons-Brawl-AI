using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class GameManagerScript : MonoBehaviour {

    public MatchManager matchInfo;

    void Start()
    {
        matchInfo = FindObjectOfType<MatchManager>();
        matchInfo.waiting = matchInfo.turnDuration;
        matchInfo.gameIsOver = false;
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            matchInfo.waiting = 0.01f;
    }
}
