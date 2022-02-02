using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckScript : MonoBehaviour
{
    private List<string> notGroundedOnTag;
    public string[] ignoreTag = { "Destroyer" };
    private PlayerMovementOffline movementScript;

    private void Start()
    {
        notGroundedOnTag = new List<string>(ignoreTag);
        movementScript = GetComponentInParent<PlayerMovementOffline>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            movementScript.isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            movementScript.isGrounded = false;
        }
    }
}