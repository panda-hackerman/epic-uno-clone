using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using AdvacedMathStuff;

public class DiscardPile : NetworkBehaviour
{
    public static DiscardPile instance;
    public UneDeck deck; //Card prefabs

    [SerializeField]
    private int maxDiscardSize = 10; //Add bottom card to draw pile after this amount
    private SyncListGameObject discard = new SyncListGameObject(); //Keep private, should add cards through Discard

    #region READ_ONLY

    public int CardCount { get { return discard.Count; } }
    public Card TopCard { get { return discard[0].GetComponent<Card>(); } }
    public Card BottomCard { get { return discard[discard.Count - 1].GetComponent<Card>(); } }
    #endregion

    private void Start()
    {
        instance = this;
    }

    public void Discard(Card card)
    {
        CmdAddToDiscard(card.ID);
        Destroy(card.gameObject);
    }

    private void CmdAddToDiscard(int id)
    {
        //Make the new card
        Vector3 position = transform.position + new Vector3(
            0f.GiveOrTake(0.1f), 0,
            0f.GiveOrTake(0.1f));

        Quaternion rotation = Quaternion.Euler(90, 0, Random.Range(0f, 360f));

        GameObject newCard = Instantiate(deck.cards[id], position, rotation, transform);

        if (newCard.TryGetComponent(out Collider col)) Destroy(col); //Get rid of the collider

        discard.Insert(0, newCard);

        //Remove old cards
        if (discard.Count > maxDiscardSize)
        {
            GameObject oldestCard = BottomCard.gameObject;

            discard.Remove(oldestCard);
            Destroy(oldestCard);

            TurnManager.instance.cardNums[id]++;
        }

        //Update sprite sorting order
        for (int i = 0; i < CardCount; i++)
        {
            Card card = discard[i].GetComponent<Card>();

            card.defaultPos = Vector3.zero;

            card.spriteRenderer.sortingLayerName = "DiscardPile";
            card.sortingOrder = CardCount - i;
        }
    }
}
