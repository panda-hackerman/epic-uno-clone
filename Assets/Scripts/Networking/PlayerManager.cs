using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using AdvacedMathStuff;

public class PlayerManager : NetworkBehaviour
{
    //TODO: List of possible card prefabs
    public GameObject testCardPrefab;
    public Transform playerHand;

    //The player's hand of cards
    public List<Card> myHand = new List<Card>();

    [Header("Position Settings")]
    public float cardGap = 1.0f;

    private void Start()
    {
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        //Server and client will do this (server acts as client)

        if (isLocalPlayer)
        {
            Transform cam = Camera.main.gameObject.transform;
            cam.parent = transform;
            cam.eulerAngles = cam.eulerAngles.Y(transform.eulerAngles.y);
            cam.position = transform.position;
            cam.localPosition += new Vector3(0f, 2.5f, -2.6f);

            if (hasAuthority)
                CmdDealCards();
            else
                Debug.LogError("No Authority to call CmdDealCards");
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
            GameObject card = Instantiate(testCardPrefab, transform);
            NetworkServer.Spawn(card, connectionToClient);
            myHand.Add(card.GetComponent<Card>());
        }

        UpdateCardPlacement();
    }

    public void UpdateCardPlacement()
    {
        for (int i = 0; i < myHand.Count; i++)
        {
            Card card = myHand[i];

            //Make sure cards aren't rendering inside eachother
            card.GetComponent<SpriteRenderer>().sortingOrder = i;

            //Set position and intital rotation
            card.transform.parent = playerHand;
            card.transform.position = playerHand.position;
            card.transform.localPosition += new Vector3((i - (myHand.Count - 1) / 2) * cardGap, 0, 0);
            card.transform.localEulerAngles = new Vector3(15, 0, 0);

            if (myHand.Count > 1)
            {
                //Rotate the cards
                float totalArc = 20.0f;
                float rotationPerCard = totalArc / (myHand.Count - 1);
                float startRotation = -1f * (totalArc / 2f);

                float thisCardArc = startRotation + (i * rotationPerCard);
                card.transform.localEulerAngles += new Vector3(0f, 0f, -thisCardArc);

                //Nudge the cards down to match their rotation
                float nudgeThisCard = Mathf.Abs(thisCardArc);
                nudgeThisCard *= 0.01f;
                card.transform.Translate(0f, -nudgeThisCard, 0f);
            }

            //Set the resting and selected positon for later
            card.restingPos = card.transform.position;
            card.transform.localPosition += new Vector3(0, 0.3f, 0);
            card.selectedPos = card.transform.position;
            card.transform.position = card.restingPos;
        }
    }
}
