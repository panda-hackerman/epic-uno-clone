using Mirror;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class NetworkPlayerLobby : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNamesText = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playersReadyText = new TMP_Text[4];
    [SerializeField] private Button startGameButton = null;

    //[SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string NameToDisplay = "???...";
    //[SyncVar(hook = nameof(HandleReadyPlayerChanged))]
    public bool IsReady = false;

    private bool isHost;
    public bool IsTheHost
    {
        set
        {
            isHost = value;
            startGameButton.gameObject.SetActive(false);
        }
    }

    private NetworkManegerLobby lobby;

    private NetworkManegerLobby Lobby
    {
        get
        {
            if(lobby != null) { return lobby; }
            return lobby = NetworkManager.singleton as NetworkManegerLobby;
        }
    }

    public override void OnStartAuthority()
    {
        CMDSetDisplayName(PlayerNameSetter.PlayerDisplayName);
        lobbyUI.SetActive(true);
        
    }

    public override void OnStartClient()
    {
        Lobby.PlayersInLobby.Add(this);

        UpdateLobbyDisplay();
    }
    public override void OnStopServer()
    {
        Lobby.PlayersInLobby.Remove(this);

        UpdateLobbyDisplay();
    }

    public void HandleIsReadyStatusChanged(bool oldValue, bool newvalue) => UpdateLobbyDisplay();
    public void HandleIsReadyStatusChanged(string oldValue, string newvalue) => UpdateLobbyDisplay();

    private void UpdateLobbyDisplay()
    {
        if (!isLocalPlayer)
        {
            foreach(var player in Lobby.PlayersInLobby)
            {
                if (player.hasAuthority)
                {
                    player.UpdateLobbyDisplay();
                    break;
                }
            }

            return;
        }

        for(int i = 0; i < playerNamesText.Length; i++)
        {
            playerNamesText[i].text = "Waiting For Players...";
            playersReadyText[i].text = string.Empty;
        }

        for(int i = 0; i < Lobby.PlayersInLobby.Count; i++)
        {
            playerNamesText[i].text = Lobby.PlayersInLobby[i].NameToDisplay;
            playersReadyText[i].text = Lobby.PlayersInLobby[i].IsReady ? "<color=green>Ready(:</color>" : "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if(!isHost) { return; }

        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CMDSetDisplayName(string displayName) => NameToDisplay = displayName;

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Lobby.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if(Lobby.PlayersInLobby[0].connectionToClient != connectionToClient) { return;  }
    }
}
