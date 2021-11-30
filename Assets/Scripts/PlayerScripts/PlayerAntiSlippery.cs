using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAntiSlippery : MonoBehaviour {

    public Rigidbody2D Player;
    public PhysicsMaterial2D Slippery;

    private void Update()
    {
        if (Player.gameObject.GetComponent<PlayerMovementOffline>().isActiveAndEnabled)
        {
            Player.sharedMaterial = Slippery;
        }
        else
        {
            Player.sharedMaterial = null;
        }
    }
}
