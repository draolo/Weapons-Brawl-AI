using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target<T> where T : MonoBehaviour
{
    public T obj;
    public List<Vector2i> path;
    public Transform transform;
    
    public Target(T o)
    {
        obj = o;
        transform = o.transform;
        path = null;
    }

    public bool CalculatePath(Vector2i startTile, Bot bot)
    {
        path = bot.mMap.mPathFinder.FindPath(
                startTile,
                bot.mMap.GetMapTileAtPoint(transform.position),
                Mathf.CeilToInt(bot.mWidth),
                Mathf.CeilToInt(bot.mHeight),
                (short)bot.mMaxJumpHeight);
        if (path != null && path.Count > 1)
            return true;
        else
            return false;
    }
}
