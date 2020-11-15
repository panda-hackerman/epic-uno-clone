using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using AdvacedMathStuff;

public class PlayerManager : NetworkBehaviour
{
    public float cardGap = 1.0f;

    public List<GameObject> cardPrefabs = new List<GameObject>();

    //The player's hand of cards. The second list is a list of the local representations of the cards.
    public SyncListInt myHand = new SyncListInt();
    public List<Card> physicalCards = new List<Card>();

    //Calls when the local player starts
    public override void OnStartLocalPlayer()
    {
        Transform cam = Camera.main.gameObject.transform; //Get the main camera
        cam.SetParent(transform); //It's mine now hehe
        cam.position = transform.position; //Temporarily set the camera to my position to offset it later
        cam.eulerAngles = cam.eulerAngles.Y(transform.eulerAngles.y); //Set the camera's Y rotation to mine
        cam.localPosition += new Vector3(0f, 2.5f, -2.6f); //Add the offset
    }

    [ClientRpc]
    public void RpcAddCard(int id)
    {
        myHand.Add(id);
    }

    [ClientRpc]
    public void RpcSpawnCards()
    {
        if (!isLocalPlayer) return;
        //Destroy all the cards and reset the list so we can add them back with the correct cards 
        //This is so the physical cards list is always accurate to the cards in the player's deck
        /*foreach (Card card in physicalCards)
            Destroy(card);

        physicalCards = new List<Card>();*/

        //Spawn the cards
        Debug.Log(myHand.Count);
        for (int i = 0; i < myHand.Count; i++)
        {
            int ID = myHand[i];
            GameObject cardPhysical = Instantiate(cardPrefabs[ID]);
            cardPhysical.transform.parent = transform;
            physicalCards.Add(cardPhysical.GetComponent<Card>());

            Debug.Log("Spawned Card");
        }

        UpdateCardPlacement();
    }

    public void UpdateCardPlacement()
    {
        for (int i = 0; i < physicalCards.Count; i++)
        {
            Card card = physicalCards[i];

            //Make sure cards aren't rendering inside eachother
            card.GetComponent<SpriteRenderer>().sortingOrder = i;

            //Set position and intital rotation
            card.transform.position = transform.position;
            card.transform.localPosition += new Vector3((i - (physicalCards.Count - 1) / 2) * cardGap, 0, 0);
            card.transform.localEulerAngles = new Vector3(15, 0, 0);

            if (physicalCards.Count > 1)
            {
                //Rotate the cards in an arc
                float totalArc = 20.0f;
                float rotationPerCard = totalArc / (physicalCards.Count - 1);
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
