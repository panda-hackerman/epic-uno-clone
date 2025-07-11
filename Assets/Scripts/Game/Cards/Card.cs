﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { other, yellow, red, blue, green };

public abstract class Card : NetworkBehaviour
{
    public CardType type;

    public int ID = -1;

    public bool wildCard = false;

    public bool isSelected;
    public int sortingOrder;

    [SyncVar] public Vector3 defaultPos = new Vector3(0, 1.75f, 0);

    private void Update()
    {
        spriteRenderer.transform.localPosition = defaultPos;

        if (isSelected)
        {
            spriteRenderer.transform.position += 0.25f * spriteRenderer.transform.up;
            spriteRenderer.sortingOrder = 100;
        }
        else
        {
            spriteRenderer.sortingOrder = sortingOrder;
        }
    }

    public virtual void OnCardDrawn()
    {
        // Called when this card is drawn or dealt
        Debug.Log($"Drew card '{name}'");
    }

    public virtual void OnCardPlayed() 
    {
        Player.networkInterface.NextTurn();
    }

    #region READ_ONLY

    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer spriteRenderer
    {
        get
        {
            if (!_spriteRenderer)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            return _spriteRenderer;
        }
    }

    #endregion
}
