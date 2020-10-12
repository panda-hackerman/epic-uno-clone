using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkLobbyManeger _lobbyManeger = null;

    [Header("UI")]
    [SerializeField] private GameObject _landingPagePanel = null;
    [SerializeField] private TMP_InputField _ipAdressInputField = null;
    [SerializeField] private Button _joinButton = null;

    private void OnEnable()
    {
        NetworkLobbyManeger.OnClientConnected += HandleClientConnected;
        NetworkLobbyManeger.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkLobbyManeger.OnClientConnected -= HandleClientConnected;
        NetworkLobbyManeger.OnClientDisconnected -= HandleClientDisconnected;
    }

    // Called from UI button
    public void JoinLobby()
    {
        // Stores network adress
        string ipAdress = _ipAdressInputField.text;
        // Sets network adress
        _lobbyManeger.networkAddress = ipAdress;
        _lobbyManeger.StartClient();
        // Disables join button so player doesn't try to join twice
        _joinButton.interactable = true;
    }

    private void HandleClientConnected()
    {
        // When connection succesful
        // Disables UI except for play button in case player joins and exits
        _joinButton.interactable = true;

        gameObject.SetActive(false);
        _landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        // When connection fails
        // Turns join button on again when player leaeves
        _joinButton.interactable = true;
    }
}
