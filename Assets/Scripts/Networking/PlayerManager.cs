using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    //TODO: Replace with a list of Card objects
    public List<GameObject> cards = new List<GameObject>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        //Server and client will do this (server acts as client)
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        //Only the server does this
    }

    [Command]
    public void CmdDealCards()
    {

    }
}
