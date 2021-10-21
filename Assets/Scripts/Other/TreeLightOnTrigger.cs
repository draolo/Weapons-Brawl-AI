using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TreeLightOnTrigger : NetworkBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidedWith = collision.gameObject;
        if (collidedWith.CompareTag("Player") && collidedWith.GetComponent<NetworkIdentity>().hasAuthority)
            transform.parent.gameObject.GetComponent<TreeScript>().TurnOnTree(true);

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collidedWith = collision.gameObject;
        if (collidedWith.CompareTag("Player") && collidedWith.GetComponent<NetworkIdentity>().hasAuthority)
            transform.parent.gameObject.GetComponent<TreeScript>().TurnOnTree(false);
    }
}
