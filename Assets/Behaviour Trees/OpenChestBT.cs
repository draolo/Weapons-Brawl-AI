using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CRBT;

public class OpenChestBT : MonoBehaviour
{
    public float aiTime = .2f;
    public float beginWaitTime = 1f;

    private BehaviorTree AI;
    private Rigidbody2D _rigidbody;
    public Transform target;
    public Vector2 targetOld;
    public Bot bot;
    private PlayerChestManager playerChestManager;


    // Start is called before the first frame update
    void Start()
    {
        playerChestManager = gameObject.GetComponent<PlayerChestManager>();
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        bot = gameObject.GetComponent<Bot>();

        BTAction setPath = new BTAction(SearchAndSetPath);
        BTAction startBot = new BTAction(Move);
        BTAction stop = new BTAction(Stop);
        BTAction open = new BTAction(OpenTheChest);

        BTCondition proximityCheck = new BTCondition(NotCloseToTheTarget);
        BTCondition samePosition = new BTCondition(TargetPositionEqual);
        BTCondition isMovinig = new BTCondition(IsItMoving);
        BTSelector pathVerifier = new BTSelector(new IBTTask[] {samePosition, setPath});
        BTSequence movingCycle = new BTSequence(new IBTTask[] { pathVerifier, proximityCheck});
        BTDecorator checkUntilFail = new BTDecoratorUntilFail(movingCycle);
        BTDecorator waitMovementEnd = new BTDecoratorUntilFail(isMovinig);



        BTSequence s1 = new BTSequence(new IBTTask[] { setPath, startBot, checkUntilFail, stop,waitMovementEnd, open });


        AI = new BehaviorTree(s1);

        StartCoroutine(OpenChest());
    }

    public IEnumerator OpenChest()
    {
        yield return new WaitForSeconds(beginWaitTime);
        while (AI.Step())
        {
            yield return new WaitForSeconds(aiTime);
        }
    }


    public bool SearchAndSetPath()
    {
        targetOld = target.position;
        Debug.Log("path search");
        Vector2 something = bot.mPosition - bot.mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f;
        Debug.Log(bot.mMap);
        Vector2i startTile = bot.mMap.GetMapTileAtPoint(bot.mPosition - bot.mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f);
        Debug.Log(something);
        if (bot.mOnGround && !bot.IsOnGroundAndFitsPos(startTile))
        {
            if (bot.IsOnGroundAndFitsPos(new Vector2i(startTile.x + 1, startTile.y)))
                startTile.x += 1;
            else
                startTile.x -= 1;
        }
        var path = bot.mMap.mPathFinder.FindPath(
                        startTile,
                        bot.mMap.GetMapTileAtPoint(target.position),
                        Mathf.CeilToInt(bot.mWidth),
                        Mathf.CeilToInt(bot.mHeight),
                        (short)bot.mMaxJumpHeight);


        

        if (path != null && path.Count > 1)
        {
            bot.mPath.Clear();
            for (var i = path.Count - 1; i >= 0; --i)
            {
                bot.mPath.Add(path[i]);
            }
            Debug.Log("path search success");

            return true;
        }
        else
        {
            Debug.Log("path search fail");

            return false;
        }

    }

    public bool Move()
    {
        bot.mCurrentNodeId = 1;

        bot.ChangeAction(Bot.BotAction.MoveTo);

        bot.mFramesOfJumping = bot.GetJumpFramesForNode(0);
        return true;

    }

    public bool Stop()
    {
        Debug.Log("stop");
        bot.ChangeAction(Bot.BotAction.None);
        return true;
    }

    public bool OpenTheChest()
    {
        Debug.Log("open");
        playerChestManager.TryToOpenChest();
        return true;
    }

    public bool NotCloseToTheTarget()
    {
        float distance= Vector2.Distance(transform.position,target.position);
        return (distance > playerChestManager.InteractionRadius);
    }

    public bool TargetPositionEqual()
    {
        Debug.Log("V " + (targetOld == (Vector2)target.position));
        return targetOld== (Vector2)target.position;
    }

    public bool IsItMoving()
    {
        return (_rigidbody.velocity != new Vector2(0, 0));
    }

}
