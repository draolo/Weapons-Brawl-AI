using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Algorithms;
using UnityEngine.UI;
using System;

public class Bot : Character
{
    public enum BotAction
    {
        None = 0,
        MoveTo,
    }

    public PlayerMovementOffline movementHandler;
    public bool freeRoam;

    public BotAction mCurrentAction = BotAction.None;

    public int mCurrentNodeId = -1;

    public int mFramesOfJumping = 0;
    public int mStuckFrames = 0;

    public int mMaxJumpHeight = 5;
    public int mWidth;
    public int mHeight;

    public const int cMaxStuckFrames = 20;
    private bool jumpOnStuck = false;

    public void GoToPosition(Vector2i mapPosInTile)
    {
        while (!(mMap.IsGround(mapPosInTile.x, mapPosInTile.y)))
        {
            --mapPosInTile.y;
            if (mapPosInTile.y < 0)
            {
                return;
            }
        }
        MoveTo(new Vector2i(mapPosInTile.x, mapPosInTile.y + 1));
    }

    public void Start()
    {
        mMap = FindObjectOfType<Map>();
        mMap.mapModificationEvent.AddListener(UpdatePath);
        mInputs = new bool[(int)KeyInput.Count];
        mPrevInputs = new bool[(int)KeyInput.Count];
        mPosition = transform.position;

        mAABB.HalfSize = new Vector2(0.5f * mWidth, 0.5f * mHeight);
    }

    public bool IsOnGroundAndFitsPos(Vector2i pos)
    {
        for (int y = pos.y; y < pos.y + mHeight; ++y)
        {
            for (int x = pos.x; x < pos.x + mWidth; ++x)
            {
                if (mMap.IsObstacle(x, y))
                    return false;
            }
        }

        for (int x = pos.x; x < pos.x + mWidth; ++x)
        {
            if (mMap.IsGround(x, pos.y - 1))
                return true;
        }

        return false;
    }

    public void MoveTo(Vector2i destinationInTile)
    {
        mStuckFrames = 0;
        jumpOnStuck = false;
        Vector2i startTile = mMap.GetMapTileAtPoint(mPosition - mAABB.HalfSize + 0.5f * Map.cTileSize * Vector2.one);
        if (mOnGround && !IsOnGroundAndFitsPos(startTile))
        {
            if (IsOnGroundAndFitsPos(new Vector2i(startTile.x + 1, startTile.y)))
                startTile.x += 1;
            else
                startTile.x -= 1;
        }

        var path = mMap.mPathFinder.FindPath(
                        startTile,
                        destinationInTile,
                        Mathf.CeilToInt(mWidth),
                        Mathf.CeilToInt(mHeight),
                        (short)mMaxJumpHeight);

        mCurrentAction = BotAction.None;
        mCurrentNodeId = -1;
        mPath.Clear();

        if (path != null && path.Count > 1)
        {
            for (var i = path.Count - 1; i >= 0; --i)
                mPath.Add(path[i]);

            mCurrentNodeId = 1;

            ChangeAction(BotAction.MoveTo);

            mFramesOfJumping = GetJumpFramesForNode(0);
        }
        else
        {
            gameObject.GetComponent<AgentAI>().StopAndStart(); //todo find a better way
        }
    }

    public void ChangeAction(BotAction newAction)
    {
        mCurrentAction = newAction;
    }

    private int GetJumpFrameCount(int deltaY)
    {
        if (deltaY <= 0)
            return 0;
        else
        {
            switch (deltaY)
            {
                case 1:
                    return 1;

                case 2:
                    return 2;

                case 3:
                    return 6;

                case 4:
                    return 9;

                case 5:
                    return 15;

                case 6:
                    return 21;

                default:
                    return 30;
            }
        }
    }

    public bool ReachedNodeOnXAxis(Vector2 pathPosition, Vector2 prevDest, Vector2 currentDest)
    {
        return (prevDest.x <= currentDest.x && pathPosition.x >= currentDest.x)
            || (prevDest.x >= currentDest.x && pathPosition.x <= currentDest.x)
            || Mathf.Abs(pathPosition.x - currentDest.x) <= Constants.cBotMaxPositionError;
    }

    public bool ReachedNodeOnYAxis(Vector2 pathPosition, Vector2 prevDest, Vector2 currentDest)
    {
        return (prevDest.y <= currentDest.y && pathPosition.y >= currentDest.y)
            || (prevDest.y >= currentDest.y && pathPosition.y <= currentDest.y)
            || (Mathf.Abs(pathPosition.y - currentDest.y) <= Constants.cBotMaxPositionError);
    }

