using CRBT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentAI : MonoBehaviour
{
    public float aiTime = .2f;
    public float beginWaitTime = 1f;

    public bool scared;

    private BehaviorTree AI;

    public bool isTeamRed;
    public Bot bot;
    public Transform target;

    private DecisionTree dt;
    private System.Random rand;


    public OpenChestBT openBT;
    public ShootBT shootBT;
    public List<Target<PlayerHealth>> reachableTargets;
    public List<Target<AbstractChest>> reachableHealthChest;
    public List<Target<AbstractChest>> reachableReviveChest;
    public List<Target<AbstractChest>> reachableUpgradeChest;
    public List<Target<PlayerHealth>> availableTargets;
    public List<PlayerInfo> targetToRevive;
    private float reactionTime=0.5f;
    public int hpMargin= 15;

    void Start()
    {
        rand = new System.Random();
        openBT = GetComponent<OpenChestBT>();
        shootBT = GetComponent<ShootBT>();
        DTAction health = new DTAction(GoForHealth);
        DTAction revive = new DTAction(GoForRevive);
        DTAction upgrade = new DTAction(GoForUpgrade);
        DTAction enemy = new DTAction(GoForEnemy);
        DTAction longEnemy = new DTAction(GoForEnemyLong);

        DTDecision isThereAreviveChestAndADeathAlly = new DTDecision(CheckForAllyAndReviveChest);
        DTDecision isThereAreviveChestAndADeathAllyWithAKillableEnemy = new DTDecision (CheckForAllyAndReviveChest);
        DTDecision isThereOnlyOneEnemy = new DTDecision (IsTheLastEnemy);
        DTDecision isThereAReachableEnemy = new DTDecision(ThereAreReachAbleEnemy) ;
        DTDecision isThereAReachableEnemyAndAMissingUpgrade = new DTDecision(ThereAreReachAbleEnemy);
        DTDecision isThereAReachableEnemyThathCouldKill = new DTDecision (ThereAreKillableEnemy);
        DTDecision isThereAreReachableEnemyWithLessHealthThenMeOrAmIBrave = new DTDecision(CheckForLowerHpEnemyOrBravery);
        DTDecision isThereAMissingReachbleUpgrade = new DTDecision (CheckForMissingUpgrade);
        DTDecision isThereAReachbleHealthChestAndImNotFull = new DTDecision(CheckIfHealthIsFullOrThereIsAChest);
        DTDecision randomBool = new DTDecision(RandomTF) ;
        DTDecision couldIDoBoth = new DTDecision(RandomTF); //TODO DO IT PROPERLY
        //0
        isThereAReachableEnemyThathCouldKill.AddLink(true, isThereOnlyOneEnemy);
        isThereAReachableEnemyThathCouldKill.AddLink(false, isThereAreviveChestAndADeathAlly);
        //0.1
        isThereOnlyOneEnemy.AddLink(true, enemy);
        isThereOnlyOneEnemy.AddLink(false, isThereAreviveChestAndADeathAllyWithAKillableEnemy);
        //0.1.0
        isThereAreviveChestAndADeathAllyWithAKillableEnemy.AddLink(true, revive);
        isThereAreviveChestAndADeathAllyWithAKillableEnemy.AddLink(false, enemy);
        //0.0
        isThereAreviveChestAndADeathAlly.AddLink(true, revive);
        isThereAreviveChestAndADeathAlly.AddLink(false, isThereAMissingReachbleUpgrade);

        //0.0.0
        isThereAMissingReachbleUpgrade.AddLink(true, isThereAReachableEnemyAndAMissingUpgrade);
        isThereAMissingReachbleUpgrade.AddLink(false, isThereAreReachableEnemyWithLessHealthThenMeOrAmIBrave);

        //0.0.0.1
        isThereAReachableEnemyAndAMissingUpgrade.AddLink(true,couldIDoBoth);
        isThereAReachableEnemyAndAMissingUpgrade.AddLink(false,upgrade);

        //0.0.0.1.1
        couldIDoBoth.AddLink(true, upgrade);
        couldIDoBoth.AddLink(false, randomBool);

        //0.0.0.1.1.0
        randomBool.AddLink(true, upgrade);
        randomBool.AddLink(false, enemy);

        //0.0.0.0
        isThereAreReachableEnemyWithLessHealthThenMeOrAmIBrave.AddLink(true, enemy);
        isThereAreReachableEnemyWithLessHealthThenMeOrAmIBrave.AddLink(false, isThereAReachbleHealthChestAndImNotFull);

        //0.0.0.0.0
        isThereAReachbleHealthChestAndImNotFull.AddLink(true,health);
        isThereAReachbleHealthChestAndImNotFull.AddLink(false,isThereAReachableEnemy);

        //0.0.0.0.0.0
        isThereAReachableEnemy.AddLink(true, enemy);
        isThereAReachableEnemy.AddLink(false, longEnemy);

        dt = new DecisionTree(isThereAReachableEnemyThathCouldKill);

        isTeamRed = transform.parent.GetComponent<PlayerInfo>().team == Color.red;





        StartCoroutine(PlayAI());
    }



    private object RandomTF(object bundle)
    {
        return rand.Next(2) == 0;
    }

    private object CheckIfHealthIsFullOrThereIsAChest(object bundle)
    {
        PlayerHealth ph = gameObject.GetComponent<PlayerHealth>();
        if ( ph.hp + hpMargin  < ph.maxHealth)
        {
            SetUpAndFilterOutUnreachableHealthChest();
            return reachableHealthChest.Count > 0;
        }
        return false;
    }

    private object CheckForMissingUpgrade(object bundle)
    {
        SetUpAndFilterOutUnreachableUpgradeChest();
        return reachableUpgradeChest.Count > 0;
    }

    private object CheckForLowerHpEnemyOrBravery(object bundle)
    {
        SetUpAndFilterUnreachablePlayer();
        if (reachableTargets.Count <= 0)
        {
            return false;
        }

        if (!scared)
            {
                return true;
            }
        FilterPlayerWithLowerHp();
        return availableTargets.Count > 0;

    }

    private void FilterPlayerWithLowerHp()
    {
        //TODO DO IT PROPERLY
        return;
    }

    private object ThereAreKillableEnemy(object bundle)
    {
        //TODO DO IT PROPERLY
        SetUpAndFilterUnreachablePlayer();
        return false;
    }

    private object ThereAreReachAbleEnemy(object bundle)
    {
        SetUpAndFilterUnreachablePlayer();
        return reachableTargets.Count > 0;
    }

    private object IsTheLastEnemy(object bundle)
    {
        InizializePlayerTarget();
        return (availableTargets.Count() == 1);
    }

    private object CheckForAllyAndReviveChest(object bundle)
    {
        if (InizializeAllyToRevive())
        {
            SetUpandFilterOutUnreachableReviveChest();
            if (reachableReviveChest.Count > 0)
            {
                return true;
            }
        }
      
        return false;


    }

    public IEnumerator PlayAI()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            if (!(shootBT.enabled || openBT.enabled))
            {
                reachableHealthChest = null;
                reachableReviveChest = null;
                reachableUpgradeChest = null;
                reachableTargets = null;
                dt.walk();
            }
            yield return new WaitForSeconds(reactionTime);
        }
    }


    public bool InizializeAllyToRevive()
    {
        List<PlayerInfo> pinfoTarget;
        if (!isTeamRed)
        {
            pinfoTarget = MatchManager._instance.BlueTeam;
        }
        else
        {
            pinfoTarget = MatchManager._instance.RedTeam;
        }
        var deadTargets = pinfoTarget.FindAll(e => e.status == PlayerInfo.Status.dead);
        targetToRevive = deadTargets;
        return targetToRevive.Count() > 0;
    }

    public bool InizializePlayerTarget()
    {
        List<PlayerInfo> pinfoTarget;
        if (isTeamRed)
        {
            pinfoTarget = MatchManager._instance.BlueTeam;
        }
        else
        {
            pinfoTarget = MatchManager._instance.RedTeam;
        }
        var aliveTargets = pinfoTarget.FindAll(e => e.status== PlayerInfo.Status.alive);
        availableTargets = new List<Target<PlayerHealth>>();
        
        foreach(PlayerInfo p in aliveTargets ) {
            availableTargets.Add(new Target<PlayerHealth>(p.GetComponentInChildren<PlayerHealth>()));
        }
        return availableTargets.Count() > 0;
    }
   
    public void SetUpAndFilterUnreachablePlayer()
    {
        if (reachableTargets != null)
        {
            availableTargets = new List<Target<PlayerHealth>>(reachableTargets);
            return;
        }
        InizializePlayerTarget();
        Vector2i startTile = bot.mMap.GetMapTileAtPoint(bot.mPosition - bot.mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f);
        if (bot.mOnGround && !bot.IsOnGroundAndFitsPos(startTile))
        {
            if (bot.IsOnGroundAndFitsPos(new Vector2i(startTile.x + 1, startTile.y)))
                startTile.x += 1;
            else
                startTile.x -= 1;
        }
        reachableTargets = availableTargets.FindAll(e => e.CalculatePath(startTile,bot));
        availableTargets = new List<Target<PlayerHealth>>(reachableTargets);

    }

    public bool InizializeReviveChest()
    {
        List<AbstractChest> aChestRevive;
        aChestRevive = MatchManager._instance._reviveChest;
        reachableReviveChest = new List<Target<AbstractChest>>();

        foreach (AbstractChest c in aChestRevive)
        {
            reachableReviveChest.Add(new Target<AbstractChest>(c));
        }
        return reachableReviveChest.Count() > 0;

    }

    public void SetUpandFilterOutUnreachableReviveChest()
    {
        if (reachableReviveChest != null)
        {
            return;
        }
        InizializeReviveChest();
        Vector2i startTile = bot.mMap.GetMapTileAtPoint(bot.mPosition - bot.mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f);
        if (bot.mOnGround && !bot.IsOnGroundAndFitsPos(startTile))
        {
            if (bot.IsOnGroundAndFitsPos(new Vector2i(startTile.x + 1, startTile.y)))
                startTile.x += 1;
            else
                startTile.x -= 1;
        }
        reachableReviveChest = reachableReviveChest.FindAll(e => e.CalculatePath(startTile, bot));
    }

    public bool InizializeUpgradeChest()
    {
        List<AbstractChest> aChestUpgrade;
        aChestUpgrade = MatchManager._instance._upgradeChest;
        reachableUpgradeChest = new List<Target<AbstractChest>>();

        foreach (AbstractChest c in aChestUpgrade)
        {
            reachableUpgradeChest.Add(new Target<AbstractChest>(c));
        }
        return reachableUpgradeChest.Count() > 0;

        
    }

    public void SetUpAndFilterOutUnreachableUpgradeChest()
    {
        if (reachableUpgradeChest != null)
        {
            return;
        }
        InizializeUpgradeChest();
        Vector2i startTile = bot.mMap.GetMapTileAtPoint(bot.mPosition - bot.mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f);
        if (bot.mOnGround && !bot.IsOnGroundAndFitsPos(startTile))
        {
            if (bot.IsOnGroundAndFitsPos(new Vector2i(startTile.x + 1, startTile.y)))
                startTile.x += 1;
            else
                startTile.x -= 1;
        }
        reachableUpgradeChest = reachableUpgradeChest.FindAll(e => e.CalculatePath(startTile, bot));

    }

    public bool InizializeHealthChest()
    {
        List<AbstractChest> aChestHealth;
        aChestHealth = MatchManager._instance._lifeChest;
        reachableHealthChest = new List<Target<AbstractChest>>();

        foreach (AbstractChest c in aChestHealth)
        {
            reachableHealthChest.Add(new Target<AbstractChest>(c));
        }
        return reachableHealthChest.Count() > 0;

    }

    public void SetUpAndFilterOutUnreachableHealthChest()
    {
        if (reachableHealthChest != null)
        {
            return;
        }
        InizializeHealthChest();
        Vector2i startTile = bot.mMap.GetMapTileAtPoint(bot.mPosition - bot.mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f);
        if (bot.mOnGround && !bot.IsOnGroundAndFitsPos(startTile))
        {
            if (bot.IsOnGroundAndFitsPos(new Vector2i(startTile.x + 1, startTile.y)))
                startTile.x += 1;
            else
                startTile.x -= 1;
        }
        reachableHealthChest = reachableHealthChest.FindAll(e => e.CalculatePath(startTile, bot));

    }



    public bool SetClosestEnemy()
    {
        availableTargets.Sort();
        if (availableTargets.Count() > 0)
        {
            target= availableTargets[0].obj.transform;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SetClosestHealthChest()
    {
        reachableHealthChest.Sort();
        if (reachableHealthChest.Count() > 0)
        {
            target = reachableHealthChest[0].obj.transform;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SetClosestReviveChest()
    {
        reachableReviveChest.Sort();
        if (reachableReviveChest.Count() > 0)
        {
            target= reachableReviveChest[0].obj.transform;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SetClosestUpgradeChest()
    {
        reachableUpgradeChest.Sort();
        if (reachableUpgradeChest.Count > 0)
        {
            target= reachableUpgradeChest[0].obj.transform;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool StartShootingBT()
    {
        shootBT.enabled = true;
        shootBT.target = target;
        return true;
    }
    public bool StartOpenChestBT()
    {
        openBT.enabled = true;
        openBT.target = target;
        return true;
    }

    public object GoForHealth(object o)
    {
        SetClosestHealthChest();
        StartOpenChestBT();
        return null;
    }

    public object GoForRevive(object o)
    {
        SetClosestReviveChest();
        SetAllyToRevive();
        StartOpenChestBT();
        return null;
    }

    private void SetAllyToRevive()
    {
        //CHOOSE THE ON WITH THE HIGHER POITS
        gameObject.GetComponent<PlayerChestManager>().SelectAllyToResurrect(targetToRevive[0].name);
    }

    public object GoForUpgrade(object o)
    {
        SetClosestUpgradeChest();
        StartOpenChestBT();
        return null;
    }
    public object GoForEnemy(object o)
    {
        SetClosestEnemy();
        StartShootingBT();
        return null;
    }

    private object GoForEnemyLong(object bundle)
    {
        InizializePlayerTarget();
        SetClosestEnemy();
        StartShootingBT();
        return null;
    }

    public bool FilterOutAlreadyTakenUpgrade()
    {
        throw new NotImplementedException();
    }


}
