using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerAfterAttackScript : MonoBehaviour {

    public static TextMeshProUGUI timerUI;
    public float TimeToDisappear = 3f;

    private static float timeLeft;
    private string previousSec;

    private void Start()
    {
        timerUI = transform.Find("TimerAfterAttack").gameObject.GetComponent<TextMeshProUGUI>();
    }

    public static void SetTimer(float sec)
    {
        timeLeft = sec;
        timerUI.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (timerUI.gameObject.activeSelf)
        {
            if(timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;

                string timeToPrint = timeLeft.ToString("0");

                if (timeToPrint != previousSec)
                {
                    timerUI.GetComponent<Animator>().Play("Bump", -1, 0f);
                    previousSec = timeToPrint;
                }

                timerUI.text = timeLeft.ToString("0");
            }
            else
                StartCoroutine(DisableTimerAfterSec());
        }


    }

    IEnumerator DisableTimerAfterSec()
    {
        yield return new WaitForSeconds(TimeToDisappear);
        timerUI.gameObject.SetActive(false);
    }

    

}
