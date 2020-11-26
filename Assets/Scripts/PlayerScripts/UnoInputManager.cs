using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using AdvacedMathStuff;
using System.Linq;

public class UnoInputManager : NetworkBehaviour
{
    ServerGameManager gameManager;
    PlayerManager playerManager;
    Camera cam;
    Card selectedCard;

    public LayerMask layerMask;

    private void Start()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();

        cam = Camera.main;

        gameManager = FindObjectOfType<ServerGameManager>();
        if (!gameManager) Debug.LogWarning("Couldn't find the server game manager");
    }

    private void Update()
    {
        selectedCard = null;

        if (Physics.Raycast(cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 50, layerMask))
        {
            selectedCard = hit.collider.GetComponent<Card>();
        }

        foreach (Card card in playerManager.physicalCards)
        {
            if (card)
                card.IsSelected = card == selectedCard;
        }
    }

    [Command]
    public void CmdAddCardToDiscard(int id)
    {
        GameObject newCard = Instantiate(gameManager.drawPile[id].prefab);
        Destroy(newCard.GetComponent<Collider>());
        newCard.GetComponent<Card>().ID = id;

        NetworkServer.Spawn(newCard);

        newCard.transform.position = new Vector3(0f.GiveOrTake(0.1f), 0.01f, 0f.GiveOrTake(0.1f));
        newCard.transform.eulerAngles = new Vector3(90, 0, Random.Range(0f, 360f));
        gameManager.discardPile.Add(newCard);
        gameManager.UpdateDiscardPile();
    }

    public void OnSelect()
    {
        if (playerManager.playerNumber != gameManager.currentPlayer)
        {
            Debug.Log("Player Number = " + playerManager.playerNumber + ", Current Player = " + gameManager.currentPlayer);
            return;
        }

        if (!selectedCard || !hasAuthority) return;
        if (!playerManager.physicalCards.Contains(selectedCard))
        {
            Debug.LogWarning("Player attempting to select card not in their hand");
            return;
        }

        if (!selectedCard.wildCard)
        {
            int type = (int)selectedCard.type;
            int cardNum = -1;

            NumberedCard numCard = selectedCard as NumberedCard;
            if (numCard) cardNum = numCard.cardValue;

            CmdCanPlayCard(type, cardNum, selectedCard.GetType().ToString());

            if (!canPlay) return;
        }

        selectedCard.PlayCard(); //You've been played! B)
        CmdNextTurn();

        //Get out of my hand and into the discard pile
        playerManager.physicalCards.Remove(selectedCard);
        CmdAddCardToDiscard(selectedCard.ID);

        Destroy(selectedCard.gameObject); //Death
        selectedCard = null; //Object deleted so set var to null

        playerManager.UpdateCardPlacement(); //Make sure the other cards are where they need to be
    }

    bool canPlay;

    //TODO: Have a card show up at the beginning of the game
    [Command]
    public void CmdCanPlayCard(int type, int cardNum, string classType)
    {
        canPlay = false;

        if (!gameManager.discardPile.Any())
        {
            canPlay = true;
            return;
        }

        Card topCard = gameManager.discardPile[gameManager.discardPile.Count - 1].GetComponent<Card>();

        if (!topCard)
        {
            canPlay = true;
            return;
        }

        if (type != 0 && (CardType)type == topCard.type) //If the card type is not other, and they are of the same type
        {
            canPlay = true;
        }
        else
        {
            NumberedCard topNumCard = topCard as NumberedCard;

            if (topNumCard && cardNum != -1) //If this is a numbered card
            {
                if (cardNum == topNumCard.cardValue)
                    canPlay = true;
            }
            else
            {
                if (topCard.GetType().ToString() == classType)
                    canPlay = true;
            }
        }
    }

    [Command]
    public void CmdNextTurn()
    {
        if (!gameManager) gameManager = FindObjectOfType<ServerGameManager>();
        if (!gameManager) Debug.LogError("What the feck happened");

        gameManager.currentPlayer += gameManager.isReversed ? -1 : 1; //Add or subtract depending on if the game is reversed

        if (gameManager.currentPlayer > gameManager.playerCount - 1)
            gameManager.currentPlayer -= gameManager.playerCount;

        if (gameManager.currentPlayer < 0)
            gameManager.currentPlayer += gameManager.playerCount;
    }
}
