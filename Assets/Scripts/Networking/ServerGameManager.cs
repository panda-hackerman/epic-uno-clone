using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvacedMathStuff;
using System;

public class ServerGameManager : NetworkBehaviour
{
    #region DISCARD_PILE
    public int maxDiscardCards = 10;

    [SyncVar]
    public List<GameObject> discardPile;

    public void UpdateDiscardPile()
    {
        if (discardPile.Count > maxDiscardCards)
        {
            GameObject oldestCard = discardPile[0];
            discardPile.Remove(oldestCard);
            drawPile[oldestCard.GetComponent<Card>().ID].numberInDeck++;
            Destroy(oldestCard);
        }

        for (int i = 0; i < discardPile.Count; i++)
        {
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

    [SyncVar]
    public List<CardPrefab> drawPile;

    public void DealCards(PlayerManager playerManager)
    {
        for (int i = 0; i < 7; i++) //Start with 8 cards
        {
            DealCard(playerManager);
        }

        Debug.Log("Finished Dealing Cards");
        playerManager.RpcSpawnCards();
    }

    public void DealCard(PlayerManager playerManager)
    {
        List<double> cardWeights = new List<double>(); //List of doubles, which represents the chance of each card being picked
        foreach(CardPrefab cardPrefab in drawPile) 
        {
            //For every card currently in the deck, add the number of them that exist in the deck
            //The more of the same card in the deck the more likely it is to get picked.
            cardWeights.Add(cardPrefab.numberInDeck);
        }

        //Pick an index from the list based on the probability we detailed above
        //The lists of double and the cardPrefab list are the same length, so each index in each correspond to an item in the other
        int pickedCardIndex = AdvMath.Roulette(cardWeights);

        //Add the card to the player's hand and set some variables
        CardPrefab pickedCard = drawPile[pickedCardIndex];
        pickedCard.card.ID = pickedCardIndex; //The ID is the card's position in the list
        playerManager.RpcAddCard(pickedCard.card.ID);

        //Since we've taken a card from the deck, the number of them decreses (and is less likely to be drawn next time)
        drawPile[pickedCardIndex].numberInDeck--;
    }

    #endregion
}
