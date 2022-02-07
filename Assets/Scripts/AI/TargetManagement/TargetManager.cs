using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager<T> where T : MonoBehaviour, AvailabilityNotificator
{
    private int _revision = -1;
    private List<Target<T>> allTargets;
    private List<Target<T>> reachableTargets;
    private Bot _bot;

    public TargetManager(List<T> objects, Bot bot)
    {
        allTargets = new List<Target<T>>();
        reachableTargets = new List<Target<T>>();
        _bot = bot;
        foreach (T o in objects)
        {
            allTargets.Add(new Target<T>(o, _bot));
            o.AvailabilityEvent.AddListener((b) => NotifyChange(o, b));
        }
        _revision = -1;
    }

    public void NotifyChange(T obj, bool available)
    {
        if (available)
        {
            AddElement(obj);
        }
        else
        {
            RemoveElement(obj);
        }
    }

    private void AddElement(T o)
    {
        Target<T> t = allTargets.Find((arrayTarget) => arrayTarget.obj.Equals(o));
        if (t != null)
        {
            return;
        }
        else
        {
            allTargets.Add(new Target<T>(o, _bot));
            _revision = -1;
        }
    }

    private void RemoveElement(T o)
    {
        Target<T> t = allTargets.Find((arrayTarget) => arrayTarget.obj.Equals(o));
        if (allTargets.Remove(t))
        {
            _revision = -1;
        }
    }

    public void FilterTarget(Predicate<Target<T>> predicate)
    {
        int oldCount = allTargets.Count;
        allTargets = allTargets.FindAll(predicate);
        if (allTargets.Count != oldCount)
        {
            _revision = -1;
        }
    }

    public List<Target<T>> GetReachable(Vector2i startTile, int revision)
    {
        if (_revision != revision)
        {
            reachableTargets = allTargets.FindAll(e => e.CalculatePath(startTile));
            _revision = revision;
        }
        return reachableTargets;
    }

    public List<Target<T>> GetAllTarget()
    {
        return allTargets;
    }

    public Target<T> GetClosest(Vector2i startTile, int revision, bool onlyReachAble = true)
    {
        if (!onlyReachAble)
        {
            if (allTargets.Count > 0)
            {
                allTargets.Sort();
                return allTargets[0];
            }
        }
        else
        {
            List<Target<T>> reachable = GetReachable(startTile, revision);
            if (reachable.Count > 0)
            {
                reachable.Sort();
                return reachable[0];
            }
        }
        return null;
    }

    public bool HasAReachAbleTarget(Vector2i startTile, int revision)
    {
        return GetReachable(startTile, revision).Count > 0;
    }

    public bool HasATarget()
    {
        return allTargets.Count > 0;
    }

    public int NumberOfTargets()
    {
        return allTargets.Count;
    }

    public void ResetCounter()
    {
        _revision = -1;
    }
}