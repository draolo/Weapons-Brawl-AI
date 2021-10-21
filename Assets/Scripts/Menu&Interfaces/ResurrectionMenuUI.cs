using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ResurrectionMenuUI : AbstractInGameInterfaces {

    public GameObject ResurrectButtonPrefab;
    public Transform ItemsParent;
    public PlayerChestManager p;
    public Text SubTitle;

    public void Close(bool complete)
    {
        base.Close();

        Button[] buttons = ItemsParent.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
            Destroy(button.gameObject);

        if (!complete && p)
            p.AbortInteraction();
    }

    public override void Close()
    {
        this.Close(false);
    }


    public void AddResurrectButton(string text)
    {
        GameObject button = Instantiate(ResurrectButtonPrefab);
        button.transform.SetParent(ItemsParent);
        Text t = button.GetComponentInChildren<Text>();
        t.text = text;
        Button buttonComponenet = button.GetComponent<Button>();
        buttonComponenet.onClick.AddListener(() => { p.SelectAllyToResurrect(text); this.Close(true); });
        button.transform.localScale = Vector3.one;
    }

    internal void InizializeInventoryUI(GameObject player)
    {
        p = player.GetComponent<PlayerChestManager>();
    }

    public void SetSubTitle(string message)
    {
        SubTitle.text = message;
    }
}
