using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : AbstractMenu {

    public void LoadLobby()
    {
        SceneManager.LoadScene(1);
    }

}
