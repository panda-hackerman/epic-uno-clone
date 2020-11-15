using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[AddComponentMenu("")]
public class NetworkManagerUno : NetworkManager
{
    public Transform playerSpawn;
    public ServerGameManager servGM;

    //When player joins
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform start = playerSpawn;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation); //Spawn a player and set their pos
        NetworkServer.AddPlayerForConnection(conn, player); //Connects the player to their network connection.

        PlayerManager playerManager = player.GetComponent<PlayerManager>();

        Debug.Assert(playerManager != null, "Uh oh, someone did an oopsie. There is no player manager on the player object!"); //Just in case

        //Now the server manager will deal the cards for the specified player.
        servGM.DealCards(playerManager);
    }

    //This happens when a client disconnects from the server
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        //TODO: Add disconnected player's cards back into the draw pile
        base.OnServerDisconnect(conn);
    }
}
