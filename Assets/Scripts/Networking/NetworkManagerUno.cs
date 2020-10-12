using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[AddComponentMenu("")]
public class NetworkManagerUno : NetworkManager
{
    public Transform playerSpawn;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform start = playerSpawn;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

        Debug.Log("Added player. Total number of players: " + NetworkServer.connections.Count);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
    }
}
