using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyMyFeature : MonoBehaviour
{

    public void LoadStartMenu()
    {
        Prototype.NetworkLobby.LobbyManager.s_Singleton.ServerChangeScene("StartMenu");
    }
}