using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameScreemUI : AbstractInGameInterfaces
{
    public GlobalStatsUI globalStatsUI;

    public override void Open()
    {
        if (globalStatsUI.isActive)
        {
            return;
        }
        base.Open();

        List<PlayerInfo> localPlayers = new List<PlayerInfo>();
        localPlayers.AddRange(GameManagerScript._instance.localPlayers.Values);
        PlayerInfo localPlayer = null;
        switch (localPlayers.Count)
        {
            case 0:
                {
                    List<PlayerInfo> winners = MatchManager._instance.teamMembers[MatchManager._instance.winner];
                    winners.Sort((a, b) => b.GetPoints() - a.GetPoints());
                    localPlayer = winners[0];
                    break;
                }
            case 1:
                {
                    localPlayer = localPlayers[0];
                    break;
                }
            default:
                {
                    List<PlayerInfo> winningPlayers = localPlayers.FindAll(p => p.win);
                    if (winningPlayers.Count > 0)
                    {
                        localPlayer = winningPlayers[0];
                    }
                    else
                    {
                        localPlayer = localPlayers[0];
                    }
                    break;
                }
        }

        Transform winLose = UI.transform.Find("YouWinLoseText");
        if (localPlayer != null)
        {
            Transform statistics = UI.transform.Find("Statistics");
            Transform killLabel = statistics.transform.Find("Kill");
            Transform deathLabel = statistics.transform.Find("Death");
            Transform ResurrectedLabel = statistics.transform.Find("Resurrected");
            Transform DamageLabel = statistics.transform.Find("Damage");
            Transform PointsLabel = statistics.transform.Find("Total");
            SetValue(killLabel, localPlayer.kills);
            SetValue(deathLabel, localPlayer.deaths);
            SetValue(ResurrectedLabel, localPlayer.resurrectedAlly);
            SetValue(DamageLabel, localPlayer.damageToEnemy);
            SetValue(PointsLabel, localPlayer.GetPoints());
            UI.transform.Find("Rank").Find("Rank").GetComponent<TextMeshProUGUI>().text = localPlayer.GetRank() + "";
            if (localPlayers.Count == 1)
            {
                if (localPlayer.win)
                {
                    winLose.GetComponent<TextMeshProUGUI>().text = "YOU WIN";
                }
                else
                {
                    winLose.GetComponent<TextMeshProUGUI>().text = "YOU LOSE";
                }
            }
            else
            {
                winLose.GetComponent<TextMeshProUGUI>().text = ColorPlus.ColorToName(MatchManager._instance.winner).ToUpper() + " WIN";
            }
        }
    }

    private void SetValue(Transform box, int value)
    {
        if (!box)
        {
            Debug.Log("null box");
            return;
        }
        Transform valueBox = box.Find("Value");
        valueBox.GetComponent<TextMeshProUGUI>().text = value.ToString();
    }
}