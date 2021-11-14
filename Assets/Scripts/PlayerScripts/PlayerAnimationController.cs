﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAnimationController : MonoBehaviour {

    public Animator PunchAnim;
    public Animator BBatAnim;

    public Animator anim;
    public PlayerMovement mov;

    private void Start()
    {
        anim = GetComponent<Animator>();
        mov= GetComponent<PlayerMovement>();
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

    IEnumerator PlayBBat()
    {
        CmdPlayBBat(true);
        yield return new WaitForSeconds(BBatAnim.GetCurrentAnimatorStateInfo(0).length);
        CmdPlayBBat(false);
    }

    void CmdPlayBBat(bool yesno)
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

    IEnumerator PlayPunch()
    {
        CmdPlayPunch(true);
        yield return new WaitForSeconds(PunchAnim.GetCurrentAnimatorStateInfo(0).length);
        CmdPlayPunch(false);
    }
    void CmdPlayPunch(bool yesno)
    {
        RpcPlayPunch(yesno);
    }

    private void RpcPlayPunch(bool yesno)
    {
        PunchAnim.SetBool("isPunching", yesno);
    }
}
