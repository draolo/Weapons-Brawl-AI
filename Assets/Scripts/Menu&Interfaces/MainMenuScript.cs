using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : AbstractMenu
{
    private MatchInfo matchInfo;

    private void Awake()
    {
        matchInfo = FindObjectOfType<MatchInfo>();
    }

    public void Play2p()
    {
        matchInfo.numberOfRealPlayerPerTeam[Color.red] = 1;
        matchInfo.numberOfRealPlayerPerTeam[Color.blue] = 1;
        matchInfo.playerPerTeam = 3;
        SceneManager.LoadScene(1);
    }

    public void Play1p()
    {
        matchInfo.numberOfRealPlayerPerTeam[Color.red] = 0;
        matchInfo.numberOfRealPlayerPerTeam[Color.blue] = 1;
        matchInfo.playerPerTeam = 3;
        SceneManager.LoadScene(1);
    }

    public void Play0p()
    {
        matchInfo.numberOfRealPlayerPerTeam[Color.red] = 0;
        matchInfo.numberOfRealPlayerPerTeam[Color.blue] = 0;
        matchInfo.playerPerTeam = 3;
        SceneManager.LoadScene(1);
    }
}