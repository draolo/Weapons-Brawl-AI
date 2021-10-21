using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : AbstractInGameInterfaces {

    public Transform itemsParent;
    public GameObject Player;
    public GameObject SlotPrefab;

    private PlayerWeaponManager_Inventory inventory;
    private InventorySlot[] slots;
    private int Size = 0;

    public void InitializeInventoryUI(GameObject player)
    {
        Player = player;
        inventory = Player.GetComponent<PlayerWeaponManager_Inventory>();
        inventory.onItemChangedCallBack += UpdateUI;

        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
            OpenClose();
    }

    public void UpdateUI()
    {
        int i = 0;
        foreach(AbstractWeaponGeneric weapon in inventory.Weapons)
        {
            if (i >= Size)
                addSlot();
            slots[i].AddItem(weapon);
            i++;
        }
    }

    public void addSlot()
    {
        GameObject newSlot = Instantiate(SlotPrefab);
        newSlot.transform.SetParent(itemsParent);
        newSlot.transform.localScale = Vector3.one;
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        Size++;
    }
}
