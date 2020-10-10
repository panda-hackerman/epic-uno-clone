using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { other, yellow, red, blue, green };

public abstract class Card : MonoBehaviour
{
    public Vector3 restingPos;
    public Vector3 selectedPos;
    public Sprite cardSprite;
    public CardType type;

    private void Start()
    {
        cardSprite = GetComponent<Sprite>();
    }

    public virtual void DrawCard()
    {
        // Called when this card is drawn from the pile (or at the beginning of the game)
        // Override
        // TODO: Add to deck
    }

    public virtual void PlayCard()
    {
        // Called when this card is discarded
        // Override
        // TODO: Contact the server and tell it to play the card
    }
}
