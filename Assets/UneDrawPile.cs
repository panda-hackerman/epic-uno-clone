using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class SyncListCardInfo : SyncList<CardInfo> { }

/* I put the drawPile in here because it was really long and you took up the entire space
 * of the turn manager inspector. Also, it's better for making alternate decks (if you're 
 * into that kinda thing)
 */

public class UneDrawPile : NetworkBehaviour
{
    [SyncVar] public List<CardInfo> cards = new List<CardInfo>(new CardInfo[54]); //Empty list with 54 items
}
