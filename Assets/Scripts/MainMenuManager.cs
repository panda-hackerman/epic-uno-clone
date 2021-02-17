using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class MainMenuManager : MonoBehaviour
{
    public NetworkManager networkManager;

    public void StartGame()
    {
        StartCoroutine(LoadScene("Offline"));
    }

    public void OpenCredits()
    {
        StartCoroutine(LoadScene("Credits"));
    }

    public void OpenMainMenu()
    {
        StartCoroutine(LoadScene("MainMenu"));

        if (networkManager)
            Destroy(networkManager.gameObject);
    }

    public IEnumerator LoadScene(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void QuitToDesktop()
    {
        Debug.Log("Closing game...");
        Application.Quit();
    }
}
