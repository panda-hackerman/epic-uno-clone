using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { other, yellow, red, blue, green };

public abstract class Card : NetworkBehaviour
{
    public CardType type;

    public int ID = -1;

    public bool wildCard = false;
    public bool callNextTurn = true;

    [HideInInspector] public Vector3 restingPos, selectedPos;
    [HideInInspector] public PlayerManager myPlayer;

    public virtual void Start() { }

    public virtual void Update() { }

    public bool IsSelected { set { transform.position = value ? selectedPos : restingPos; } }

    public virtual void DrawCard()
    {
        // Called when this card is drawn or dealt
    }

    public virtual void PlayCard()
    {
        Debug.Log("Played Card: " + name);
        // Called when this card is discarded
    }
}
