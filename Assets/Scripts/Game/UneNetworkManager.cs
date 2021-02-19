using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Lobby;

public class UneNetworkManager : NetworkManager
{
    public static UneNetworkManager instance;

    private void OnEnable()
    {
        instance = this;
    }

    //Overridden to make sure the player leaves the match before the player object is destroyed
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Player player = conn.identity.GetComponent<Player>();

        MatchMaker.FindMatchByID(player.matchID, out Match match);
        TurnManager turnManager = match.turnManagerObj.GetComponent<TurnManager>();

        MatchMaker.instance.LeaveMatch(player.gameObject, turnManager);

        NetworkServer.DestroyPlayerForConnection(conn);
    }
}