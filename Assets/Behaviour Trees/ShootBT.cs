using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CRBT;

public class ShootBT : MonoBehaviour
{
    public float aiTime = .2f;
    public float beginWaitTime = 1f;
    private IBTTask root;
    private BehaviorTree AI;

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
    private Vector2 impactPoint;

    // Start is called before the first frame update
    private void Awake()
    {
        playerMovementOffline = GetComponent<PlayerMovementOffline>();
        targetAim = GetComponent<TargetAim>();
        bot = gameObject.GetComponent<Bot>();
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        shootingManager = gameObject.GetComponent<PlayerWeaponManager_Inventory>();

        BTAction setTarget = new BTAction(SetTarget);
        BTAction setPath = new BTAction(IsThereAPathToTheTarget);
        BTAction startBot = new BTAction(Move);
        BTAction stop = new BTAction(Stop);
        BTAction aim = new BTAction(SetAim);
        BTAction shoot = new BTAction(Shoot);
        BTAction shake = new BTAction(AimShaker);
        BTAction setUpWeapon = new BTAction(SetWeapon);
        BTAction faceTheTarget = new BTAction(FaceTheTarget);

        BTCondition targetAlive = new BTCondition(IsTargetAlive);
        BTCondition couldSeeTheTarget = new BTCondition(LineOfSight);
        BTCondition samePosition = new BTCondition(TargetPositionEqual);
        BTCondition straightLine = new BTCondition(TestAndSetStraight);
        BTCondition lobbedLine = new BTCondition(TestAndSetLobbed);
        BTCondition emptyFireline = new BTCondition(EmptyFireLine);
        BTCondition isMovinig = new BTCondition(IsItMoving);
        BTCondition isGrounded = new BTCondition(IsItOnGround);
        BTDecorator waitMovementEnd = new BTDecoratorUntilFail(isMovinig);

        BTSequence safeShoot = new BTSequence(new IBTTask[] { setUpWeapon, targetAlive, shoot });
        BTSequence emptyStraightFireLine = new BTSequence(new IBTTask[] { isGrounded, straightLine, emptyFireline });
        BTSequence emptyLobbedFireLine = new BTSequence(new IBTTask[] { isGrounded, lobbedLine, emptyFireline });
        BTSelector isThereAFireLine = new BTSelector(new IBTTask[] { emptyStraightFireLine, emptyLobbedFireLine });
        BTSequence shootFarAway = new BTSequence(new IBTTask[] { stop, waitMovementEnd, faceTheTarget, isThereAFireLine, aim, safeShoot });
        BTSequence safeSetPath = new BTSequence(new IBTTask[] { stop, setPath });

        BTSelector pathVerifier = new BTSelector(new IBTTask[] { samePosition, safeSetPath });
        BTDecorator notInLine = new BTDecoratorInverter(couldSeeTheTarget);
        BTDecorator notEmptyLine = new BTDecoratorInverter(isThereAFireLine);

        BTSequence endMovementAndTest = new BTSequence(new IBTTask[] { isGrounded, stop, waitMovementEnd, notEmptyLine, startBot });

        BTSelector movementConditions = new BTSelector(new IBTTask[] { notInLine, notEmptyLine });

        BTDecorator movingCycle = new BTDecoratorUntilFail(movementConditions);

        BTSequence getCloser = new BTSequence(new IBTTask[] { safeSetPath, startBot, movingCycle, shootFarAway });

        BTRandomSelector standardBehaviour = new BTRandomSelector(new IBTTask[] { shootFarAway, getCloser });

        BTSequence desperateBehaviour = new BTSequence(new IBTTask[] { straightLine, aim, safeShoot });

        BTSelector shootingStrategies = new BTSelector(new IBTTask[] { standardBehaviour, desperateBehaviour });

        root = new BTSequence(new IBTTask[] { setTarget, shootingStrategies });
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        targetAim.target = target;
        targetAim.enabled = true;
        try
        {
            targetInfo = target.gameObject.GetComponentInParent<PlayerInfo>();
            AI = new BehaviorTree(root);
            StartCoroutine(ShootTarget());
        }
        catch (MissingReferenceException)
        {
            this.enabled = false;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        target = null;
        targetAim.target = null;
    }

    public IEnumerator ShootTarget()
    {
        yield return new WaitForSeconds(beginWaitTime);
        bool step;
        do
        {
            try
            {
                if (!IsTargetAlive())
                {
                    step = false;
                }
                else
                {
                    step = AI.Step();
                }
            }
            catch (MissingReferenceException mre)
            {
                Debug.Log(mre);
                step = false;
            }
            yield return new WaitForSeconds(aiTime);
        } while (step);
        this.enabled = false;
    }

    public bool LineOfSight()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, projectileObstacle);
        if (hit.collider == null)
        {
            return true;
        }
        return false;
    }

    public bool SetAim()
    {
        targetAim.SetAim(shootingAngle);
        return true;
    }

    public bool TestAndSetStraight()
    {
        Vector2 direction = targetAim.Aim();
        if (direction.x <= -999)
        {
            return false;
        }
        shootingAngle = direction;
        return true;
    }

    public bool EmptyFireLine()
    {
        RaycastHit2D hitPoint = targetAim.CollisionPredictionStupid(shootingAngle);
        impactPoint = hitPoint.point;
        if (impactPoint.x <= -999)
        {
            //very strange but we haven't hit nothing
            return true;
        }
        if (hitPoint.collider.gameObject == target.gameObject)
        {
            return true;
        }
        return false;
    }

    public bool TestAndSetLobbed()
    {
        Vector2 direction = targetAim.Aim(true);
        if (direction.x <= -999)
        {
            return false;
        }
        shootingAngle = direction;
        return true;
    }

    public bool IsThereAPathToTheTarget()
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

    public bool Stop()
    {
        bot.ChangeAction(Bot.BotAction.None);
        return true;
    }

    public bool Move()
    {
        bot.mCurrentNodeId = 1;

        bot.ChangeAction(Bot.BotAction.MoveTo);

        bot.mFramesOfJumping = bot.GetJumpFramesForNode(0);
        return true;
    }

    public bool TargetPositionEqual()
    {
        return targetOld == (Vector2)target.position;
    }

    public bool AimShaker()
    {
        //TODO

        return true;
    }

    public bool Shoot()
    {
        shootingManager.CmdAttack(100);
        return true;
    }

    public bool IsItMoving()
    {
        return (_rigidbody.velocity != new Vector2(0, 0));
    }

    public bool IsItOnGround()
    {
        return bot.mOnGround;
    }

    public bool FaceTheTarget()
    {
        Vector2 dir = target.position - transform.position;
        playerMovementOffline.FaceTowards(dir.x);
        return true;
    }

    public bool SetTarget()
    {
        targetAim.SetTarget(target);
        return true;
    }

    public bool IsTargetAlive()
    {
        return targetInfo.status == PlayerInfo.Status.alive;
    }

    public bool SetWeapon()
    {
        float targetDistance = Vector2.Distance(transform.position, target.position);
        float impactPointDistance = Vector2.Distance(transform.position, impactPoint);
        bool meleeOnly = targetDistance < targetAim.firePointDistance;
        bool emptyFireLine = EmptyFireLine();
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

        return false;
    }

    private bool DoesIHitMyselfWithThisWeapon(AbstractWeaponBulletBased w, float impactPointDistance)
    {
        AbstractBulletExplosive bullet = w.bulletPrefab.GetComponent<AbstractBulletExplosive>();
        return impactPointDistance <= bullet.ExplosionRadius;
    }
}