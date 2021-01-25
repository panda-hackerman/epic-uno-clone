using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using AdvacedMathStuff;

[System.Obsolete]
public class PlayerManager : NetworkBehaviour
{
    ServerGameManager gameManager;
    public GameObject chooseColorButtons;

    [SyncVar]
    public int playerNumber;

    public float cardGap = 1.0f;

    [Header("Cards")]
    public List<GameObject> cardPrefabs = new List<GameObject>();

    //The player's hand of cards. The second list is a list of the local representations of the cards.
    public List<int> myHand = new List<int>();
    public List<Card> physicalCards = new List<Card>();

    //Calls when the local player starts
    public override void OnStartLocalPlayer()
    {
        Transform cam = Camera.main.gameObject.transform; //Get the main camera
        cam.SetParent(transform); //It's mine now hehe
        cam.position = Vector3.zero; //Reset pos to change it later
        cam.eulerAngles = cam.eulerAngles.Y(transform.eulerAngles.y); //Set the camera's Y rotation to mine
        cam.localPosition += new Vector3(0f, 4.5f, -4.5f); //Add the offset

        //chooseColorButtons = cam.GetComponent<CanvasInfo>().colorButtons;
        //chooseColorButtons.SetActive(false);

        gameObject.name += " (Local Player)";
    }

    private void Start()
    {
        gameManager = FindObjectOfType<ServerGameManager>();
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
        foreach (Card card in physicalCards)
        {
            Destroy(card.gameObject);
        }

        physicalCards = new List<Card>();

        //Spawn the cards
        for (int i = 0; i < myHand.Count; i++)
        {
            int ID = myHand[i];

            GameObject cardPhysical = Instantiate(cardPrefabs[ID]);
            Card card = cardPhysical.GetComponent<Card>();

            physicalCards.Add(card);

            cardPhysical.transform.parent = transform;
            //card.myPlayer = this; //Who's your daddy
        }

        UpdateCardPlacement();
    }

    public void UpdateCardPlacement()
    {
/*        for (int i = 0; i < physicalCards.Count; i++)
        {
            Card card = physicalCards[i];

            //Make sure cards aren't rendering inside eachother
            card.GetComponent<SpriteRenderer>().sortingOrder = i;

            //Set position and intital rotation
            card.transform.position = transform.position;
            card.transform.localPosition += new Vector3((i - (physicalCards.Count - 1) / 2) * cardGap, 0, 0);
            card.transform.localEulerAngles = new Vector3(30, 0, 0);

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
        }*/
    }

    [Command]
    public void CmdPickColor(int color)
    {
        Card topCard = gameManager.discardPile[gameManager.discardPile.Count - 1].GetComponent<Card>();

        topCard.type = (CardType) color;

        if (topCard is WildCard)
        {
            WildCard wildCard = topCard as WildCard;
            SpriteRenderer spriteRenderer = wildCard.GetComponent<SpriteRenderer>();

            switch (color)
            {
                case 1:
                    if (wildCard.yellowSprite) spriteRenderer.sprite = wildCard.yellowSprite;
                    break;
                case 2:
                    if (wildCard.redSprite) spriteRenderer.sprite = wildCard.redSprite;
                    break;
                case 3:
                    if (wildCard.blueSprite) spriteRenderer.sprite = wildCard.blueSprite;
                    break;
                case 4:
                    if (wildCard.greenSprite) spriteRenderer.sprite = wildCard.greenSprite;
                    break;
            }
        }

        Debug.Log("Changed the top card's color to " + topCard.type.ToString());
    }
}
