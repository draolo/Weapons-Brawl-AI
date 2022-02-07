using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target<T> : IComparable<Target<T>> where T : MonoBehaviour
{
    public T obj;
    public List<Vector2i> path;
    public Transform transform;

    public float Distance
    {
        get
        {
            return GetRealDistance();
        }
    }

    private float _hDistance;
    private Bot _bot;

    public Target(T o, Bot bot)
    {
        obj = o;
        _bot = bot;
        transform = o.transform;
        path = null;
    }

    private void HeuristicDistance(Vector2i startTile)
    {
        Vector2i tilePos = _bot.mMap.GetMapTileAtPoint(transform.position);
        Vector2 difference = (Vector2)startTile - (Vector2)tilePos;
        float vertical = difference.y > 0 ? 1.5f : 0.5f;
        _hDistance = Mathf.Abs(difference.x) + vertical * Mathf.Abs(difference.y);
    }

    public bool CalculatePath(Vector2i startTile)
    {
        HeuristicDistance(startTile);
        Vector2i tilePos = _bot.mMap.GetMapTileAtPoint(transform.position);
        path = _bot.mMap.mPathFinder.FindPath(
                startTile,
                tilePos,
                Mathf.CeilToInt(_bot.mWidth),
                Mathf.CeilToInt(_bot.mHeight),
                (short)_bot.mMaxJumpHeight);
        if (path != null && path.Count > 1)
        {
            return true;
        }
        else
        {
            path = null;
            return false;
        }
    }

    private float GetRealDistance()
    {
        if (path == null)
        {
            return _hDistance;
        }
        else
        {
            float distance = 0;
            Vector2i prev = path[0];
            foreach (Vector2i point in path)
            {
                distance += Vector2.Distance(prev, point);
                prev = point;
            }
            return distance;
        }
    }

    public int CompareTo(Target<T> other)
    {
        return Distance.CompareTo(other.Distance);
    }
}