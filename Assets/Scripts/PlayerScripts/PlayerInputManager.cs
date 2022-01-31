using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerAim playerAim;
    private PlayerChestManager playerChestManager;
    private PlayerMovementOffline playerMovement;
    private PlayerWeaponManager_Inventory playerWeaponManager;
    private InventoryUI inventoryUI;
    private bool isABot;

    // Start is called before the first frame update
    private void Start()
    {
        isABot = GetComponent<PlayerManager>().isABot;
        playerAim = GetComponent<PlayerAim>();
        playerChestManager = GetComponent<PlayerChestManager>();
        playerMovement = GetComponent<PlayerMovementOffline>();
        playerWeaponManager = GetComponent<PlayerWeaponManager_Inventory>();
        inventoryUI = GameObject.Find("Canvas").GetComponent<InventoryUI>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isABot)
        {
            return;
        }
        if (Input.GetButtonDown("Inventory"))
        {
            inventoryUI.InitializeInventoryUI(playerWeaponManager);
            inventoryUI.OpenClose();
        }
        playerAim.dir = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Chest"))
        {
            playerChestManager.TryToOpenChest();
        }
        if (Input.GetButtonDown("Fire1"))
        {
            playerWeaponManager.ShowChargeBar();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            playerWeaponManager.HideBarAndShoot();
        }
        if (Input.GetButtonDown("Switch Left"))
        {
            playerWeaponManager.NextWeapon();
        }
        if (Input.GetButtonDown("Switch Right"))
        {
            playerWeaponManager.NextWeapon();
        }

        playerMovement.horizontalMove = Input.GetAxisRaw("Horizontal") * playerMovement.speed;
        playerMovement.jump = Input.GetButtonDown("Jump") || playerMovement.jump;
    }
}