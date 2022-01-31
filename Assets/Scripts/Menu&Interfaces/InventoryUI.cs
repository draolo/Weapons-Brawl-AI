using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : AbstractInGameInterfaces
{
    public Transform itemsParent;
    public GameObject SlotPrefab;

    public PlayerWeaponManager_Inventory inventory;
    private InventorySlot[] slots;
    private int Size = 0;

    public void InitializeInventoryUI(PlayerWeaponManager_Inventory player)
    {
        inventory = player;
        UpdateUI();
    }

    public void UpdateUI()
    {
        int i = 0;
        foreach (AbstractWeaponGeneric weapon in inventory.Weapons)
        {
            if (i >= Size)
                AddSlot();
            slots[i].AddItem(weapon);
            i++;
        }
        for (int j = i; j < Size; j++)
        {
            Destroy(slots[j].gameObject);
        }
        Size = i;
    }

    public void AddSlot()
    {
        GameObject newSlot = Instantiate(SlotPrefab);
        newSlot.transform.SetParent(itemsParent);
        newSlot.transform.localScale = Vector3.one;
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        Size++;
    }
}