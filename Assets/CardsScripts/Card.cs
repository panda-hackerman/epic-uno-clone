using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    // very, very, very, very, very simple card data stats
    public CardType type;
    public enum CardType {red, blue, yellow, green, none};

    public virtual void DrawCard()
    {
        // Override
    }
}
