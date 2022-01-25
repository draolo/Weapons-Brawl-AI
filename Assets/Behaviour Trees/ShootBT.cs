using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CRBT;

public class ShootBT : MonoBehaviour
{
    public float aiTime = .2f;
    public float beginWaitTime = 1f;
    public float safeDistance = 8f;
    private BehaviorTree AI;

    private AgentAI aiManager;

    private TargetAim targetAim;
    public Transform target;
    public PlayerInfo targetInfo;
    public Vector2 targetOld;
    public LayerMask projectileObstacle;
    public Vector2 shootingAngle;
    private Bot bot;
    private Rigidbody2D _rigidbody;
    private PlayerWeaponManager_Inventory shootingManager;
    private PlayerMovementOffline playerMovementOffline;
    private RaycastHit2D hitPoint;
    private Vector2 impactPoint;
    private Vector2 safePlace = new Vector2(-9999, -9999);

    // Start is called before the first frame update
    private void Awake()
    {
        playerMovementOffline = GetComponent<PlayerMovementOffline>();
        targetAim = GetComponent<TargetAim>();
        bot = gameObject.GetComponent<Bot>();
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        shootingManager = gameObject.GetComponent<PlayerWeaponManager_Inventory>();
        aiManager = gameObject.GetComponent<AgentAI>();
    }

