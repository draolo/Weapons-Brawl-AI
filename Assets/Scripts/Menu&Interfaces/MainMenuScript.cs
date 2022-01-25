using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : AbstractMenu
{
    public void Play2p()
    {
        SceneManager.LoadScene(1);
    }

    public void Play1p()
    {
        SceneManager.LoadScene(1);
    }

    public void Play0p()
    {
        SceneManager.LoadScene(1);
    }
}