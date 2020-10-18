using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using AdvacedMathStuff;

public class PlayerManager : NetworkBehaviour
{
    public float cardGap = 1.0f;

    //The player's hand of cards
    public List<Card> myHand = new List<Card>();

    public ServerGameManager sgm;

    public override void OnStartLocalPlayer()
    {
        Transform cam = Camera.main.gameObject.transform;
        cam.SetParent(transform);
        cam.position = transform.position;
        cam.eulerAngles = cam.eulerAngles.Y(transform.eulerAngles.y);
        cam.localPosition += new Vector3(0f, 2.5f, -2.6f);

        sgm = FindObjectOfType<ServerGameManager>();
        if (!sgm) Debug.LogWarning("Couldn't find the server game manager");

        if (isLocalPlayer && hasAuthority)
            DealCards();
    }

    public void DealCards()
    {
        for (int i = 0; i < 7; i++)
        {
            //Make the card
            List<double> cardWeights = new List<double>();
            foreach(CardPrefab cardPrefab in sgm.drawPile)
            {
                cardWeights.Add(cardPrefab.numberInDeck);
            }

            int number = AdvMath.Roulette(cardWeights); //Cards with a higher count are more likely to be picked
            GameObject prefab = sgm.drawPile[number].prefab;
            Debug.Log("The prefab at number " + number + " is called: " + prefab.name);
            GameObject cardObj = Instantiate(prefab, transform);
            Card card = cardObj.GetComponent<Card>();

            CmdRemoveCardFromDrawPile(number, sgm.GetComponent<NetworkIdentity>()); //Take one of this card out of the draw pile
            card.ID = number;
            myHand.Add(card); //Add to list of cards currently in my hand

            UpdateCardPlacement(); //Position the cards correctly on my screen
        }
    }

    [Command]
    void CmdRemoveCardFromDrawPile(int number, NetworkIdentity servGM)
    {
        servGM.GetComponent<ServerGameManager>().drawPile[number].numberInDeck--;
    }

    public void UpdateCardPlacement()
    {
        for (int i = 0; i < myHand.Count; i++)
        {
            Card card = myHand[i];
            card.name = card.type.ToString() + " uno card";

            //Make sure cards aren't rendering inside eachother
            card.GetComponent<SpriteRenderer>().sortingOrder = i;

            //Set position and intital rotation
            card.transform.position = transform.position;
            card.transform.localPosition += new Vector3((i - (myHand.Count - 1) / 2) * cardGap, 0, 0);
            card.transform.localEulerAngles = new Vector3(15, 0, 0);

            if (myHand.Count > 1)
            {
                //Rotate the cards in an arc
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
