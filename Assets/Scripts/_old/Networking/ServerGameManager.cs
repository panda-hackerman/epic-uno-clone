using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvacedMathStuff;

public class ServerGameManager : NetworkBehaviour
{
    public List<PlayerManager> players = new List<PlayerManager>();
    public Dictionary<NetworkConnection, PlayerManager> playerConnections = new Dictionary<NetworkConnection, PlayerManager>();

    public int playerCount { get { return players.Count; } }

    #region TURNS

    [SyncVar]
    public int currentPlayer;
    public bool isReversed;

    #endregion

    #region DISCARD_PILE

    public int maxDiscardCards = 10; //Most cards in the discard pile at one time

    [SyncVar]
    public List<GameObject> discardPile;

    public void UpdateDiscardPile()
    {
        //Remove the oldest card if the total is more than the max
        if (discardPile.Count > maxDiscardCards)
        {
            GameObject oldestCard = discardPile[0];
            discardPile.Remove(oldestCard);
            drawPile[oldestCard.GetComponent<Card>().ID].numberInDeck++;
            Destroy(oldestCard);
        }

        for (int i = 0; i < discardPile.Count; i++)
        {
            //Make sure everything is rendering in the correct order
            RpcSetSortingOrder(discardPile[i].GetComponent<NetworkIdentity>(), i);
        }
    }

    [ClientRpc]
    public void RpcSetSortingOrder(NetworkIdentity identity, int index)
    {
        SpriteRenderer sr = identity.GetComponent<SpriteRenderer>();

        sr.sortingLayerName = "DiscardPile";
        sr.sortingOrder = index;
    }
    #endregion

    #region DRAW_PILE
    
    [HideInInspector]
    [SyncVar]
    public List<CardInfo> drawPile;

    public void DealCards(PlayerManager playerManager)
    {
        Debug.Log("This should appear once");
        for (int i = 0; i < 7; i++) //Start with 8 cards
        {
            DealCard(playerManager);
        }

        playerManager.RpcSpawnCards();
    }

    public void DealCard(PlayerManager playerManager)
    {
        Debug.Log("Starting to deal card...");
/*        List<double> cardWeights = new List<double>(); //List of doubles, which represents the chance of each card being picked
        foreach(CardInfo cardPrefab in drawPile) 
        {
            //For every card currently in the deck, add the number of them that exist in the deck
            cardWeights.Add(cardPrefab.numberInDeck);
            Debug.Log("Calculating weights...");
        }

        //Pick an index from the list based on the probability we detailed above
        int pickedCardIndex = AdvMath.Roulette(cardWeights);
        Debug.Log("Picked index #" + pickedCardIndex);

        //Add the card to the player's hand and set some variables
        CardInfo pickedCard = drawPile[pickedCardIndex];
        pickedCard.prefab.GetComponent<Card>().ID = pickedCardIndex; //The ID is the card's position in the list
        playerManager.RpcAddCard(pickedCard.prefab.GetComponent<Card>().ID);

        //Since we've taken a card from the deck, the number of them decreses (and is less likely to be drawn next time)
        drawPile[pickedCardIndex].numberInDeck--;
        Debug.Log("Done!");*/
    }

    #endregion
}
