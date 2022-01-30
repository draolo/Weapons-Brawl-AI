using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchInfo : MonoBehaviour
{
    public int playerPerTeam = 3;
    public int realBluePlayer = 0;
    public int realRedPlayer = 0;

    // Start is called before the first frame update
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}