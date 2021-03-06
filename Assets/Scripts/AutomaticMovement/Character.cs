using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Algorithms;

public class Character : MovingObject
{
    [System.Serializable]
    public enum CharacterState
    {
        Stand,
        Run,
        Jump,
    };

    /// <summary>
    /// The current state.
    /// </summary>
    [HideInInspector]
    public CharacterState mCurrentState = CharacterState.Stand;

    /// <summary>
    /// The number of frames passed from changing the state to jump.
    /// </summary>
    protected int mFramesFromJumpStart = 0;

    protected bool[] mInputs;
    protected bool[] mPrevInputs;

    protected List<Vector2i> mPath = new List<Vector2i>();

    /// <summary>
    /// Raises the draw gizmos event.
    /// </summary>
    private void OnDrawGizmos()
    {
        DrawMovingObjectGizmos();

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere((mPosition - mAABB.HalfSize + 0.5f * Map.cTileSize * Vector2.one), 1);

        //draw the path

        if (mPath != null && mPath.Count > 0)
        {
            var start = mMap.GetMapTilePosition(mPath[0].x, mPath[0].y);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(start, .2f);

            for (var i = 1; i < mPath.Count; ++i)
            {
                var end = mMap.GetMapTilePosition(mPath[i].x, mPath[i].y);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(end, .2f);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(start, end);
                start = end;
            }
        }
    }

    public void UpdatePrevInputs()
    {
        var count = (byte)KeyInput.Count;

        for (byte i = 0; i < count; ++i)
            mPrevInputs[i] = mInputs[i];
    }
}