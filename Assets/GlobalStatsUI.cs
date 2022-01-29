using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalStatsUI : AbstractInGameInterfaces
{
    public TextMeshProUGUI SingleScoreYouWinText;
    public TextMeshProUGUI GlobalScoreYouWinText;
    public GameObject rowObject;
    public RectTransform container;
    private float offset;
    private float h;

    public override void Open()
    {
        base.Open();

        GlobalScoreYouWinText.text = SingleScoreYouWinText.text;

        Transform red = UI.transform.Find("RedScore");
        Transform blue = UI.transform.Find("BlueScore");
        RectTransform rectTransform = rowObject.GetComponent<RectTransform>();
        h = rectTransform.rect.height;
        int numberOfPlayers = MatchManager._instance._players.Count;
        container.sizeDelta = new Vector2(container.sizeDelta.x, h * numberOfPlayers);
        offset = 0f;
        foreach (Color team in MatchManager._instance.teams)
        {
            List<PlayerInfo> members = MatchManager._instance.teamMembers[team];
            members.Sort((a, b) => b.GetPoints() - a.GetPoints());
            SetSquadInfo(members);
        }
    }

    private void SetSquadInfo(List<PlayerInfo> players)
    {
        foreach (PlayerInfo p in players)
        {
            try
            {
                GameObject row = Instantiate(rowObject, new Vector3(0, offset, 0), Quaternion.identity);
                row.transform.SetParent(container.transform, false);
                Transform name = row.transform.Find("Name");
                Transform rank = row.transform.Find("Rank");
                Transform score = row.transform.Find("Score");
                Transform KDRatio = row.transform.Find("KD");
                Transform damage = row.transform.Find("Damage");
                SetValue(name, p.pname);
                SetValue(rank, p.GetRank().ToString());
                SetValue(score, p.GetPoints().ToString());
                SetValue(KDRatio, p.KDRatio());
                SetValue(damage, p.damageToEnemy.ToString());
                Image img = row.GetComponent<Image>();
                Color c = p.team;
                c.a = 50f / 255f;
                img.color = c;
                offset -= h;
            }
            catch (Exception e)
            {
                Debug.Log("exception " + e.ToString());
                break;
            }
        }
    }

    private void SetValue(Transform box, string value)
    {
        if (!box)
        {
            Debug.Log("null box");
            return;
        }
        box.GetComponent<TextMeshProUGUI>().text = value;
    }
}