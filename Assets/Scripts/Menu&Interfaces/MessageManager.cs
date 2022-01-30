using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour {

    public static MessageManager Instance;

    public GameObject YourTurnText;
    private Animator YourTurnAnimator;

    public GameObject EndTurnText;
    private Animator EndTurnTextAnimator;


    private void Start()
    {
        Instance = this;
        YourTurnAnimator = YourTurnText.GetComponent<Animator>();
        EndTurnTextAnimator = EndTurnText.GetComponent<Animator>();
    }




    public void PlayEndTurnAnimation()
    {
        StartCoroutine(PETAnim());
    }

    IEnumerator PETAnim()
    {
        EndTurnText.SetActive(true);
        EndTurnTextAnimator.Play("Bump", -1, 0f);
        yield return new WaitForSeconds(EndTurnTextAnimator.GetCurrentAnimatorStateInfo(0).length);
        EndTurnText.SetActive(false);
    }












    public void PlayYourTurnAnimation()
    {
        StartCoroutine(PYTAnim());
    }

    IEnumerator PYTAnim()
    {
        YourTurnText.SetActive(true);
        YourTurnAnimator.Play("Bump", -1, 0f);
        yield return new WaitForSeconds(YourTurnAnimator.GetCurrentAnimatorStateInfo(0).length);
        YourTurnText.SetActive(false);
    }
}