    public void GetContext(out Vector2 prevDest, out Vector2 currentDest, out Vector2 nextDest, out bool destOnGround, out bool reachedX, out bool reachedY)
    {
        //   Debug.Log("context node id" + mCurrentNodeId + "path count " + mPath.Count);
        if (mCurrentNodeId > 0)
        {
            prevDest = mMap.GetMapTilePosition(mPath[mCurrentNodeId - 1]);
        }
        else
        {
            prevDest = transform.position;
        }
        try
        {
            currentDest = mMap.GetMapTilePosition(mPath[mCurrentNodeId]);

            nextDest = currentDest;

            if (mPath.Count > mCurrentNodeId + 1)
            {
                nextDest = mMap.GetMapTilePosition(mPath[mCurrentNodeId + 1]); ;
            }

            destOnGround = false;
            for (int x = mPath[mCurrentNodeId].x; x < mPath[mCurrentNodeId].x + mWidth; ++x)
            {
                if (mMap.IsGround(x, mPath[mCurrentNodeId].y - 1))
                {
                    destOnGround = true;
                    break;
                }
            }

            Vector2 pathPosition = (Vector2)transform.position - mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f;
            reachedX = ReachedNodeOnXAxis(pathPosition, prevDest, currentDest);
            reachedY = ReachedNodeOnYAxis(pathPosition, prevDest, currentDest);

            //snap the character if it reached the goal but overshot it by more than cBotMaxPositionError
            if (reachedX && Mathf.Abs(pathPosition.x - currentDest.x) > Constants.cBotMaxPositionError && Mathf.Abs(pathPosition.x - currentDest.x) < Constants.cBotMaxPositionError * 3.0f && !mPrevInputs[(int)KeyInput.GoRight] && !mPrevInputs[(int)KeyInput.GoLeft])
            {
                pathPosition.x = currentDest.x;
                mPosition.x = pathPosition.x - Map.cTileSize * 0.5f + mAABB.HalfSizeX;
                transform.position = mPosition;
            }

            if (destOnGround && !mOnGround)
                reachedY = false;
        }
        catch (ArgumentOutOfRangeException dd)
        {
            Debug.Log("catched node id" + mCurrentNodeId + "path count " + mPath.Count);
            Debug.Log(dd);
            nextDest = currentDest = prevDest = new Vector2();
            destOnGround = true;
            reachedX = false;
            reachedY = false;
        }
    }

    /*
    public void TestJumpValues()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            mFramesOfJumping = GetJumpFrameCount(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            mFramesOfJumping = GetJumpFrameCount(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            mFramesOfJumping = GetJumpFrameCount(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            mFramesOfJumping = GetJumpFrameCount(4);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            mFramesOfJumping = GetJumpFrameCount(5);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            mFramesOfJumping = GetJumpFrameCount(6);
    }
    */

    public int GetJumpFramesForNode(int prevNodeId)
    {
        int currentNodeId = prevNodeId + 1;

        if (mPath[currentNodeId].y - mPath[prevNodeId].y > 0 && mOnGround)
        {
            int jumpHeight = 1;
            for (int i = currentNodeId; i < mPath.Count; ++i)
            {
                if (mPath[i].y - mPath[prevNodeId].y >= jumpHeight)
                    jumpHeight = mPath[i].y - mPath[prevNodeId].y;
                if (mPath[i].y - mPath[prevNodeId].y < jumpHeight || mMap.IsGround(mPath[i].x, mPath[i].y - 1))
                    return GetJumpFrameCount(jumpHeight);
            }
        }

        return 0;
    }

    private void FixedUpdate()
    {
        BotUpdate();
        if (Input.GetKeyUp(KeyCode.P))
        {
            freeRoam = !freeRoam;
        }
    }

