using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target<T> : IComparable<Target<T>> where T : MonoBehaviour
{
    public T obj;
    public List<Vector2i> path;
    public Transform transform;
    public float distance;
    private Bot _bot;

    public Target(T o, Bot bot)
    {
        obj = o;
        _bot = bot;
        transform = o.transform;
        path = null;
    }

    public void SetUpDistance(Vector2i startTile)
    {
        Vector2i tilePos = _bot.mMap.GetMapTileAtPoint(transform.position);
        Vector2 difference = (Vector2)startTile - (Vector2)tilePos;
        float vertical = difference.x > 0 ? 1.5f : 0.5f;
        distance = Mathf.Abs(difference.x) + vertical * Mathf.Abs(difference.y);
    }

    public bool CalculatePath(Vector2i startTile)
    {
        SetUpDistance(startTile);
        Vector2i tilePos = _bot.mMap.GetMapTileAtPoint(transform.position);
        path = _bot.mMap.mPathFinder.FindPath(
                startTile,
                tilePos,
                Mathf.CeilToInt(_bot.mWidth),
                Mathf.CeilToInt(_bot.mHeight),
                (short)_bot.mMaxJumpHeight);
        if (path != null && path.Count > 1)
            return true;
        else
            return false;
    }

    public int CompareTo(Target<T> other)
    {
        return distance.CompareTo(other.distance);
    }
}