using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour {

    public Sprite treeOff;
    public Sprite treeOn;

    public GameObject PressQ;

    public void TurnOnTree(bool yesno)
    {
        if (yesno)
        {
            GetComponent<SpriteRenderer>().sprite = treeOn;
            PressQ.SetActive(true);
        }

        else
        {
            GetComponent<SpriteRenderer>().sprite = treeOff;
            PressQ.SetActive(false);
        }

    }
}
