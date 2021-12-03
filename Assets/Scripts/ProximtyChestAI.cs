using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximtyChestAI : MonoBehaviour
{
    public CircleCollider2D circle;
    public GameObject player;
    private PlayerChestManager chestManager;
    private PlayerWeaponManager_Inventory inventory;
    private PlayerHealth health;
    private bool isTeamRed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.CompareTo("Chest") == 0)
        {
            AbstractChest chest = collision.gameObject.GetComponent<AbstractChest>();
            //TODO VERIFICA SE QUALCUNO SI è GIA' PRENOTATO LA CASSA
            ManageChest(chest);
        }
    }

    private void ManageChest(AbstractChest chest)
    {
        switch (chest.type)
        {
            case AbstractChest.ChestType.Health:
                {
                    if (health.hp + 15 < health.maxHealth)
                    {
                        chestManager.TryToOpenChest();
                    }
                    break;
                }
            case AbstractChest.ChestType.Upgrade:
                {
                    AbstractWeaponGeneric weapon = ((WeaponChestScript)chest).Weapon.GetComponent<AbstractWeaponGeneric>();
                    List<AbstractWeaponGeneric> sameClassWeapon = inventory.Weapons.FindAll(e => e.GetType() == weapon.GetType());
                    if (sameClassWeapon.Count == 0)
                    {
                        chestManager.TryToOpenChest();
                    }
                    break;
                }
            case AbstractChest.ChestType.Revive:
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
                    List<PlayerInfo> targetToRevive = pinfoTarget.FindAll(e => e.status == PlayerInfo.Status.dead);
                    if (targetToRevive.Count > 0)
                    {
                        chestManager.SetAllyToResurrectBot(targetToRevive[0].pname);
                        chestManager.TryToOpenChest();
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        chestManager = player.GetComponent<PlayerChestManager>();
        inventory = player.GetComponent<PlayerWeaponManager_Inventory>();
        health = player.GetComponent<PlayerHealth>();
        isTeamRed = transform.GetComponentInParent<PlayerInfo>().team == Color.red;
        circle.radius = chestManager.InteractionRadius;
    }
}