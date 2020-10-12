using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Mirror;

public class NetworkLobbyManeger : NetworkManager
{
    // Drag in a scene and it's name will be turned into a string
    [Scene] [SerializeField] private string _menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkLobbyPlayer _lobbyPlayerPrefab = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

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
        if(SceneManager.GetActiveScene().name != _menuScene){
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // Checks if the player is in the right scene
        if(SceneManager.GetActiveScene().name == _menuScene)
        {
            // Instantiates a lobby player
            NetworkLobbyPlayer lobbyPlayerInstance = Instantiate(_lobbyPlayerPrefab);
            // Assigns connection to the lobby player
            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance.gameObject);
        }
    }
}
