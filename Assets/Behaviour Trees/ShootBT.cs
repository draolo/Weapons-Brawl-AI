using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CRBT;


public class ShootBT : MonoBehaviour
{
    public float aiTime = .2f;
    public float beginWaitTime = 1f;


    private BehaviorTree AI;

    private TargetAim targetAim;
    public Transform target;
    public Vector2 targetOld;
    public LayerMask projectileObstacle;
    public Vector2 shootingAngle;
    private Bot bot;
    private Rigidbody2D _rigidbody;
    private PlayerWeaponManager_Inventory shootingManager;
    private PlayerMovementOffline playerMovementOffline;
    // Start is called before the first frame update
    void Start()
    {
        playerMovementOffline = GetComponent<PlayerMovementOffline>();
        targetAim = GetComponent<TargetAim>();
        bot = gameObject.GetComponent<Bot>();
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        shootingManager = gameObject.GetComponent<PlayerWeaponManager_Inventory>();

        BTAction setPath = new BTAction(IsThereAPathToTheTarget);
        BTAction startBot = new BTAction(Move);
        BTAction stop = new BTAction(Stop);
        BTAction aim = new BTAction(SetAim);
        BTAction shoot = new BTAction(Shoot);
        BTAction shake = new BTAction(AimShaker);
        BTAction faceTheTarget = new BTAction(FaceTheTarget);

        BTCondition couldSeeTheTarget = new BTCondition(LineOfSight);
        BTCondition samePosition = new BTCondition(TargetPositionEqual);
        BTCondition straightLine = new BTCondition(TestAndSetStraight);
        BTCondition lobbedLine = new BTCondition(TestAndSetLobbed);
        BTCondition emptyFireline = new BTCondition(EmptyFireLine);
        BTCondition isMovinig = new BTCondition(IsItMoving);
        BTCondition isGrounded = new BTCondition(IsItOnGround);
        BTDecorator waitMovementEnd = new BTDecoratorUntilFail(isMovinig);


        BTSequence emptyStraightFireLine = new BTSequence(new IBTTask[] {isGrounded, straightLine, emptyFireline });
        BTSequence emptyLobbedFireLine = new BTSequence(new IBTTask[] {isGrounded, lobbedLine, emptyFireline});
        BTSelector isThereAFireLine = new BTSelector(new IBTTask[] { emptyStraightFireLine, emptyLobbedFireLine });
        BTSequence shootFarAway = new BTSequence(new IBTTask[] { stop,waitMovementEnd, faceTheTarget, isThereAFireLine, aim, shoot });


        BTSelector pathVerifier = new BTSelector(new IBTTask[] { samePosition, setPath });
        BTDecorator notInLine = new BTDecoratorInverter(couldSeeTheTarget);
        BTDecorator notEmptyLine = new BTDecoratorInverter(isThereAFireLine);
        
        BTSequence endMovementAndTest = new BTSequence(new IBTTask[] {isGrounded, stop, waitMovementEnd, notEmptyLine, startBot});
        

        BTSelector movementConditions = new BTSelector(new IBTTask[] { notInLine,notEmptyLine });

        
        BTDecorator movingCycle = new BTDecoratorUntilFail(movementConditions);



        BTSequence getCloser = new BTSequence(new IBTTask[] { setPath, startBot, movingCycle, shootFarAway});



        BTRandomSelector standardBehaviour = new BTRandomSelector(new IBTTask[] {shootFarAway, getCloser});

        BTSequence desperateBehaviour = new BTSequence(new IBTTask[] { straightLine, aim, shoot });

        BTSelector s1 = new BTSelector(new IBTTask[] { standardBehaviour, desperateBehaviour });

        AI = new BehaviorTree(s1);

        StartCoroutine(ShootTarget());
    }

    public IEnumerator ShootTarget()
    {
        yield return new WaitForSeconds(beginWaitTime);
        while (AI.Step())
        {
            yield return new WaitForSeconds(aiTime);
        }
    }

    public bool LineOfSight()
    {
        Debug.Log("AIS: test line of sight");
        RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, projectileObstacle );
        if (hit.collider == null)
        {
            Debug.Log("AIS: test line of sight pass");
            return true;
        }
        Debug.Log("AIS: test line of sight fail");
        return false;
    }

    public bool SetAim()
    {
        Debug.Log("AIS: set aim");
        targetAim.SetAim(shootingAngle);
        return true;
    }

    public bool TestAndSetStraight()
    {
        Debug.Log("AIS: test straight");

        Vector2 direction = targetAim.Aim();
        if (direction.x <= -999)
        {
            Debug.Log("AIS: test straight fail");
            return false;

        }
        Debug.Log("AIS: test straight pass");
        shootingAngle = direction;
        return true;
    }

    public bool EmptyFireLine()
    {
        Debug.Log("AIS: test fire line");

        Vector2 impactPoint = targetAim.CollisionPredictionStupid(shootingAngle);
        if (impactPoint.x <= -999)
        {
            Debug.Log("AIS: test fire line pass");
            return true;

        }
        Debug.Log("AIS: test fire line fail");
        return false;
    }

    public bool TestAndSetLobbed()
    {
        Debug.Log("AIS: test lobbed");
        Vector2 direction = targetAim.Aim(true);
        if (direction.x <= -999)
        {
            Debug.Log("AIS: test lobbed fail");
            return false;

        }
        shootingAngle = direction;
        Debug.Log("AIS: test lobbed pass");
        return true;
    }

    public bool IsThereAPathToTheTarget()
    {
        Debug.Log("AIS: path searching");
        targetOld = target.position;
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
            Debug.Log("AIS: path search success");

            return true;
        }
        else
        {
            Debug.Log("AIS: path search fail");

            return false;
        }

    }

    public bool Stop()
    {
        Debug.Log("AIS: stop moving");
        bot.ChangeAction(Bot.BotAction.None);
        return true;
    }

    public bool Move()
    {
        Debug.Log("AIS: startmoving");

        bot.mCurrentNodeId = 1;

        bot.ChangeAction(Bot.BotAction.MoveTo);

        bot.mFramesOfJumping = bot.GetJumpFramesForNode(0);
        return true;

    }

    public bool TargetPositionEqual()
    {
        Debug.Log("AIS: checkpos");

        Debug.Log("V " + (targetOld == (Vector2)target.position));
        return targetOld == (Vector2)target.position;
    }

    public bool AimShaker()
    {
        //TODO
        Debug.Log("AIS: aimshaking");

        return true;
    }

    public bool Shoot()
    {
        Debug.Log("AIS: shooting");
        shootingManager.CmdAttack(100);
        return true;
    }

    public bool IsItMoving()
    {
        return (_rigidbody.velocity != new Vector2(0, 0));
    }

    public bool IsItOnGround()
    {
        Debug.Log("AIS: ground check: "+ bot.mOnGround);
        return bot.mOnGround;
    }


    public bool FaceTheTarget()
    {
        Vector2 dir = target.position - transform.position;
        playerMovementOffline.FaceTowards(dir.x);
        return true;
    }

}
