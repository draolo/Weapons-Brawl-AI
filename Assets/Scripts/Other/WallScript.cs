using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class WallScript : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    
    [ClientRpc]
    public void RpcSetup(float x, float y, float z)
    {
        Vector3 scale = new Vector3(x, y, z); 
        transform.localScale=scale;
        this.GetComponent<BoxCollider2D>().enabled = true;
        Color spriteColor = this.GetComponent<SpriteRenderer>().color;
        spriteColor.a = 1;
        this.GetComponent<SpriteRenderer>().color = spriteColor;

    }
}
