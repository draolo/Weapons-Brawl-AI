using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AbstractMenu : MonoBehaviour {


    public AudioMixer audioMixer;
    public Dropdown resolutionDropDown;
    public Toggle fullScreenToggle;


    private List<string> ResolutionOptions;


    public void Start()
    {
        Resolution[] resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();
        ResolutionOptions = new List<string>();

        ResolutionOptions.Add("Select resolution");

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;

            if (i == 0 || !option.Equals(ResolutionOptions[ResolutionOptions.Count - 1]))
                ResolutionOptions.Add(option);

        }
        resolutionDropDown.AddOptions(ResolutionOptions);
        resolutionDropDown.RefreshShownValue();

        fullScreenToggle.isOn = Screen.fullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex != 0)
        {
            string[] resolution = ResolutionOptions[resolutionIndex].Split('x');
            Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), Screen.fullScreen);
        }


    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetFullScreen(bool fullScreen)
    {
        Screen.fullScreen = fullScreen;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
