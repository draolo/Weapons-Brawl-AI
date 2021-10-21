using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using System;

public class NetworkLobbyHook : LobbyHook {
    public Color[] teamsColor = { Color.blue, Color.red };
    // for users to apply settings from their lobby player GameObject to their in-game player GameObject
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager network, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        var cc = lobbyPlayer.GetComponent<LobbyPlayer>();
        var player = gamePlayer.GetComponent<PlayerInfo>();
        foreach (Color c in teamsColor)
        {
            if (c==cc.playerColor)
            {
                player.team = cc.playerColor;
                player.pname = cc.playerName;
            }
        }
        if (Array.IndexOf(teamsColor, cc.playerColor)<0)
        {
            Destroy(lobbyPlayer);
            Destroy(gamePlayer);
        }
        else
        {
            MatchManager._instance.AddPlayer(player);
        }

    }


}
