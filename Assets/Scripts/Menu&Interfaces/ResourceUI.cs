using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ResourceUI : AbstractInGameInterfaces {

    public TextMeshProUGUI ResourceText;

    public void SetResourceUI(int amount)
    {
        if (ResourceText.text == amount.ToString())
            return;

        ResourceText.text = amount.ToString();
        PlayBumpAnimation();
    }

    private void PlayBumpAnimation()
    {
        ResourceText.GetComponent<Animator>().Play("Bump", -1, 0f);
    }
}
