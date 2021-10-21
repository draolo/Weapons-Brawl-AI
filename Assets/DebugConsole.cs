using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour {

    public GameObject Console;
    public Text msg;

    #region Singleton
    public static DebugConsole Instance;

    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;
        Console.SetActive(false);
    }
    #endregion

    public void print(string message)
    {
        Console.SetActive(true);
        msg.text = message;

    }
}