    public void BotUpdate()
    {
        if (freeRoam)
        {
            return;
        }
        mOnGround = movementHandler.isGrounded;
        //get the position of the bottom of the bot's aabb, this will be much more useful than the center of the sprite (mPosition)
        int tileX, tileY;
        var position = transform.position;
        position.y -= mAABB.HalfSizeY;
        mInputs[(int)KeyInput.GoRight] = false;
        mInputs[(int)KeyInput.GoLeft] = false;
        mInputs[(int)KeyInput.Jump] = false;
        mInputs[(int)KeyInput.GoDown] = false;
        mMap.GetMapTileAtPoint(position, out tileX, out tileY);
        //Debug.Log("Bottom at " + tileX + ":" + tileY);

        switch (mCurrentAction)
        {
            case BotAction.None:

                //TestJumpValues();

                if (mFramesOfJumping > 0)
                {
                    mFramesOfJumping -= 1;
                    mInputs[(int)KeyInput.Jump] = true;
                }

                break;

            case BotAction.MoveTo:

                Vector2 prevDest, currentDest, nextDest;
                bool destOnGround, reachedY, reachedX;
                GetContext(out prevDest, out currentDest, out nextDest, out destOnGround, out reachedX, out reachedY);
                Vector2 pathPosition = (Vector2)transform.position - mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f;

                mInputs[(int)KeyInput.GoRight] = false;
                mInputs[(int)KeyInput.GoLeft] = false;
                mInputs[(int)KeyInput.Jump] = false;
                mInputs[(int)KeyInput.GoDown] = false;
                /*
                if (pathPosition.y - currentDest.y > Constants.cBotMaxPositionError && mOnOneWayPlatform)
                    mInputs[(int)KeyInput.GoDown] = true;
                */
                if (reachedX && reachedY)
                {
                    int prevNodeId = mCurrentNodeId;
                    mCurrentNodeId++;

                    if (mCurrentNodeId >= mPath.Count)
                    {
                        mCurrentNodeId = -1;
                        ChangeAction(BotAction.None);
                        break;
                    }

                    if (mOnGround)
                        mFramesOfJumping = GetJumpFramesForNode(prevNodeId);

                    goto case BotAction.MoveTo;
                }
                else if (!reachedX)
                {
                    if (currentDest.x - pathPosition.x > Constants.cBotMaxPositionError)
                        mInputs[(int)KeyInput.GoRight] = true;
                    else if (pathPosition.x - currentDest.x > Constants.cBotMaxPositionError)
                        mInputs[(int)KeyInput.GoLeft] = true;
                }
                else if (!reachedY && mPath.Count > mCurrentNodeId + 1 && !destOnGround)
                {
                    int checkedX = 0;

                    if (mPath[mCurrentNodeId + 1].x != mPath[mCurrentNodeId].x)
                    {
                        mMap.GetMapTileAtPoint(pathPosition, out tileX, out tileY);

                        if (mPath[mCurrentNodeId + 1].x > mPath[mCurrentNodeId].x)
                            checkedX = tileX + mWidth;
                        else
                            checkedX = tileX - 1;
                    }

                    if (checkedX != 0 && !mMap.AnySolidBlockInStripe(checkedX, tileY, mPath[mCurrentNodeId + 1].y))
                    {
                        if (nextDest.x - pathPosition.x > Constants.cBotMaxPositionError)
                            mInputs[(int)KeyInput.GoRight] = true;
                        else if (pathPosition.x - nextDest.x > Constants.cBotMaxPositionError)
                            mInputs[(int)KeyInput.GoLeft] = true;

                        if (ReachedNodeOnXAxis(pathPosition, currentDest, nextDest) && ReachedNodeOnYAxis(pathPosition, currentDest, nextDest))
                        {
                            mCurrentNodeId += 1;
                            if (mCurrentNodeId >= mPath.Count)
                            {
                                mCurrentNodeId = -1;
                                ChangeAction(BotAction.None);
                            }
                            else
                            {
                                goto case BotAction.MoveTo;
                            }
                        }
                    }
                }

                if (mFramesOfJumping > 0 &&
                    (!mOnGround || (reachedX && !destOnGround) || (mOnGround && destOnGround)))
                {
                    mInputs[(int)KeyInput.Jump] = true;
                    if (!mOnGround)
                        --mFramesOfJumping;
                }

                if (mPosition == mOldPosition)
                {
                    ++mStuckFrames;
                    if (mStuckFrames > cMaxStuckFrames)
                    {
                        if (jumpOnStuck)
                        {
                            UpdatePath();
                        }
                        else
                        {
                            mInputs[(int)KeyInput.Jump] = true;
                            jumpOnStuck = true;
                        }
                    }
                }
                else
                {
                    mStuckFrames = 0;
                    if (jumpOnStuck && (Mathf.Abs(mPosition.x - mOldPosition.x) > 0.2f))
                    {
                        jumpOnStuck = false;
                    }
                }
                break;
        }
        if (mInputs[(int)KeyInput.Jump])
        {
            movementHandler.jump = true;
        }
        if (mInputs[(int)KeyInput.GoRight])
        {
            movementHandler.horizontalMove = movementHandler.speed;
        }
        if (mInputs[(int)KeyInput.GoLeft])
        {
            movementHandler.horizontalMove = -movementHandler.speed;
        }
        if (!(mInputs[(int)KeyInput.GoRight] ^ mInputs[(int)KeyInput.GoLeft]))
        {
            movementHandler.horizontalMove = 0;
        }
        movementHandler.PlayerMovement();
        UpdatePrevInputs();
        mOldPosition = mPosition;
        mPosition = transform.position;
    }

    public void UpdatePath()
    {
        if (mPath != null && mPath.Count > 0)
        {
            MoveTo(mPath[mPath.Count - 1]);
        }
    }
}