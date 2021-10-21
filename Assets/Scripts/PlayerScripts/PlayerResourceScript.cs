using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerResourceScript : NetworkBehaviour {

    private ResourceUI UI;
    public int resources = 100;

    private bool canAdd = true;

    private void Start()
    {
        UI = GetGameObjectInRoot("Canvas").GetComponent<ResourceUI>();
        UpdateUI();
    }

    [Command]
    public void CmdAddResouces(int amount)
    {
        RpcAddResources(amount);
    }

    [ClientRpc]
    private void RpcAddResources(int amount)
    {
        AddResources(amount);
    }

    private void AddResources(int amount)
    {
        if (canAdd)
        {
            canAdd = false;
            resources += amount;
            UpdateUI();
            StartCoroutine(enableCanAdd());
        }
    }

    IEnumerator enableCanAdd()
    {
        yield return new WaitForSeconds(0.2f);
        canAdd = true;
    }


    public bool UseResource(int amount)
    {
        if (resources >= amount)
        {
            CmdAddResouces(-amount);
            return true;
        }
        return false;
    }

    private void UpdateUI()
    {
        if (hasAuthority)
            UI.SetResourceUI(resources);
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
