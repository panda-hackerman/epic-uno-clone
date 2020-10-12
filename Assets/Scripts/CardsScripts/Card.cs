using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { other, yellow, red, blue, green };

public abstract class Card : NetworkBehaviour
{
    public int ID = -1;
    public CardType type;
    [HideInInspector] public Vector3 restingPos, selectedPos;
    public bool IsSelected { set { transform.position = value ? selectedPos : restingPos; } }

    public virtual void Start() { }

    public virtual void Update() { }

    public virtual void DrawCard()
    {
        // You should override this function
        // Called when this card is drawn from the pile (or dealt at the beginning of the game)
    }

    public virtual void PlayCard()
    {
        // You should override this function
        // Called when this card is discarded
    }
}
