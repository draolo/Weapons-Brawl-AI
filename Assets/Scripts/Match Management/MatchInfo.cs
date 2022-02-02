using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchInfo : MonoBehaviour
{
    public int playerPerTeam = 3;
    public Dictionary<Color, int> numberOfRealPlayerPerTeam;

    // Start is called before the first frame update
    private void Start()
    {
        numberOfRealPlayerPerTeam = new Dictionary<Color, int>();

        numberOfRealPlayerPerTeam.Add(Color.red, 0);
        numberOfRealPlayerPerTeam.Add(Color.blue, 0);
        DontDestroyOnLoad(gameObject);
    }
}