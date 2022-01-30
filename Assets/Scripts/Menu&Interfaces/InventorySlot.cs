using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InventorySlot : MonoBehaviour
{
    public Image icon;

    public AbstractWeaponGeneric item;

    public void AddItem(AbstractWeaponGeneric newItem)
    {
        item = newItem;
        icon.sprite = item.info.icon;
        icon.enabled = true;
    }

    public void SwitchWeapon()
    {
        InventoryUI inventory = GetGameObjectInRoot("Canvas").GetComponent<InventoryUI>();
        PlayerWeaponManager_Inventory weaponManager = inventory.Player.GetComponent<PlayerWeaponManager_Inventory>();
        int wid = weaponManager.Weapons.FindIndex(a => a == item);
        weaponManager.CmdSwitchWeapon(wid);
        inventory.OpenClose();
    }


















    private GameObject GetGameObjectInRoot(string objname)
    {
        GameObject[] root = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in root)
            if (obj.name == objname)
                return obj;
        return null;
    }
}