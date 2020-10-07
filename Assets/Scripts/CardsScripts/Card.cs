using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { other, yellow, red, blue, green };

public abstract class Card : MonoBehaviour
{

    // very, very, very, very, very simple card data stats
    public CardType type;

    public virtual void DrawCard()
    {
        // Override
        // TODO: Add to deck
    }

    public virtual void PlayCard()
    {
        // Override
        // TODO: Contact the server and tell it to play the card
    }
}
