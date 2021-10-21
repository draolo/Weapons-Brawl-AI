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
        if(PauseMenuUI.activeSelf == false)
            CloseAllOtherMenu();

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
        Prototype.NetworkLobby.LobbyManager.s_Singleton.GoBackButton();
        //Prototype.NetworkLobby.LobbyManager.s_Singleton.gameObject.GetComponent<LobbyMyFeature>().LoadStartMenu();
    }
}



