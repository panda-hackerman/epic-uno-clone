using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGameManager : NetworkBehaviour
{
    public int maxDiscardCards = 10;

    [SyncVar]
    public List<GameObject> discardPile;

    public void UpdateDiscardPile()
    {
        if (discardPile.Count > maxDiscardCards)
        {
            GameObject oldestCard = discardPile[0];
            discardPile.Remove(oldestCard);
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
}
