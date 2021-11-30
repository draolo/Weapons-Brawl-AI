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
        StartCoroutine(PlayBBat());
    }

    private IEnumerator PlayBBat()
    {
        CmdPlayBBat(true);
        yield return new WaitForSeconds(BBatAnim.GetCurrentAnimatorStateInfo(0).length);
        CmdPlayBBat(false);
    }

    private void CmdPlayBBat(bool yesno)
    {
        RpcPlayBBat(yesno);
    }

    private void RpcPlayBBat(bool yesno)
    {
        if (BBatAnim == null)
            BBatAnim = FindObjectOfType<Weapon4BBatScript>().gameObject.GetComponent<Animator>();
        BBatAnim.SetBool("isAttacking", yesno);
    }

    public void PlayPunchAnimation()
    {
        StartCoroutine(PlayPunch());
    }

    private IEnumerator PlayPunch()
    {
        CmdPlayPunch(true);
        yield return new WaitForSeconds(PunchAnim.GetCurrentAnimatorStateInfo(0).length);
        CmdPlayPunch(false);
    }

    private void CmdPlayPunch(bool yesno)
    {
        RpcPlayPunch(yesno);
    }

    private void RpcPlayPunch(bool yesno)
    {
        PunchAnim.SetBool("isPunching", yesno);
    }
}