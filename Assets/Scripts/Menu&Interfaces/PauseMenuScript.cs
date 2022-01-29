using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : AbstractMenu
{
    public GameObject PauseMenuUI;
    public AbstractInGameInterfaces[] otherMenu;

    private new void Start()
    {
        base.Start();
        otherMenu = GetComponents<AbstractInGameInterfaces>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
            OpenClosePauseMenu();
    }

    public void OpenClosePauseMenu()
    {
        if (PauseMenuUI.activeSelf == false)
        {
            CloseAllOtherMenu();
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        PauseMenuUI.SetActive(!PauseMenuUI.activeSelf);
    }

    private void CloseAllOtherMenu()
    {
        foreach (AbstractInGameInterfaces menu in otherMenu)
            menu.Close();
    }

    public void LoadStartMenu()
    {
        MatchManager._instance.Reset();
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}