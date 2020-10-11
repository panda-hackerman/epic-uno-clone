using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[AddComponentMenu("")]
public class NetworkManagerUno : NetworkManager
{
    public Transform p1spawn, p2spawn, p3spawn, p4spawn;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform start = p1spawn;
/*        switch (NetworkServer.connections.Count)
        {
            case 1:
                start = p1spawn;
                break;
            case 2:
                start = p2spawn;
                break;
            case 3:
                start = p3spawn;
                break;
            case 4:
                start = p4spawn;
                break;
            default:
                Debug.LogError("There are already 4 players connected");
                conn.Disconnect();
                return;
        }
*/
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

        Debug.Log("Added player. Total number of players: " + NetworkServer.connections.Count);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
    }
}
