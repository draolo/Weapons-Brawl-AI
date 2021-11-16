using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentAI : MonoBehaviour
{
    public List<Target<PlayerInfo>> availableTargets;
    public List<PlayerInfo> targetToRevive;
    public List<Target<AbstractChest>> availableHealthChest;
    public List<Target<AbstractChest>> availableReviveChest;
    public List<Target<AbstractChest>> availableUpgradeChest;
    private bool isTeamRed;
    public Bot bot;

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






    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
