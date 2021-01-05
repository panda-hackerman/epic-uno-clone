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

    bool selectedDrawPile;

    public LayerMask cardLayer;
    public LayerMask drawPileLayer;

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

        if (Physics.Raycast(cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 50, cardLayer))
        {
            selectedCard = hit.collider.GetComponent<Card>();
        }

        foreach (Card card in playerManager.physicalCards)
        {
            //if (card) card.IsSelected = card == selectedCard;
        }

        //True if hovering over the discard pile
        selectedDrawPile = Physics.Raycast(cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit2, 50, drawPileLayer);
    }

    [Command]
    public void CmdAddCardToDiscard(int id)
    {
/*        GameObject newCard = Instantiate(gameManager.drawPile[id].prefab);
        Destroy(newCard.GetComponent<Collider>());
        newCard.GetComponent<Card>().ID = id;

        NetworkServer.Spawn(newCard);

        newCard.transform.position = new Vector3(0f.GiveOrTake(0.1f), 0.01f, 0f.GiveOrTake(0.1f));
        newCard.transform.eulerAngles = new Vector3(90, 0, Random.Range(0f, 360f));
        gameManager.discardPile.Add(newCard);
        gameManager.UpdateDiscardPile();*/
    }

    public void OnSelect()
    {
        if (!hasAuthority) return;

        //Check if it is my turn
        if (playerManager.playerNumber != gameManager.currentPlayer)
        {
            Debug.Log("Player Number = " + playerManager.playerNumber + ", Current Player = " + gameManager.currentPlayer);
            return;
        }

        if (selectedCard)
            DoPlayCard();
        else if (selectedDrawPile)
            CmdDoDrawCard(playerManager.gameObject);
    }

    private void DoPlayCard()
    {
        if (!playerManager.physicalCards.Contains(selectedCard))
        {
            Debug.LogWarning("Player attempting to select card not in their hand");
            return;
        }

        if (!selectedCard.wildCard) //We can skip all this if you are playing a wild, as they can be played on anything
        {
            int type = (int)selectedCard.type;
            int cardNum = -1;

            NumberedCard numCard = selectedCard as NumberedCard;
            if (numCard) cardNum = numCard.cardValue;

            CmdCanPlayCard(gameObject, type, cardNum, selectedCard.GetType().ToString());

            if (!canPlay) return;
        }

        selectedCard.OnCardPlayed(); //You've been played! B)
        if (selectedCard.callNextTurn) CmdNextTurn();

        //Get out of my hand and into the discard pile
        playerManager.physicalCards.Remove(selectedCard);
        CmdAddCardToDiscard(selectedCard.ID);

        Destroy(selectedCard.gameObject); //Death

        playerManager.UpdateCardPlacement(); //Make sure the other cards are where they need to be
    }

    [Command]
    private void CmdDoDrawCard(GameObject _playerManager)
    {
        PlayerManager pManager = _playerManager.GetComponent<PlayerManager>();
        gameManager.DealCard(pManager);
        pManager.RpcSpawnCards();
        Debug.Log("bababooey");
    }

    //TODO: Have a card show up at the beginning of the game

    public bool canPlay;
    [Command]
    public void CmdCanPlayCard(GameObject player, int type, int cardNum, string classType)
    {
        UnoInputManager self = player.GetComponent<UnoInputManager>();

        self.canPlay = false;

        if (!gameManager.discardPile.Any()) //If there are no card in discard, should be fixed
        {
            self.canPlay = true;
            return;
        }

        Card topCard = gameManager.discardPile[gameManager.discardPile.Count - 1].GetComponent<Card>();
        NumberedCard topNumCard = topCard as NumberedCard;

        if (topNumCard && cardNum != -1) //If this is a numbered card
        {
            if (cardNum == topNumCard.cardValue)
                self.canPlay = true;
        }
        else
        {
            if (topCard.GetType().ToString() == classType)
                self.canPlay = true;
        }

        if (type != 0 && (CardType)type == topCard.type) //If the cards are of the same type then they can always play
        {
            self.canPlay = true;
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
