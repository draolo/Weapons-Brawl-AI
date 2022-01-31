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
    private Color team;
    private bool operative;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (operative && collision.tag.CompareTo("Chest") == 0)
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

                    pinfoTarget = MatchManager._instance.teamMembers[team];

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

    private void OnEnable()
    {
        operative = true;
    }

    private void OnDisable()
    {
        operative = false;
    }

    // Start is called before the first frame update
    private void Start()
    {
        chestManager = player.GetComponent<PlayerChestManager>();
        inventory = player.GetComponent<PlayerWeaponManager_Inventory>();
        health = player.GetComponent<PlayerHealth>();
        team = transform.GetComponentInParent<PlayerInfo>().team;
        circle.radius = chestManager.InteractionRadius;
    }
}