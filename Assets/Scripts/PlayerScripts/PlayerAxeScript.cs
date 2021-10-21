using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAxeScript : NetworkBehaviour
{
    private PlayerResourceScript resource;
    public GameObject player;

    private void Start()
    {
        resource = player.GetComponent<PlayerResourceScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool treeHitted = collision.gameObject.CompareTag("Tree");
        bool wallHitted = collision.gameObject.CompareTag("Wall");

        if (treeHitted || wallHitted)
        {
            if(treeHitted)
                resource.CmdAddResouces(50);

            NetworkServer.Destroy(collision.gameObject);
        }
    }

}
