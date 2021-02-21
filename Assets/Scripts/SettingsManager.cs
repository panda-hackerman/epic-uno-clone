using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SettingsManager : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Toggle fullscreenToggle;

    public Slider musicVolSlider;
    public Slider effectsVolSlider;

    Resolution[] resolutions;

    private void Awake()
    {
        //Get saved values
        if (PlayerPrefs.HasKey("MusicVolume"))
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));

        if (PlayerPrefs.HasKey("EffectsVolume"))
            SetEffectsVolume(PlayerPrefs.GetFloat("EffectsVolume"));

        //Update the settings visuals
        SetResDropdownOptions();

        float musicVolume = MusicManager.instance.GetAudioLevel("MusicVolume");
        float effectsVolume = MusicManager.instance.GetAudioLevel("EffectsVolume");

        musicVolSlider.SetValueWithoutNotify(musicVolume);
        effectsVolSlider.SetValueWithoutNotify(effectsVolume);

        qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
        fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
    }

    public void SetResDropdownOptions()
    {
        resolutionDropdown.ClearOptions();

        resolutions = Screen.resolutions;

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width}x{resolutions[i].height} at {resolutions[i].refreshRate}Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].width == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.SetValueWithoutNotify(currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetMusicVolume(float value)
    {
        MusicManager.instance.SetAudioLevel(value, "MusicVolume");
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetEffectsVolume(float value)
    {
        MusicManager.instance.SetAudioLevel(value, "EffectsVolume");
        PlayerPrefs.SetFloat("EffectsVolume", value);
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