    private void CreateTree()
    {
        //for advanced tree could be better to use a reset method that reset the index of BTComposite task
        BTAction setTarget = new BTAction(SetTarget);
        BTAction setPath = new BTAction(IsThereAPathToTheTarget);
        BTAction startBot = new BTAction(Move);
        BTAction stop = new BTAction(Stop);
        BTAction pauseMovement = new BTAction(PauseMovement);
        BTAction resumeMovement = new BTAction(ResumeMovement);
        BTAction setTheClosestPathToTheEnemy = new BTAction(SetClosestPathToTheTarget);
        BTAction setPathToSafePlace = new BTAction(SetPathToASafePlaceToShoot);

        BTAction aim = new BTAction(SetAim);
        BTAction shoot = new BTAction(Shoot);
        BTAction shake = new BTAction(AimShaker);
        BTAction setUpWeapon = new BTAction(SetWeapon);
        BTAction faceTheTarget = new BTAction(FaceTheTarget);
        BTAction setIfItIsAsafePlace = new BTAction(CheckAndSetSafePlace);
        BTAction setImpactPoint = new BTAction(SetImpactPoint);
        BTAction stupid = new BTAction(LogStupidAction);

        BTCondition testIfIHitMyself = new BTCondition(DoesTheShootHitMyself);
        // BTCondition targetAlive = new BTCondition(IsTargetAlive);
        BTCondition couldSeeTheTarget = new BTCondition(LineOfSight);
        BTCondition samePosition = new BTCondition(TargetPositionEqual);
        BTCondition straightLine = new BTCondition(TestAndSetStraight);
        BTCondition lobbedLine = new BTCondition(TestAndSetLobbed);
        BTCondition emptyFireline = new BTCondition(EmptyFireLine);
        BTCondition isMovinig = new BTCondition(IsItMoving);
        BTCondition isGrounded = new BTCondition(IsItOnGround);
        BTDecorator waitMovementEnd = new BTDecoratorUntilFail(isMovinig);

        BTSequence safeShoot = new BTSequence(new IBTTask[] { setUpWeapon, shoot });

        BTSequence emptyStraightFireLine = new BTSequence(new IBTTask[] { isGrounded, straightLine, setImpactPoint, setIfItIsAsafePlace, emptyFireline });
        BTSequence emptyLobbedFireLine = new BTSequence(new IBTTask[] { isGrounded, lobbedLine, setImpactPoint, setIfItIsAsafePlace, emptyFireline });
        BTSelector isThereAFireLine = new BTSelector(new IBTTask[] { emptyStraightFireLine, emptyLobbedFireLine });
        BTSequence shootWithEmptyFireLine = new BTSequence(new IBTTask[] { stop, waitMovementEnd, faceTheTarget, isThereAFireLine, aim, safeShoot });
        BTSequence safeSetPath = new BTSequence(new IBTTask[] { stop, setPath });

        BTSelector pathVerifier = new BTSelector(new IBTTask[] { samePosition, safeSetPath });
        BTDecorator notInLine = new BTDecoratorInverter(couldSeeTheTarget);
        BTDecorator notEmptyLine = new BTDecoratorInverter(isThereAFireLine);
        BTDecorator failPause = new BTDecoratorInverter(pauseMovement);
        BTDecorator failResume = new BTDecoratorInverter(resumeMovement);

        BTSequence checkTarget = new BTSequence(new IBTTask[] { couldSeeTheTarget, isThereAFireLine });

        BTDecorator invertedTargetCheck = new BTDecoratorInverter(checkTarget);

        BTDecorator waitUntilTargetLocked = new BTDecoratorUntilFail(invertedTargetCheck);

        BTSequence endMovementAndTest = new BTSequence(new IBTTask[] { pauseMovement, waitMovementEnd, invertedTargetCheck, resumeMovement });

        BTSequence movementConditions = new BTSequence(new IBTTask[] { waitUntilTargetLocked, endMovementAndTest });

        BTDecorator movingCycle = new BTDecoratorUntilFail(movementConditions);

        BTSequence getCloser = new BTSequence(new IBTTask[] { safeSetPath, startBot, movingCycle, shootWithEmptyFireLine });

        BTRandomSelector standardBehaviour = new BTRandomSelector(new IBTTask[] { shootWithEmptyFireLine, getCloser });

        BTDecorator didINotHitMyself = new BTDecoratorInverter(testIfIHitMyself);

        BTSequence testIfIHitMyselfStraight = new BTSequence(new IBTTask[] { isGrounded, straightLine, setImpactPoint, setIfItIsAsafePlace, didINotHitMyself });
        BTSequence testIfIHitMyselfLobbed = new BTSequence(new IBTTask[] { isGrounded, lobbedLine, setImpactPoint, setIfItIsAsafePlace, didINotHitMyself });

        BTSelector testLobbedAndStraigthShoot = new BTSelector(new IBTTask[] { testIfIHitMyselfStraight, testIfIHitMyselfLobbed });

        BTSequence shootWithoutHitMyself = new BTSequence(new IBTTask[] { stop, waitMovementEnd, faceTheTarget, testLobbedAndStraigthShoot, aim, safeShoot });

        BTSequence stupidShoot = new BTSequence(new IBTTask[] { stupid, faceTheTarget, straightLine, aim, safeShoot });

        BTDecorator invertedFirelineCheck = new BTDecoratorInverter(isThereAFireLine);

        BTSequence checkLineOrMovement = new BTSequence(new IBTTask[] { invertedFirelineCheck, isMovinig });

        BTDecorator waitUntilThereIsALineOrPathEnd = new BTDecoratorUntilFail(checkLineOrMovement);

        BTSequence searchAndShootForward = new BTSequence(new IBTTask[] { waitUntilThereIsALineOrPathEnd, shootWithoutHitMyself });

        BTDecorator checkIfIHaventGotAline = new BTDecoratorInverter(testLobbedAndStraigthShoot);

        BTSequence testLineAndMovementBackwards = new BTSequence(new IBTTask[] { checkIfIHaventGotAline, isMovinig });

        BTDecorator moveUntilIDontShootMyself = new BTDecoratorUntilFail(testLineAndMovementBackwards);

        BTSequence searchAndShootBackwards = new BTSequence(new IBTTask[] { setPathToSafePlace, startBot, moveUntilIDontShootMyself, shootWithoutHitMyself });

        BTSelector searchLineAndShoot = new BTSelector(new IBTTask[] { searchAndShootForward, searchAndShootBackwards });

        BTSequence desperateGetCloser = new BTSequence(new IBTTask[] { setTheClosestPathToTheEnemy, startBot, searchLineAndShoot });

        BTRandomSelector desperateBehaviour = new BTRandomSelector(new IBTTask[] { shootWithoutHitMyself, desperateGetCloser });

        BTSelector shootingStrategies = new BTSelector(new IBTTask[] { standardBehaviour, desperateBehaviour, stupidShoot });
        IBTTask root = new BTSequence(new IBTTask[] { setTarget, shootingStrategies });

        AI = new BehaviorTree(root);
    }

    private void Log(string message)
    {
        Debug.Log(gameObject.transform.parent.gameObject.name + " " + message);
    }

