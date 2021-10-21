using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameScreemUI : AbstractInGameInterfaces {
    public PlayerInfo localPlayer;
    public GlobalStatsUI globalStatsUI;
    public override void Open()
    {
        if (globalStatsUI.isActive)
        {
            return;
        }
        base.Open();
        
        Transform winLose = UI.transform.Find("YouWinLoseText");
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
        UI.transform.Find("Rank").Find("Rank").GetComponent<TextMeshProUGUI>().text=localPlayer.getRank()+"";
        if (localPlayer.win)
        {
            winLose.GetComponent<TextMeshProUGUI>().text = "YOU WIN";
        }
        else
        {
            winLose.GetComponent<TextMeshProUGUI>().text = "YOU LOSE";
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
        valueBox.GetComponent<TextMeshProUGUI>().text=value.ToString();
    }
}
