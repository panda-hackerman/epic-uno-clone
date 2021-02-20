using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioMixer mixer;
    public AudioSource audioSource;

    public AudioClip mainMenuMusic;
    public AudioClip mainLobbyMusic;
    public AudioClip[] mainGameMusic;

    private void Awake()
    {
        audioSource.Play();

        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SetAudioLevel(float value, string name)
    {
        float decibelValue = Mathf.Log10(value) * 20;

        mixer.SetFloat(name, decibelValue);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                audioSource.clip = mainMenuMusic;
                audioSource.Play();
                break;
            case "MainLobby":
                audioSource.clip = mainLobbyMusic;
                audioSource.Play();
                break;
            case "MainGame":
                audioSource.clip = mainGameMusic[0];
                audioSource.Play();
                break;
        }
    }
}
