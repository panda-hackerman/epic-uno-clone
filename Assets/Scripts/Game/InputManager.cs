using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Lobby;
using Mirror;
using Convert;
using AdvacedMathStuff;

public class InputManager : NetworkBehaviour
{
    public bool getSelection = false;

    private GameObject selection;
    private Card selectedCard
    {
        get
        {
            if (selection && selection.TryGetComponent(out Card card))
                return card;
            else return null;
        }
    }

    private bool selectedDraw { get { return selection && selection.CompareTag("DrawPile"); } }

    private Player player;
    public Camera cam;

    private void Start()
    {
        player = GetComponent<Player>();
        getSelection = false;
    }

    private void Update()
    {
        if (!cam || !TurnManager.instance) return;

        if (getSelection && !UIGame.instance.pauseMenuOpen) //Get selection will only be true when it is local player's turn
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
                selection = hit.transform.gameObject;
            else
                selection = null;
        }
        else
        {
            selection = null;
        }

        foreach (Card card in player.cardObjs)
        {
            card.isSelected = card == selectedCard;
        }
    }

    public void OnInit() //Runs when the match starts
    {
        Debug.Log("Hello");
        cam = GameObject.FindGameObjectWithTag("GameCamera").GetComponent<Camera>();

        getSelection = TurnManager.instance.currentPlayer == player.playerID;
    }

    public void OnSelect() //Click!
    {
        if (TurnManager.instance && TurnManager.instance.currentPlayer == player.playerID) //Checking just in case
        {
            if (selectedCard)
            {
                if (CanPlayCard(selectedCard))
                    PlayCard(selectedCard);
            }
            else if (selectedDraw)
            {
                CmdDrawCard();
            }
        }
    }

    public void OnPause() //Esc!
    {
        if (UIGame.instance)
            UIGame.instance.TogglePause();
    }

    public bool CanPlayCard(Card card)
    {
        if (DiscardCount == 0) //modCheck cards?
            return true;

        if (card.wildCard) //Wilds can play on anything
            return true;

        Card topCard = DiscardTop; //Caching since TopCard uses get component

        if (card is NumberedCard) //0-9?
        {
            if (topCard is NumberedCard)
            {
                NumberedCard _card = card as NumberedCard;
                NumberedCard _topCard = topCard as NumberedCard;

                if (_card.cardValue == _topCard.cardValue) //Same number?
                    return true;
            }
        }
        else //Not a numbered card or wild, but a reverse, skip, draw2, etc. 
        {
            if (topCard.GetType() == card.GetType()) //Both skips/ both reverses, etc. ?
                return true;
        }

        if (card.type != 0) //Color != misc, but yellow, red, etc.
        {
            if (card.type == topCard.type) //Same color?
                return true;
        }

        return false;
    }

    #region PLAY CARD

    public void PlayCard(Card card)
    {
        card.OnCardPlayed();

        player.RemoveCardAt(player.cardObjs.IndexOf(card));
        player.cardObjs.Remove(card);

        CmdDiscard(card.ID);
        Destroy(card.gameObject);

        getSelection = false;
    }

    [Command]
    public void CmdDiscard(int id) //TODO: Might be able to move this over to the turn manager or something. 
    {
        //Pos and rot
        Vector3 position = new Vector3(0f.GiveOrTake(0.1f), 0.01f, 1f.GiveOrTake(0.1f));
        Quaternion rotation = Quaternion.Euler(90, 0, Random.Range(0f, 360f));

        GameObject newCard = Instantiate(player.deck.cards[id], position, rotation); //Creation

        if (newCard.TryGetComponent(out Collider col))
            Destroy(col);

        newCard.GetComponent<Card>().defaultPos = Vector3.zero;

        newCard.GetComponent<NetworkMatchChecker>().matchId = player.matchID.ToGuid();
        NetworkServer.Spawn(newCard); //Spawn it on the server

        TurnManager.instance.discard.Insert(0, newCard.gameObject); //Add it to the discard list

        //Remove old cards
        if (DiscardCount > 10)
        {
            GameObject oldestCard = DiscardBottom.gameObject;

            TurnManager.instance.discard.Remove(oldestCard);
            TurnManager.instance.cardNums[id]++;

            Destroy(oldestCard);
        }

        //Update sprite sorting order
        for (int i = 0; i < DiscardCount; i++)
        {
            RpcSetSortingOrder(TurnManager.instance.discard[i], i);
        }
    }

    [ClientRpc]
    public void RpcSetSortingOrder(GameObject gameObject, int index)
    {
        Card card = gameObject.GetComponent<Card>();

        card.defaultPos = Vector3.zero;

        card.spriteRenderer.sortingLayerName = "DiscardPile";
        card.sortingOrder = DiscardCount - index;
    }

    public void SetMyTurn(bool value)
    {
        TargetSetMyTurn(value);
    }

    [TargetRpc]
    private void TargetSetMyTurn(bool value)
    {
        getSelection = value;
    }

    #endregion

    [Command]
    public void CmdDrawCard()
    {
        TurnManager.instance.DealCard(player);
    }

    #region READ_ONLY

    public int DiscardCount { get { return TurnManager.instance.discard.Count; } }
    public Card DiscardTop
    {
        get
        {
            if (TurnManager.instance.discard[0].TryGetComponent(out Card card))
                return card;
            else return null;
        }
    }
    public Card DiscardBottom
    {
        get
        {
            if (TurnManager.instance.discard[DiscardCount - 1].TryGetComponent(out Card card))
                return card;
            else return null;
        }
    }

    #endregion
}
