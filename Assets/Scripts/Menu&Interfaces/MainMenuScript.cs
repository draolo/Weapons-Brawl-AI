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
        matchInfo.realBluePlayer = 1;
        matchInfo.realRedPlayer = 1;
        matchInfo.playerPerTeam = 3;
        SceneManager.LoadScene(1);
    }

    public void Play1p()
    {
        matchInfo.realBluePlayer = 1;
        matchInfo.realRedPlayer = 0;
        matchInfo.playerPerTeam = 3;
        SceneManager.LoadScene(1);
    }

    public void Play0p()
    {
        matchInfo.realBluePlayer = 0;
        matchInfo.realRedPlayer = 0;
        matchInfo.playerPerTeam = 3;
        SceneManager.LoadScene(1);
    }
}