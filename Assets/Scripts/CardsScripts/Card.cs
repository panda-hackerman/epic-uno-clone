using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { other, yellow, red, blue, green };

public abstract class Card : NetworkBehaviour
{
    public int ID = -1;
    public CardType type;
    public bool wildCard = false;
    [HideInInspector] public Vector3 restingPos, selectedPos;
    public bool IsSelected { set { transform.position = value ? selectedPos : restingPos; } }

    public virtual void Start() { }

    public virtual void Update() { }

    public virtual void DrawCard()
    {
        // Called when this card is drawn or dealt
    }

    public virtual void PlayCard()
    {
        // Called when this card is discarded
    }
}
