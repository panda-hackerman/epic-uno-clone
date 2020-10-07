using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    //TODO: List of possible card prefabs
    public GameObject testCardPrefab;
    public Transform playerHand;

    //The player's hand of cards
    public List<Card> myHand = new List<Card>();
    public List<Transform> cardPositions = new List<Transform>();

    public float cardGap = 1.0f;

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
        for (int i = 0; i < 7; i++)
        {
            GameObject card = Instantiate(testCardPrefab, playerHand);
            //NetworkServer.Spawn(card, connectionToClient);
            myHand.Add(card.GetComponent<Card>());
            cardPositions.Add(card.transform);

        }

        UpdateCardPlacement();
    }

    private void UpdateCardPlacement()
    {
        for(int i = 0; i < myHand.Count; i++)
        {
            myHand[i].GetComponent<SpriteRenderer>().sortingOrder = i;
            myHand[i].transform.position = playerHand.position;
            myHand[i].transform.position += new Vector3((i - (myHand.Count - 1) / 2) * cardGap, 0, 0);
            myHand[i].transform.eulerAngles = new Vector3(45, 0, -6);

            myHand[i].transform.parent = playerHand;

            float totalArc = 20f;
            float rotationPerCard = totalArc / myHand.Count;
            float startRotation = -1f * (totalArc / 2f);

            float thisCardArc = startRotation + (i * rotationPerCard);
            myHand[i].transform.eulerAngles = new Vector3(45, 0f, -thisCardArc);

            float nudgeThisCard = Mathf.Abs(thisCardArc);
            nudgeThisCard *= 0.01f;
            myHand[i].transform.Translate(0f, -nudgeThisCard, 0f);
        }
    }
}
