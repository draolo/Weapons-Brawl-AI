using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChestLv1 : WeaponChestScript {

    public SpriteRenderer Circle1;
    public PlayerCounter ChestArea;

    private void Update()
    {
        int nred = ChestArea.GetPlayerCounter(Color.red);
        int nblue = ChestArea.GetPlayerCounter(Color.blue);
        int nCircleOn = Mathf.Max(nred, nblue);

        if(nCircleOn == 0)
        {
            Circle1.color = Color.gray;
        }
        else if (nCircleOn == 1)
        {
            Circle1.color = Color.yellow;
        }
    }


}
