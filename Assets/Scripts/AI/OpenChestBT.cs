using CRBT;
using System.Collections;
using UnityEngine;

public class OpenChestBT : MonoBehaviour
{
    private BehaviorTree AI;
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
    }

    private void CreateTree()
    {
        BTAction setPath = new BTAction(SearchAndSetPath);
        BTAction startBot = new BTAction(Move);
        BTAction stop = new BTAction(Stop);
        BTAction open = new BTAction(OpenTheChest);

        BTCondition proximityCheck = new BTCondition(NotCloseToTheTarget);
        BTCondition samePosition = new BTCondition(TargetPositionEqual);
        BTCondition isMovinig = new BTCondition(IsItMoving);

        BTSequence safeSetPath = new BTSequence(new IBTTask[] { stop, setPath, startBot });
        BTSelector pathVerifier = new BTSelector(new IBTTask[] { samePosition, safeSetPath });
        BTSequence movingCycle = new BTSequence(new IBTTask[] { pathVerifier, proximityCheck });
        BTDecorator checkUntilFail = new BTDecoratorUntilFail(movingCycle);
        BTDecorator waitMovementEnd = new BTDecoratorUntilFail(isMovinig);
        IBTTask root = new BTSequence(new IBTTask[] { safeSetPath, checkUntilFail, stop, waitMovementEnd, open });
        AI = new BehaviorTree(root);
    }

    public void StartBehavior()
    {
        StopAllCoroutines();
        CreateTree();
        StartCoroutine(OpenChest());
    }

    public void StopBehavior()
    {
        StopAllCoroutines();
        bot.StopTheBot();
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
        return bot.SearchAndSetPath(bot.mMap.GetMapTileAtPoint(target.position));
    }

    public bool Move()
    {
        bot.StartTheBot();
        return true;
    }

    public bool Stop()
    {
        bot.StopTheBot();

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