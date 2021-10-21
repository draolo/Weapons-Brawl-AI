using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounter : MonoBehaviour {

    private Dictionary<Color, int> playerCounter;
    private List<GameObject> inside = new List<GameObject>();


    void Start () {
        playerCounter = new Dictionary<Color, int>();	
	}
	

    public int GetPlayerCounter(Color c)
    {
        if (!playerCounter.ContainsKey(c))
            playerCounter[c] = 0;

        return playerCounter[c];
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player"&& (!inside.Contains(other.gameObject)))
        {
            inside.Add(other.gameObject);
            Color team=other.gameObject.GetComponent<PlayerManager>().GetTeam();
            if (!playerCounter.ContainsKey(team))
            {
                playerCounter[team] = 0;
            }
            playerCounter[team]++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            inside.Remove(other.gameObject);
            Color team = other.gameObject.GetComponent<PlayerManager>().GetTeam();
            if (!playerCounter.ContainsKey(team))
            {
                playerCounter[team] = 0;
            }
            playerCounter[team]--;
            playerCounter[team] = Mathf.Max(playerCounter[team], 0);
        }
    }

}
