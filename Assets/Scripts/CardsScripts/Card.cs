using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { other, yellow, red, blue, green };

public abstract class Card : NetworkBehaviour
{
    public GameObject prefab;

    public Sprite cardSprite;
    public Vector3 restingPos, selectedPos;
    public CardType type;
    public bool IsSelected { set { transform.position = value ? selectedPos : restingPos; } }

    private void Start()
    {
        //cardSprite = GetComponent<Sprite>();
    }

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
