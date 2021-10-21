using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnExit : MonoBehaviour {

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player" && collision.tag!="PartOfPlayer")
        {
            Destroy(collision.gameObject);
        }
        else if (collision.tag=="Player")
        {
            PlayerHealth ph=collision.gameObject.GetComponentInChildren<PlayerHealth>();
            ph.CmdTakeDamage(ph.maxHealth, ph.gameObject);

        }
    }
}
