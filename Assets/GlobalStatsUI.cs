using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalStatsUI : AbstractInGameInterfaces {

    public TextMeshProUGUI SingleScoreYouWinText;
    public TextMeshProUGUI GlobalScoreYouWinText;

    public override void Open()
    {
        base.Open();

        GlobalScoreYouWinText.text = SingleScoreYouWinText.text;

        Transform red = UI.transform.Find("RedScore");
        Transform blue = UI.transform.Find("BlueScore");
        List<PlayerInfo> redplayers = MatchManager._instance.RedTeam;
        List<PlayerInfo> blueplayers = MatchManager._instance.BlueTeam;
        SetSquadInfo(red, redplayers);
        SetSquadInfo(blue, blueplayers);

    }

    private void SetSquadInfo(Transform table, List<PlayerInfo> players)
    {
        int i = 0;
        foreach (PlayerInfo p in players)
        {
            try
            {
                Transform row = table.GetChild(i);
                Transform name = row.transform.Find("Name");
                Transform rank = row.transform.Find("Rank");
                Transform score = row.transform.Find("Score");
                Transform KDRatio = row.transform.Find("KD");
                Transform damage = row.transform.Find("Damage");
                SetValue(name, p.pname);
                SetValue(rank, p.getRank().ToString());
                SetValue(score, p.GetPoints().ToString());
                SetValue(KDRatio, p.KDRatio());
                SetValue(damage, p.damageToEnemy.ToString());
                i++;
            }
            catch (Exception e)
            {
                Debug.Log("exception " + e.ToString());
                break;
            }

        }
    }

    private void SetValue(Transform box, String value)
    {
        if (!box)
        {
            Debug.Log("null box");
            return;
        }
        box.GetComponent<TextMeshProUGUI>().text = value;
    }
}
