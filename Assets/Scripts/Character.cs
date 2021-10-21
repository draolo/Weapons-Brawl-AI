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


    public List<Vector2i> mPath = new List<Vector2i>();

    /// <summary>
    /// Raises the draw gizmos event.
    /// </summary>
    void OnDrawGizmos()
    {
        DrawMovingObjectGizmos();

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
                Gizmos.DrawLine(start,end);
                start = end;
            }
        }
    }

    public LineRenderer lineRenderer;

    protected void DrawPathLines()
    {
        if (mPath != null && mPath.Count > 0)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetVertexCount(mPath.Count);
            lineRenderer.SetWidth(.2f, .2f);

            for (var i = 0; i < mPath.Count; ++i)
            {
                lineRenderer.SetColors(Color.red, Color.red);
                Vector2 worldCellPosition = mMap.GetMapTilePosition(mPath[i].x, mPath[i].y);
                lineRenderer.SetPosition(i, worldCellPosition);
            }
        }
        else
            lineRenderer.enabled = false;
    }

    public void UpdatePrevInputs()
    {
        var count = (byte)KeyInput.Count;

        for (byte i = 0; i < count; ++i)
            mPrevInputs[i] = mInputs[i];
    }
    
}
