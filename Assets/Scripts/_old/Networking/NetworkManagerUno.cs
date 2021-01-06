using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[System.Obsolete]
[AddComponentMenu("")]
public class NetworkManagerUno : NetworkManager
{
    public Transform playerSpawn;
    public ServerGameManager gameManager;

    //When player joins
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform start = playerSpawn;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation); //Spawn a player and set their pos
        NetworkServer.AddPlayerForConnection(conn, player); //Connects the player to their network connection.

        PlayerManager playerManager = player.GetComponent<PlayerManager>();

        gameManager.players.Add(playerManager);
        gameManager.playerConnections.Add(conn, playerManager);

        //Now the server manager will deal the cards for the specified player.
        gameManager.DealCards(playerManager);

        for (int i = 0; i < gameManager.playerCount; i++)
        {
            gameManager.players[i].playerNumber = i;
        }

        Debug.Log("Added player");
    }

    //This happens when a client disconnects from the server
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        PlayerManager playerManager = gameManager.playerConnections[conn];

        gameManager.players.Remove(playerManager);
        gameManager.playerConnections.Remove(conn);

        for (int i = 0; i < gameManager.playerCount; i++)
        {
            gameManager.players[i].playerNumber = i;
        }

        NetworkServer.DestroyPlayerForConnection(conn);

        Debug.Log("Disconnected Player");
    }
}
