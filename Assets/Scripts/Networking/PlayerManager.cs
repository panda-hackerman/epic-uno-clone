using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    //TODO: List of possible card prefabs
    public GameObject testCardPrefab;

    //The player's hand of cards
    public List<Card> myHand = new List<Card>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        //Server and client will do this (server acts as client)

        if (hasAuthority)
        {
            CmdDealCards();
        }
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
        for (int i = 0; i <= 7; i++)
        {
            GameObject card = Instantiate(testCardPrefab);
            //NetworkServer.Spawn(card, connectionToClient);
            myHand.Add(card.GetComponent<Card>());
        }
    }
}
