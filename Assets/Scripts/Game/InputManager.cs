using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Lobby;

public class InputManager : MonoBehaviour
{
    public bool getSelection = true; //TODO: No need to get the player's selected card if it isn't their turn

    private GameObject selection;
    private Card selectedCard
    {
        get
        {
            if (!selection) return null;
            if (selection.TryGetComponent(out Card card))
                return card;
            else return null;
        }
    }

    private Player player;
    private Camera cam;

    private void Start()
    {
        player = GetComponent<Player>();
        getSelection = false;
    }

    private void Update()
    {
        if (getSelection)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(mousePos); //TODO: Cache camera

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

    public void OnInit() //Runs when the match starts, called by player.css
    {
        cam = GameObject.FindGameObjectWithTag("GameCamera").GetComponent<Camera>();
        getSelection = true;
    }

    public void OnSelect() //Click!
    {
        if (!selectedCard) return;

        if (CanPlayCard(selectedCard))
            DiscardPile.instance.Discard(selectedCard);
    }

    public bool CanPlayCard(Card card)
    {
        if (DiscardPile.instance.CardCount == 0) //modCheck cards?
            return true;

        if (card is WildCard)
            return true;

        Card topCard = DiscardPile.instance.TopCard;

        if (card is NumberedCard)
        {
            if (topCard is NumberedCard)
            {
                NumberedCard _card = card as NumberedCard;
                NumberedCard _topCard = topCard as NumberedCard;

                if (_card.cardValue == _topCard.cardValue) //Same number?
                    return true;
            }
        }
        else
        {
            //TODO: C# 8.0, topCard is card.GetType()
            if (topCard.GetType() == card.GetType()) //Both skips/ both reverses?
                return true;
        }

        if (card.type != 0) //Color
        {
            if (card.type == topCard.type) //Same color?
                return true;
        }

        return false;
    }
}