    public void StartBehavior()
    {
        StopAllCoroutines();
        targetAim.target = target;
        Log("starting behavior");
        try
        {
            targetInfo = target.gameObject.GetComponentInParent<PlayerInfo>();
            CreateTree();
            StartCoroutine(ShootTarget());
        }
        catch (MissingReferenceException)
        {
            EndOfTask(false);
        }
    }

    private void EndOfTask(bool completed = true)
    {
        Log("end of task");
        StopBehavior();
        aiManager.FindNewAction();
    }

    public void StopBehavior()
    {
        Log("ending behavior");
        StopAllCoroutines();
        bot.StopTheBot();
        target = null;
        targetInfo = null;
        targetAim.target = null;
    }

    public IEnumerator ShootTarget()
    {
        Log("start coroutine");
        yield return new WaitForSeconds(beginWaitTime);
        bool step;
        do
        {
            try
            {
                if (!IsTargetAlive())
                {
                    step = false;
                    Log("stopped coroutine due to target death");
                }
                else
                {
                    step = AI.Step();
                }
            }
            catch (MissingReferenceException mre)
            {
                Log("stopped coroutine due to missing reference");
                Debug.Log(mre);
                step = false;
            }
            yield return new WaitForSeconds(aiTime);
        } while (step);
        EndOfTask();
    }

