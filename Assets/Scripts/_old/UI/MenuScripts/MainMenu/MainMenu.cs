using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManegerLobby _lobbyManeger = null;

    [Header("UI")]
    [SerializeField] private GameObject _landingPagePanel = null;

    public void HostLobby()
    {
        // Start hosting
        _lobbyManeger.StartHost();
        // Disables panel
        _landingPagePanel.SetActive(false);
    }

    // Call from Ui button
    public void CloseGame()
    {
        // Closes Game
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(0);
    }
}
