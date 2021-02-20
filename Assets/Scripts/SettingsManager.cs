using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Dropdown resolutionDropdown;

    Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;

        if (resolutionDropdown)
        {
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = $"{resolutions[i].width}x{resolutions[i].height}";
                options.Add(option);
            }

            resolutionDropdown.AddOptions(options);
        }
    }

    public void SetMusicVolume(float value)
    {
        MusicManager.instance.SetAudioLevel(value, "MusicVolume");
    }

    public void SetEffectsVolume(float value)
    {
        MusicManager.instance.SetAudioLevel(value, "EffectsVolume");
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }
}
