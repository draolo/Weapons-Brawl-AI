using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target<T>: IComparable<Target<T>> where T : MonoBehaviour  
{
    public T obj;
    public List<Vector2i> path;
    public Transform transform;
    public float distance;
    
    public Target(T o)
    {
        obj = o;
        transform = o.transform;
        path = null;
    }

    public bool CalculatePath(Vector2i startTile, Bot bot)
    {
        Vector2i tilePos = bot.mMap.GetMapTileAtPoint(transform.position);
        Vector2 difference = (Vector2)startTile - (Vector2)tilePos;
        distance = Mathf.Abs(difference.x) + Mathf.Abs(difference.y);
        path = bot.mMap.mPathFinder.FindPath(
                startTile,
                tilePos,
                Mathf.CeilToInt(bot.mWidth),
                Mathf.CeilToInt(bot.mHeight),
                (short)bot.mMaxJumpHeight);
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
