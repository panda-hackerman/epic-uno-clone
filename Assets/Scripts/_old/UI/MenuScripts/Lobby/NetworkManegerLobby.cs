using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Linq;
using Mirror;

[System.Obsolete]
public class NetworkManegerLobby : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    // Drag in a scene and it's name will be turned into a string
    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Lobby")]
    [SerializeField] private NetworkPlayerLobby lobbyPlayerPrefab = null;
    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<NetworkPlayerLobby> PlayersInLobby { get; } = new List<NetworkPlayerLobby>();

    // >>> MAKE SURE THERES A FOLDER UNDER THIS NAME <<<
    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient()
    {
        // Gets every GameObject in the "SpawnablePrefabs" folder
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs){
            // Registers all spawnablePrefabs
            ClientScene.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        // Execute behaviour if connected
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        // Execute behaviour if disconnected
        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        // Disconnects players if the lobby is full [imagen Uno battle royale]
        if(numPlayers >= maxConnections){
            conn.Disconnect();
            return;
        }
        // Disconnects players if not in the right scene
        if(SceneManager.GetActiveScene().name != menuScene){
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // Checks if the player is in the right scene
        if(SceneManager.GetActiveScene().path == menuScene)
        {
            bool isHost = PlayersInLobby.Count == 0;

            // Instantiates a lobby player
            NetworkPlayerLobby lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab);

            lobbyPlayerInstance.IsTheHost = isHost;

            // Assigns connection to the lobby player
            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkPlayerLobby>();

            PlayersInLobby.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer() => PlayersInLobby.Clear();

    public void UpdatePlayersOfReadyState()
    {
        foreach(var player in PlayersInLobby)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach(var player in PlayersInLobby)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if(numPlayers < minPlayers) { return false; }

        foreach (var player in PlayersInLobby)
        {
            if(!player.IsReady) { return false;  }
        }

        return true;
    }
}
