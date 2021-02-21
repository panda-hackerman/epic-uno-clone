using System.Collections;
using System.Collections.Generic;
using AdvacedMathStuff;
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
    private AudioClip[] shuffledGameMusic;

    string activeScene;
    int gameMusicIndex;

    private void Awake()
    {
        mixer.SetFloat("MasterVolume", -27); //TODO: Add master volume slider somewhere

        audioSource.Play();

        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            UpdateMusic();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void SetAudioLevel(float value, string name) //name = "MusicVolume" or "EffectsVolume" or "MasterVolume".
    {
        float decibelValue = Mathf.Log10(value) * 20;

        mixer.SetFloat(name, decibelValue);
    }

    public float GetAudioLevel(string name)
    {
        mixer.GetFloat(name, out float decibelValue);

        float linearValue = Mathf.Pow(10, decibelValue / 20); //Inverse of log(x) * 20
        return linearValue;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        activeScene = scene.name;

        switch (activeScene)
        {
            case "MainMenu":
                SetActiveMusic(mainMenuMusic);
                break;
            case "MainLobby":
                SetActiveMusic(mainLobbyMusic);
                break;
            case "MainGame":
                shuffledGameMusic = mainGameMusic.ShuffleArray();
                gameMusicIndex = 0;
                SetActiveMusic(shuffledGameMusic[0]);
                break;
            default:
                break;
        }
    }

    public void OnSceneUnloaded(Scene scene)
    {
        if (activeScene == scene.name)
        {
            activeScene = SceneManager.GetActiveScene().name;
            UpdateMusic();
        }
    }

    public void UpdateMusic()
    {
        switch (activeScene)
        {
            case "MainMenu":
                SetActiveMusic(mainMenuMusic);
                break;
            case "MainLobby":
                SetActiveMusic(mainLobbyMusic);
                break;
            case "MainGame":
                gameMusicIndex++;
                if (gameMusicIndex > mainGameMusic.Length) gameMusicIndex = 0;
                SetActiveMusic(shuffledGameMusic[gameMusicIndex]);
                break;
            default:
                break;
        }
    }

    public void SetActiveMusic(AudioClip clip)
    {
        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
