using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CRBT;
using System;

public class ShootBT : BTBehaviour
{
    public float beginWaitTime = 1f;
    public float safeDistance = 8.5f;

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
    private Vector2 impactPoint = new Vector2(-9999, -9999);

    private float power;
    private int inaccuracy;
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
        inaccuracy = UnityEngine.Random.Range(0, 15);
    }

    protected override void CreateTree()
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
        BTAction choosePower = new BTAction(SetPower);
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
        BTDecorator FastThereIsAFireLine = new BTDecoratorFastTree(isThereAFireLine);
        BTSequence shootWithEmptyFireLine = new BTSequence(new IBTTask[] { stop, waitMovementEnd, faceTheTarget, FastThereIsAFireLine, shake, aim, safeShoot });
        BTSequence safeSetPath = new BTSequence(new IBTTask[] { stop, setPath });
        BTSequence setPathAndStart = new BTSequence(new IBTTask[] { safeSetPath, startBot });

        BTSelector pathVerifier = new BTSelector(new IBTTask[] { samePosition, setPathAndStart });

        BTSequence checkTarget = new BTSequence(new IBTTask[] { isGrounded, couldSeeTheTarget, choosePower, FastThereIsAFireLine });

        BTDecorator invertedTargetCheck = new BTDecoratorInverter(FastThereIsAFireLine);

        BTDecorator waitUntilTargetLocked = new BTDecoratorUntilSucces(checkTarget);

        BTSequence endMovementAndTest = new BTSequence(new IBTTask[] { pauseMovement, waitMovementEnd, invertedTargetCheck, resumeMovement });

        BTSequence movementConditions = new BTSequence(new IBTTask[] { waitUntilTargetLocked, endMovementAndTest });

        BTDecorator movingCycle = new BTDecoratorUntilFail(movementConditions);

        BTSequence getCloser = new BTSequence(new IBTTask[] { safeSetPath, startBot, movingCycle, shootWithEmptyFireLine });

        BTRandomSelector standardBehaviour = new BTRandomSelector(new IBTTask[] { shootWithEmptyFireLine, getCloser });

        BTDecorator didINotHitMyself = new BTDecoratorInverter(testIfIHitMyself);

        BTSequence testIfIHitMyselfStraight = new BTSequence(new IBTTask[] { isGrounded, straightLine, setImpactPoint, didINotHitMyself, setIfItIsAsafePlace });
        BTSequence testIfIHitMyselfLobbed = new BTSequence(new IBTTask[] { isGrounded, lobbedLine, setImpactPoint, didINotHitMyself, setIfItIsAsafePlace });

        BTSelector testLobbedAndStraigthShoot = new BTSelector(new IBTTask[] { testIfIHitMyselfStraight, testIfIHitMyselfLobbed });
        BTDecorator fastTestIfIHitMyself = new BTDecoratorFastTree(testLobbedAndStraigthShoot);

        BTSequence shootWithoutHitMyself = new BTSequence(new IBTTask[] { stop, waitMovementEnd, faceTheTarget, fastTestIfIHitMyself, shake, aim, safeShoot });

        BTSequence stupidShoot = new BTSequence(new IBTTask[] { stupid, faceTheTarget, straightLine, shake, aim, safeShoot });

        BTDecorator invertedFirelineCheck = new BTDecoratorInverter(FastThereIsAFireLine);

        BTSequence checkLineOrMovement = new BTSequence(new IBTTask[] { choosePower, invertedFirelineCheck, isMovinig });

        BTDecorator waitUntilThereIsALineOrPathEnd = new BTDecoratorUntilFail(checkLineOrMovement);

        BTSequence searchAndShootForward = new BTSequence(new IBTTask[] { waitUntilThereIsALineOrPathEnd, shootWithoutHitMyself });

        BTDecorator checkIfIHaventGotAline = new BTDecoratorInverter(fastTestIfIHitMyself);

        BTSequence testLineAndMovementBackwards = new BTSequence(new IBTTask[] { choosePower, checkIfIHaventGotAline, isMovinig });

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
        //Debug.Log(gameObject.transform.parent.gameObject.name + " " + message);
    }

    public override void StartBehavior()
    {
        power = 100;
        try
        {
            targetInfo = target.gameObject.GetComponentInParent<PlayerInfo>();
        }
        catch (Exception)
        {
            FailedTask();
        }
        base.StartBehavior();
    }

    public override void StopBehavior()
    {
        base.StopBehavior();
        bot.StopTheBot();
        target = null;
        targetInfo = null;
        targetAim.SetTarget(null);
    }

    protected override void BeforeAIStep()
    {
        if (!IsTargetAlive())
        {
            FailedTask();
            Log("stopped coroutine due to target death");
        }
        base.BeforeAIStep();
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
        Vector2 direction = targetAim.GetShootingAngle(power / 100f);
        if (direction.x <= -999)
        {
            Log("fail");
            return false;
        }
        Log("success" + direction);
        shootingAngle = direction.normalized;
        return true;
    }

    public bool SetPower()
    {
        Vector2 _45Deg = Vector2.one.normalized;
        float normalizedPower = targetAim.GetPower(_45Deg);
        int minPower;
        if (normalizedPower < 0 || normalizedPower > 1)
        {
            minPower = 100;
        }
        else
        {
            minPower = Mathf.CeilToInt(normalizedPower * 100f);
        }
        minPower = Mathf.Clamp(minPower, 20, 100);
        power = UnityEngine.Random.Range(minPower, 100);
        return true;
    }

    public bool SetImpactPoint()
    {
        Log("test if the fireline is empty " + shootingAngle);
        hitPoint = targetAim.CollisionPredictionStupid(shootingAngle, power / 100f);
        impactPoint = hitPoint.point;
        return true;
    }

    public bool EmptyFireLine()
    {
        if (impactPoint.x <= -999)
        {
            Log("success we hit nothing, is that a succes???");
            //very strange but we haven't hit nothing
            return false;
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
        Vector2 direction = targetAim.GetShootingAngle(power / 100f, true);
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
        Log("called check and set " + distance);
        if (distance > safeDistance)
        {
            Log("it is safe");
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
        Vector2 shake = new Vector2();
        shake.x = UnityEngine.Random.Range(-inaccuracy, inaccuracy);
        shake.y = UnityEngine.Random.Range(-inaccuracy, inaccuracy);
        shake /= 100;
        Debug.Log("shake: " + shake.x + ":" + shake.y);
        shootingAngle = shootingAngle + shake;
        shootingAngle.Normalize();
        return true;
    }

    public bool Shoot()
    {
        Log("shooting");
        shootingManager.Attack(Mathf.FloorToInt(power));
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
        if ((!emptyFireLine && !meleeOnly) || targetDistance > 90)
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
        Debug.Log(gameObject.transform.parent.gameObject.name + " supid");
        power = 100;
        return true;
    }
}