using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAntiSlippery : MonoBehaviour
{
    public Rigidbody2D playerRB;
    public PhysicsMaterial2D Slippery;

    private void Update()
    {
        if (playerRB.gameObject.GetComponent<PlayerMovementOffline>().enabled)
        {
            playerRB.sharedMaterial = Slippery;
        }
        else
        {
            playerRB.sharedMaterial = null;
        }
    }
}