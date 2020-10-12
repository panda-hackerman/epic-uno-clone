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

    [HideInInspector] public List<GameObject> cardPrefabs; //Always at the bottom

    public override void OnStartLocalPlayer()
    {
        Transform cam = Camera.main.gameObject.transform;
        cam.SetParent(transform);
        cam.position = transform.position;
        cam.eulerAngles = cam.eulerAngles.Y(transform.eulerAngles.y);
        cam.localPosition += new Vector3(0f, 2.5f, -2.6f);

        DealCards();
    }

    private void DealCards()
    {
        for (int i = 0; i < 7; i++)
        {
            //TODO: Instead of using Random.Range, have certain cards more likely to be picked than others

            //Make the card
            GameObject prefab = cardPrefabs[Random.Range(0, cardPrefabs.Count - 1)];
            GameObject cardObj = Instantiate(prefab, transform);
            Card card = cardObj.GetComponent<Card>();

            card.ID = cardPrefabs.IndexOf(prefab); //Card ID needed for when it's added to the deck later
            myHand.Add(card); //Add to list of cards currently in my hand

            UpdateCardPlacement(); //Position the cards correctly on my screen
        }
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
