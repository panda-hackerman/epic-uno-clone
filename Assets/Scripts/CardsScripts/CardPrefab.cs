using UnityEngine;

/* The card prefab class holds important information about a card, such as
 * how many exist in the deck, the prefab, and a reference to the card class.
 * The reason this information is not held in the card itself is that while a
 * Card represents an physical object that has been spawned in the game world,
 * A CardPrefab exists to list information for a card that has not been spawned yet,
 * and provide information that is required to spawn it in the first place.
 */

[System.Serializable]
public class CardPrefab
{
    public int numberInDeck;

    public GameObject prefab;

    private Card _card;
    public Card card
    {
        get
        {
            if (!_card) _card = prefab.GetComponent<Card>();
            return _card;
        }
        set { _card = value; }
    }
}

