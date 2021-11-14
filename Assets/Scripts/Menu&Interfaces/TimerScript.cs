using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerScript : MonoBehaviour {

    public Image turn;
    public TextMeshProUGUI timer;

    private MatchManager matchInfo;

	void Start () {
        matchInfo = FindObjectOfType<MatchManager>();
    }

    private void Update()
    {
        turn.color = matchInfo.turn;
        timer.text = matchInfo.waiting.ToString("0");
    }

}
