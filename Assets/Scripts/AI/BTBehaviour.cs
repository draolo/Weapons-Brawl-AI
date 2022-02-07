using CRBT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BTBehaviour : MonoBehaviour
{
    public UnityEvent onStartBehaviour = new UnityEvent();
    public UnityEvent onStopBehaviour = new UnityEvent();
    public UnityEvent onBehaviourComplete = new UnityEvent();
    public UnityEvent onBehaviourFail = new UnityEvent();

    protected BehaviorTree AI;
    protected float aiTime = .2f;

    protected abstract void CreateTree();

    public virtual void StartBehavior()
    {
        StopAllCoroutines();
        onStartBehaviour.Invoke();
        try
        {
            CreateTree();
            StartCoroutine(PlayBT());
        }
        catch (Exception)
        {
            FailedTask();
        }
    }

    public virtual void StopBehavior()
    {
        StopAllCoroutines();
        onStopBehaviour.Invoke();
    }

    public virtual IEnumerator PlayBT()
    {
        bool step;
        do
        {
            try
            {
                BeforeAIStep();
                step = AI.Step();
                AfterAIStep();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                FailedTask();
                break;
            }
            yield return new WaitForSeconds(aiTime);
        } while (step);
        CompletedTask();
    }

    protected virtual void BeforeAIStep()
    {
        return;
    }

    protected virtual void AfterAIStep()
    {
        return;
    }

    protected virtual void CompletedTask()
    {
        StopAllCoroutines();
        onBehaviourComplete.Invoke();
    }

    protected virtual void FailedTask()
    {
        StopAllCoroutines();
        onBehaviourFail.Invoke();
    }
}