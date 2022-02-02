using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator PunchAnim;
    public Animator BBatAnim;

    public Animator anim;
    public PlayerMovementOffline mov;

    private void Start()
    {
        mov = GetComponent<PlayerMovementOffline>();
    }

    private void Update()
    {
        if (!mov.enabled)
        {
            anim.SetFloat("Blend", 0f);
        }
    }

    public void PlayBBatAnimation()
    {
        StartCoroutine(PlayBBatCoroutine());
    }

    private IEnumerator PlayBBatCoroutine()
    {
        PlayBBat(true);
        yield return new WaitForSeconds(BBatAnim.GetCurrentAnimatorStateInfo(0).length);
        PlayBBat(false);
    }

    private void PlayBBat(bool yesno)
    {
        if (BBatAnim == null)
            BBatAnim = FindObjectOfType<Weapon4BBatScript>().gameObject.GetComponent<Animator>();
        BBatAnim.SetBool("isAttacking", yesno);
    }

    public void PlayPunchAnimation()
    {
        StartCoroutine(PlayPunchCoroutine());
    }

    private IEnumerator PlayPunchCoroutine()
    {
        PlayPunch(true);
        yield return new WaitForSeconds(PunchAnim.GetCurrentAnimatorStateInfo(0).length);
        PlayPunch(false);
    }

    private void PlayPunch(bool yesno)
    {
        PunchAnim.SetBool("isPunching", yesno);
    }
}