using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentAI : MonoBehaviour
{
    public OpenChestBT openBT;
    public List<Target<PlayerInfo>> availableTargets;
    public List<PlayerInfo> targetToRevive;
    public List<Target<AbstractChest>> availableHealthChest;
    public List<Target<AbstractChest>> availableReviveChest;
    public List<Target<AbstractChest>> availableUpgradeChest;
    private bool isTeamRed;
    public Bot bot;
    
    void Start()
    {
        StartCoroutine(OpenClosestChest());
    }

    public IEnumerator OpenClosestChest()
    {
        yield return new WaitForSeconds(1);
        InizializeHealthChest();
        FilterOutUnreachableHealthChest();
        AbstractChest t = GetClosestHealthChest();
        openBT.enabled = true;
        openBT.target = t.transform;
    }


    public void InizializePlayerTarget()
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
        availableTargets = new List<Target<PlayerInfo>>();
        
        foreach(PlayerInfo p in aliveTargets ) {
            availableTargets.Add(new Target<PlayerInfo>(p));
        }
    }
   
    public void FilterOutUnreachablePlayer()
    {
        Vector2i startTile = bot.mMap.GetMapTileAtPoint(bot.mPosition - bot.mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f);
        if (bot.mOnGround && !bot.IsOnGroundAndFitsPos(startTile))
        {
            if (bot.IsOnGroundAndFitsPos(new Vector2i(startTile.x + 1, startTile.y)))
                startTile.x += 1;
            else
                startTile.x -= 1;
        }
        availableTargets = availableTargets.FindAll(e => e.CalculatePath(startTile,bot));
    }

    public void InizializeReviveChest()
    {
        List<AbstractChest> aChestRevive;
        aChestRevive = MatchManager._instance._reviveChest;
        availableReviveChest = new List<Target<AbstractChest>>();

        foreach (AbstractChest c in aChestRevive)
        {
            availableReviveChest.Add(new Target<AbstractChest>(c));
        }
    }

    public void FilterOutUnreachableReviveChest()
    {
        Vector2i startTile = bot.mMap.GetMapTileAtPoint(bot.mPosition - bot.mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f);
        if (bot.mOnGround && !bot.IsOnGroundAndFitsPos(startTile))
        {
            if (bot.IsOnGroundAndFitsPos(new Vector2i(startTile.x + 1, startTile.y)))
                startTile.x += 1;
            else
                startTile.x -= 1;
        }
        availableReviveChest = availableReviveChest.FindAll(e => e.CalculatePath(startTile, bot));
    }

    public void InizializeUpgradeChest()
    {
        List<AbstractChest> aChestUpgrade;
        aChestUpgrade = MatchManager._instance._upgradeChest;
        availableUpgradeChest = new List<Target<AbstractChest>>();

        foreach (AbstractChest c in aChestUpgrade)
        {
            availableUpgradeChest.Add(new Target<AbstractChest>(c));
        }
    }

    public void FilterOutUnreachableUpgradeChest()
    {
        Vector2i startTile = bot.mMap.GetMapTileAtPoint(bot.mPosition - bot.mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f);
        if (bot.mOnGround && !bot.IsOnGroundAndFitsPos(startTile))
        {
            if (bot.IsOnGroundAndFitsPos(new Vector2i(startTile.x + 1, startTile.y)))
                startTile.x += 1;
            else
                startTile.x -= 1;
        }
        availableUpgradeChest = availableUpgradeChest.FindAll(e => e.CalculatePath(startTile, bot));
    }

    public void InizializeHealthChest()
    {
        List<AbstractChest> aChestHealth;
        aChestHealth = MatchManager._instance._lifeChest;
        availableHealthChest = new List<Target<AbstractChest>>();

        foreach (AbstractChest c in aChestHealth)
        {
            availableHealthChest.Add(new Target<AbstractChest>(c));
        }
    }

    public void FilterOutUnreachableHealthChest()
    {
        Debug.Log("map "+ bot.mMap);
        Vector2i startTile = bot.mMap.GetMapTileAtPoint(bot.mPosition - bot.mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f);
        if (bot.mOnGround && !bot.IsOnGroundAndFitsPos(startTile))
        {
            if (bot.IsOnGroundAndFitsPos(new Vector2i(startTile.x + 1, startTile.y)))
                startTile.x += 1;
            else
                startTile.x -= 1;
        }
        availableHealthChest = availableHealthChest.FindAll(e => e.CalculatePath(startTile, bot));
    }



    public PlayerInfo GetClosestEnemy()
    {
        availableTargets.Sort();
        return availableTargets[0].obj;
    }

    public AbstractChest GetClosestHealthChest()
    {
        availableHealthChest.Sort();
        return availableHealthChest[0].obj;
    }

    public AbstractChest GetClosestReviveChest()
    {
        availableReviveChest.Sort();
        return availableReviveChest[0].obj;
    }

    public AbstractChest GetClosestUpgradeChest()
    {
        availableUpgradeChest.Sort();
        return availableUpgradeChest[0].obj;
    }

    public void FilterOutAlreadyTakenUpgrade()
    {
        throw new NotImplementedException();
    }


}
