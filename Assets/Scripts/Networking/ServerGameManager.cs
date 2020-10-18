using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvacedMathStuff;
using System;

[System.Serializable]
public class CardPrefab
{
    public int numberInDeck;

    public GameObject prefab;

    private Card _card;
    public Card card
    {
        get
        {
            if (!card) _card = prefab.GetComponent<Card>();
            return _card;
        }
        set { _card = value; }
    }
}

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

    #endregion
}
