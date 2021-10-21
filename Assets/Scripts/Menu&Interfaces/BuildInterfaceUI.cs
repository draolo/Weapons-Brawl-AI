using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildInterfaceUI : AbstractInGameInterfaces {

    public BuildingController buildManager;

	void Update () {

        if (Input.GetButtonDown("Build"))
            OpenClose();  

    }

    public override void OpenClose()
    {
        if (isActive)
            Close();
        else
            Open();

    }

    public override void Close()
    {
        if (isActive)
        {
            base.Close();
            buildManager.ChangeBuildingStatus();
        }

    }

    public override void Open()
    {
        if (!isActive)
        {
            base.Open();
            buildManager.ChangeBuildingStatus();
        }
    }

    public void SelectBuilding(int rotation)
    {
        buildManager.zRotation = rotation;
    }

    internal void InitializeInventoryUI(GameObject player)
    {
        buildManager = player.GetComponent<BuildingController>();
    }
}
