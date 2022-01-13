using CRBT;
using System.Collections;
using UnityEngine;

public class OpenChestBT : MonoBehaviour
{
    private BehaviorTree AI;
    private IBTTask root;
    private PlayerChestManager playerChestManager;
    private Rigidbody2D _rigidbody;
    public Bot bot;
    public float aiTime = .2f;
    public float beginWaitTime = 1f;
    public Transform target;
    public Vector2 targetOld;

    private AgentAI aiManager;

    // Start is called before the first frame update
    private void Awake()
    {
        playerChestManager = gameObject.GetComponent<PlayerChestManager>();
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        bot = gameObject.GetComponent<Bot>();
        aiManager = gameObject.GetComponent<AgentAI>();
        BTAction setPath = new BTAction(SearchAndSetPath);
        BTAction startBot = new BTAction(Move);
        BTAction stop = new BTAction(Stop);
        BTAction open = new BTAction(OpenTheChest);

        BTSequence safeSetPath = new BTSequence(new IBTTask[] { stop, setPath, startBot });
        BTCondition proximityCheck = new BTCondition(NotCloseToTheTarget);
        BTCondition samePosition = new BTCondition(TargetPositionEqual);
        BTCondition isMovinig = new BTCondition(IsItMoving);
        BTSelector pathVerifier = new BTSelector(new IBTTask[] { samePosition, safeSetPath });
        BTSequence movingCycle = new BTSequence(new IBTTask[] { pathVerifier, proximityCheck });
        BTDecorator checkUntilFail = new BTDecoratorUntilFail(movingCycle);
        BTDecorator waitMovementEnd = new BTDecoratorUntilFail(isMovinig);

        root = new BTSequence(new IBTTask[] { safeSetPath, checkUntilFail, stop, waitMovementEnd, open });
    }

    public void StartBehavior()
    {
        StopAllCoroutines();
        AI = new BehaviorTree(root);
        StartCoroutine(OpenChest());
    }

    public void StopBehavior()
    {
        StopAllCoroutines();
        target = null;
    }

    public IEnumerator OpenChest()
    {
        yield return new WaitForSeconds(beginWaitTime);
        bool step;
        do
        {
            yield return new WaitForSeconds(aiTime);
            try
            {
                step = AI.Step();
            }
            catch (MissingReferenceException mre)
            {
                Debug.Log(mre);
                step = false;
            }
        } while (step);
        EndOfTask();
    }

    private void EndOfTask(bool completed = true)
    {
        StopBehavior();
        aiManager.FindNewAction();
    }

    public bool SearchAndSetPath()
    {
        targetOld = target.position;
        Vector2 something = bot.mPosition - bot.mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f;
        Vector2i startTile = bot.mMap.GetMapTileAtPoint(bot.mPosition - bot.mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f);
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

            return true;
        }
        else
        {
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
        bot.ChangeAction(Bot.BotAction.None);

        return true;
    }

    public bool OpenTheChest()
    {
        playerChestManager.TryToOpenChest();
        return true;
    }

    public bool NotCloseToTheTarget()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        return (distance > playerChestManager.InteractionRadius);
    }

    public bool TargetPositionEqual()
    {
        return targetOld == (Vector2)target.position;
    }

    public bool IsItMoving()
    {
        return (_rigidbody.velocity != new Vector2(0, 0));
    }
}