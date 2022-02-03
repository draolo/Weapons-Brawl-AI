using CRBT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentAI : MonoBehaviour
{
    [SerializeField] private float reactionTime = 0.5f;
    [SerializeField] private int hpMargin = 15;
    [SerializeField] private int lowHealthHp = 20;

    private OpenChestBT openBT;
    private ShootBT shootBT;
    private Bot bot;
    private PlayerHealth playerHealth;
    private PlayerWeaponManager_Inventory inventory;

    private TargetManager<PlayerHealth> enemyTargets;
    private TargetManager<AbstractChest> healthChest;
    private TargetManager<AbstractChest> reviveChest;
    private TargetManager<AbstractChest> upgradeChest;
    private List<PlayerInfo> targetToRevive;

    private System.Random rand;
    private bool scared;
    private Color team;
    private bool waitingActionToEnd;
    private Transform target;
    private int revision;

    private IDTNode root;
    private DecisionTree dt;

    private void Awake()
    {
        rand = new System.Random();
        openBT = GetComponent<OpenChestBT>();
        shootBT = GetComponent<ShootBT>();
        bot = GetComponent<Bot>();
        playerHealth = GetComponent<PlayerHealth>();
        inventory = GetComponent<PlayerWeaponManager_Inventory>();
        scared = rand.Next(2) == 0;

        revision = -1;
        waitingActionToEnd = true;
        target = null;

        DTAction health = new DTAction(GoForHealth);
        DTAction revive = new DTAction(GoForRevive);
        DTAction upgrade = new DTAction(GoForUpgrade);
        DTAction enemy = new DTAction(GoForEnemy);
        DTAction longEnemy = new DTAction(GoForEnemyLong);
        DTAction nothing = new DTAction(DoNothing);

        DTDecision isThereAreviveChestAndADeathAlly = new DTDecision(CheckForAllyAndReviveChest);
        DTDecision isThereAreviveChestAndADeathAllyWithAKillableEnemy = new DTDecision(CheckForAllyAndReviveChest);
        DTDecision isThereOnlyOneEnemy = new DTDecision(IsTheLastEnemy);
        DTDecision isThereAReachableEnemy = new DTDecision(ThereAreReachAbleEnemy);
        DTDecision isThereAReachableEnemyAndAMissingUpgrade = new DTDecision(ThereAreReachAbleEnemy);
        DTDecision isThereAReachableEnemyThathCouldKill = new DTDecision(ThereAreKillableEnemy);
        DTDecision isThereAreReachableEnemyWithLessHealthThenMeOrAmIBrave = new DTDecision(CheckForLowerHpEnemyOrBravery);
        DTDecision isThereAMissingReachbleUpgrade = new DTDecision(CheckForMissingUpgrade);
        DTDecision isThereAReachbleHealthChestAndImNotFull = new DTDecision(CheckIfHealthIsFullOrThereIsAChest);
        DTDecision randomBool = new DTDecision(RandomTF);
        DTDecision couldIDoBoth = new DTDecision(CouldIDoUpgradeAndEnemy);
        DTDecision canAttack = new DTDecision(CanAttack);
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
        isThereAReachableEnemyAndAMissingUpgrade.AddLink(true, couldIDoBoth);
        isThereAReachableEnemyAndAMissingUpgrade.AddLink(false, upgrade);

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
        isThereAReachbleHealthChestAndImNotFull.AddLink(true, health);
        isThereAReachbleHealthChestAndImNotFull.AddLink(false, isThereAReachableEnemy);

        //0.0.0.0.0.0
        isThereAReachableEnemy.AddLink(true, enemy);
        isThereAReachableEnemy.AddLink(false, canAttack);

        //0.0.0.0.0.0.0
        canAttack.AddLink(true, longEnemy);
        canAttack.AddLink(false, nothing);

        root = isThereAReachableEnemyThathCouldKill;
    }

    private void Start()
    {
        team = transform.parent.GetComponent<PlayerInfo>().team;
        InizializeEnemyTarget();
        InizializeHealthChest();
        InizializeReviveChest();
        InizializeUpgradeChest();
    }

    private void OnEnable()
    {
        StopAndStart();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        shootBT.StopBehavior();
        openBT.StopBehavior();
        waitingActionToEnd = false;
    }

    public void InizializeEnemyTarget()
    {
        List<PlayerInfo> pinfoTarget = new List<PlayerInfo>();
        foreach (Color color in MatchManager._instance.teams)
        {
            if (color != team)
            {
                pinfoTarget.AddRange(MatchManager._instance.teamMembers[color]);
            }
        }
        var aliveTargets = pinfoTarget.FindAll(e => e.status == PlayerInfo.Status.alive);
        List<PlayerHealth> enemies = new List<PlayerHealth>();

        foreach (PlayerInfo p in aliveTargets)
        {
            enemies.Add(p.GetComponentInChildren<PlayerHealth>());
        }
        enemyTargets = new TargetManager<PlayerHealth>(enemies, bot);
    }

    public void InizializeReviveChest()
    {
        ResurrectionChest[] revives = GameObject.FindObjectsOfType<ResurrectionChest>();
        List<AbstractChest> revivesList = new List<AbstractChest>(revives);
        reviveChest = new TargetManager<AbstractChest>(revivesList, bot);
    }

    public void InizializeUpgradeChest()
    {
        WeaponChestScript[] upgrades = GameObject.FindObjectsOfType<WeaponChestScript>();
        List<AbstractChest> uplgradesList = new List<AbstractChest>(upgrades);
        upgradeChest = new TargetManager<AbstractChest>(uplgradesList, bot);
        FilterOutAlreadyTakenUpgrade();
    }

    public void InizializeHealthChest()
    {
        LifeChest[] hps = GameObject.FindObjectsOfType<LifeChest>();
        List<AbstractChest> hpsList = new List<AbstractChest>(hps);
        healthChest = new TargetManager<AbstractChest>(hpsList, bot);
    }

    public void StopAndStart()
    {
        StopAllCoroutines();
        shootBT.StopBehavior();
        openBT.StopBehavior();
        waitingActionToEnd = false;
        revision++;
        dt = new DecisionTree(root);
        StartCoroutine(PlayAI());
    }

    public void FindNewAction()
    {
        waitingActionToEnd = false;
    }

    public IEnumerator PlayAI()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            if (!waitingActionToEnd)  //todo find a new way
            {
                try
                {
                    dt.walk();
                }
                catch (MissingReferenceException mre)
                {
                    Debug.Log(mre);
                }
            }
            yield return new WaitForSeconds(reactionTime);
        }
    }

    private object RandomTF(object bundle)
    {
        return rand.Next(2) == 0;
    }

    private object CheckIfHealthIsFullOrThereIsAChest(object bundle)
    {
        if (playerHealth.hp + hpMargin < playerHealth.maxHealth)
        {
            return healthChest.HasAReachAbleTarget(GetStartTile(), revision);
        }
        return false;
    }

    private object CheckForMissingUpgrade(object bundle)
    {
        //todo filter already taken targets
        return upgradeChest.HasAReachAbleTarget(GetStartTile(), revision);
    }

    //TODO REDO
    private object CheckForLowerHpEnemyOrBravery(object bundle)
    {
        if (!enemyTargets.HasAReachAbleTarget(GetStartTile(), revision))
        {
            return false;
        }
        List<Target<PlayerHealth>> playerWithLowerHp = enemyTargets.GetReachable(GetStartTile(), revision).FindAll(e => e.obj.hp <= playerHealth.hp);
        if ((playerWithLowerHp.Count <= 0) && (!scared))
        {
            return true;
        }
        return playerWithLowerHp.Count > 0;
    }

    private object ThereAreKillableEnemy(object bundle)
    {
        List<Target<PlayerHealth>> killableEnemies = enemyTargets.GetReachable(GetStartTile(), revision).FindAll(e => e.obj.hp <= lowHealthHp);
        return killableEnemies.Count() > 0;
    }

    private object ThereAreReachAbleEnemy(object bundle)
    {
        if (!inventory.canAttack)
        {
            return false;
        }
        return enemyTargets.HasAReachAbleTarget(GetStartTile(), revision);
    }

    private object IsTheLastEnemy(object bundle)
    {
        return (enemyTargets.NumberOfTargets() == 1);
    }

    private object CanAttack(object bundle)
    {
        return inventory.canAttack;
    }

    private object CheckForAllyAndReviveChest(object bundle)
    {
        if (SearchAllyToRevive())
        {
            return reviveChest.HasAReachAbleTarget(GetStartTile(), revision);
        }

        return false;
    }

    public bool SearchAllyToRevive()
    {
        List<PlayerInfo> pinfoTarget;

        pinfoTarget = MatchManager._instance.teamMembers[team];

        var deadTargets = pinfoTarget.FindAll(e => e.status == PlayerInfo.Status.dead);
        targetToRevive = deadTargets;
        return targetToRevive.Count() > 0;
    }

    public object CouldIDoUpgradeAndEnemy(object bundle)
    {
        //KISS no complex things, if we have a lot of time yes otherwise no
        float marginTime = 12f;
        float time = MatchManager._instance.waiting;

        float d = upgradeChest.GetClosest(GetStartTile(), revision).distance;
        return d + marginTime < time;
    }

    private void SetAllyToRevive()
    {
        //CHOOSE THE ONE WITH THE HIGHER POITS
        List<PlayerInfo> realPlayers = targetToRevive.FindAll((x) => !x.isAbot);
        if (realPlayers.Count > 0)
        {
            targetToRevive = realPlayers;
        }
        targetToRevive.Sort((a, b) => b.GetPoints() - a.GetPoints());
        gameObject.GetComponent<PlayerChestManager>().SetAllyToResurrectBot(targetToRevive[0].pname);
    }

    public bool StartShootingBT()
    {
        shootBT.target = target;
        shootBT.StartBehavior();
        waitingActionToEnd = true;
        return true;
    }

    public bool StartOpenChestBT()
    {
        openBT.target = target;
        openBT.StartBehavior();
        waitingActionToEnd = true;
        return true;
    }

    public object GoForHealth(object o)
    {
        target = healthChest.GetClosest(GetStartTile(), revision).transform;
        StartOpenChestBT();
        return null;
    }

    public object GoForRevive(object o)
    {
        target = reviveChest.GetClosest(GetStartTile(), revision).transform;
        SetAllyToRevive();
        StartOpenChestBT();
        return null;
    }

    public object GoForUpgrade(object o)
    {
        target = upgradeChest.GetClosest(GetStartTile(), revision).transform;
        StartOpenChestBT();
        return null;
    }

    public object GoForEnemy(object o)
    {
        target = enemyTargets.GetClosest(GetStartTile(), revision).transform;
        StartShootingBT();
        return null;
    }

    private object GoForEnemyLong(object bundle)
    {
        Target<PlayerHealth> playerHealthTarget = enemyTargets.GetClosest(GetStartTile(), revision, false);
        if (playerHealthTarget == null)
        {
            return null;
        }
        target = playerHealthTarget.transform;
        StartShootingBT();
        return null;
    }

    private object DoNothing(object bundle)
    {
        return null;
    }

    public void FilterOutAlreadyTakenUpgrade()
    {
        Predicate<Target<AbstractChest>> predicate = new Predicate<Target<AbstractChest>>(e => !DidIHaveThisUpgrade((WeaponChestScript)e.obj));
        upgradeChest.FilterTarget(predicate);
    }

    private bool DidIHaveThisUpgrade(WeaponChestScript chest)
    {
        AbstractWeaponGeneric weapon = chest.Weapon.GetComponent<AbstractWeaponGeneric>();
        List<AbstractWeaponGeneric> sameClassWeapon = inventory.Weapons.FindAll(e => e.GetType() == weapon.GetType());
        return sameClassWeapon.Count > 0;
    }

    private Vector2i GetStartTile()
    {
        Vector2i startTile = bot.mMap.GetMapTileAtPoint(bot.mPosition - bot.mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f);
        if (bot.mOnGround && !bot.IsOnGroundAndFitsPos(startTile))
        {
            if (bot.IsOnGroundAndFitsPos(new Vector2i(startTile.x + 1, startTile.y)))
                startTile.x += 1;
            else
                startTile.x -= 1;
        }
        return startTile;
    }
}