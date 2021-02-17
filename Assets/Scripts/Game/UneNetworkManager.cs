using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Lobby;

public class UneNetworkManager : NetworkManager
{
    //Overridden to make sure the player leaves the match before the player object is destroyed
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Player player = conn.identity.GetComponent<Player>();

        MatchMaker.instance.LeaveMatch(player.gameObject);
        
        base.OnServerDisconnect(conn);
    }
}