    public bool LineOfSight()
    {
        Log("testing line of sight");
        RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, projectileObstacle);
        if (hit.collider == null)
        {
            Log("success");
            return true;
        }
        Log("fails");
        return false;
    }

    public bool SetAim()
    {
        Log("setting aim" + shootingAngle);
        targetAim.SetAim(shootingAngle);
        return true;
    }

    public bool TestAndSetStraight()
    {
        Log("test and set straight");
        Vector2 direction = targetAim.Aim();
        if (direction.x <= -999)
        {
            Log("fail");
            return false;
        }
        Log("success" + direction);
        shootingAngle = direction.normalized;
        return true;
    }

    public bool SetImpactPoint()
    {
        Log("test if the fireline is empty " + shootingAngle);
        hitPoint = targetAim.CollisionPredictionStupid(shootingAngle);
        impactPoint = hitPoint.point;
        return true;
    }

    public bool EmptyFireLine()
    {
        if (impactPoint.x <= -999)
        {
            Log("success we hit nothing, is that a succes???");
            //very strange but we haven't hit nothing
            return true;
        }
        if (hitPoint.collider.gameObject == target.gameObject)
        {
            Log("success we hit the target");
            return true;
        }
        float targetDistance = Vector2.Distance(transform.position, target.position);
        if (targetDistance < targetAim.firePointDistance)
        {
            Log("success we are close to the target");
            return true;
        }
        Log("fail");
        return false;
    }

    public bool TestAndSetLobbed()
    {
        Log("test and set lobbed");
        Vector2 direction = targetAim.Aim(true);
        if (direction.x <= -999)
        {
            Log("fail");
            return false;
        }
        Log("success " + direction);
        shootingAngle = direction.normalized;
        return true;
    }

    public bool IsThereAPathToTheTarget()
    {
        Log("search and set path to the target");
        targetOld = target.position;
        bool res = bot.SearchAndSetPath(bot.mMap.GetMapTileAtPoint(target.position));
        Log("result: " + res);
        return res;
    }

    public bool SetClosestPathToTheTarget()
    {
        Log("search and set path close to the target");
        targetOld = target.position;
        bool res = bot.SearchAndSetPath(bot.mMap.GetMapTileAtPoint(target.position), true);
        Log("result: " + res);
        return res;
    }

    public bool SetPathToASafePlaceToShoot()
    {
        Log("search and set path to the target");
        //targetOld = target.position;
        bool res = bot.SearchAndSetPath(bot.mMap.GetMapTileAtPoint(safePlace));
        Log("result: " + res);
        return res;
    }

    public bool DoesTheShootHitMyself()
    {
        Log("checking if i hit myself");
        bool res = (Vector2.Distance(impactPoint, transform.position) < safeDistance);
        Log("result " + res);
        return res;
    }

    public bool CheckAndSetSafePlace()
    {
        float distance = Vector2.Distance(impactPoint, transform.position);
        Debug.Log("called check and set " + distance + "collided with" + hitPoint.collider.name);
        if (distance > safeDistance)
        {
            Debug.Log("it is safe");
            safePlace = transform.position;
        }
        return true;
    }

    public bool Stop()
    {
        Log("stopping the bot");
        bot.StopTheBot();
        return true;
    }

    public bool PauseMovement()
    {
        Log("pause movent");
        bot.PauseTheMovement();
        return true;
    }

    public bool ResumeMovement()
    {
        Log("resume movement");
        bot.ResumeTheMovement();
        return true;
    }

    public bool Move()
    {
        Log("starting the bot");
        bot.StartTheBot();
        return true;
    }

    public bool TargetPositionEqual()
    {
        Log("testing if target is in the same position");
        bool res = targetOld == (Vector2)target.position;
        Log("result: " + res);
        return res;
    }

    public bool AimShaker()
    {
        Log("adding an error to the aim");
        //TODO

        return true;
    }

    public bool Shoot()
    {
        Log("shooting");
        shootingManager.CmdAttack(100);
        return true;
    }

    public bool IsItMoving()
    {
        Log("checking if is moving");
        bool res = (_rigidbody.velocity != new Vector2(0, 0));
        Log("result: " + res);
        return res;
    }

    public bool IsItOnGround()
    {
        Log("checking if is on the ground");
        return bot.mOnGround;
    }

    public bool FaceTheTarget()
    {
        Log("facing the target");
        Vector2 dir = target.position - transform.position;
        playerMovementOffline.FaceTowards(dir.x);
        return true;
    }

    public bool SetTarget()
    {
        Log("setting the target");
        targetAim.SetTarget(target);
        return true;
    }

    public bool IsTargetAlive()
    {
        Log("checking if target is alive");
        return targetInfo.status == PlayerInfo.Status.alive;
    }

    public bool SetWeapon()
    {
        Log("setting the weapon");
        bool emptyFireLine = EmptyFireLine();
        float targetDistance = Vector2.Distance(transform.position, target.position);
        float impactPointDistance = Vector2.Distance(transform.position, impactPoint);
        bool meleeOnly = targetDistance < targetAim.firePointDistance;
        if (!emptyFireLine && !meleeOnly)
        {
            shootingManager.SwitchWeapon(0);
            return true;
        }
        List<AbstractWeaponGeneric> availableWeapons = shootingManager.Weapons;
        List<AbstractWeaponGeneric> longRangeWeapons = availableWeapons.FindAll(w => typeof(AbstractWeaponBulletBased).IsAssignableFrom(w.GetType()));
        List<AbstractWeaponGeneric> longRangeWeaponsThatDontHitMe = longRangeWeapons.FindAll(w => !DoesIHitMyselfWithThisWeapon((AbstractWeaponBulletBased)w, impactPointDistance));
        List<AbstractWeaponGeneric> meleeWeapons = availableWeapons.FindAll(w => typeof(AbstractWeaponMelee).IsAssignableFrom(w.GetType()));
        List<AbstractWeaponGeneric> meleeWeaponsThatHitTheTarget = meleeWeapons.FindAll(w => ((AbstractWeaponMelee)w).attackRange > targetDistance);
        List<AbstractWeaponGeneric> suitableWeapons = new List<AbstractWeaponGeneric>();
        if (!meleeOnly)
        {
            suitableWeapons.AddRange(longRangeWeaponsThatDontHitMe);
        }
        suitableWeapons.AddRange(meleeWeaponsThatHitTheTarget);
        AbstractWeaponGeneric bestWeapon;
        if (suitableWeapons.Count > 0)
        {
            suitableWeapons.Sort();
            suitableWeapons.Reverse();
            bestWeapon = suitableWeapons[0];
        }
        else
        {
            longRangeWeapons.Sort();
            bestWeapon = longRangeWeapons[0];
        }
        if (bestWeapon)
        {
            int index = availableWeapons.FindIndex(w => w.GetType() == bestWeapon.GetType());
            shootingManager.SwitchWeapon(index);
            return true;
        }
        Log("failed to set the weaopon, really????");
        return false;
    }

    private bool DoesIHitMyselfWithThisWeapon(AbstractWeaponBulletBased w, float impactPointDistance)
    {
        float bias = 1;
        AbstractBulletExplosive bullet = w.bulletPrefab.GetComponent<AbstractBulletExplosive>();
        return (impactPointDistance - bias) <= bullet.ExplosionRadius;
    }

    private bool LogStupidAction()
    {
        Log("supid");
        return true;
    }
}