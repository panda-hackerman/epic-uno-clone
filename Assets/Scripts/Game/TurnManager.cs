using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Lobby;
using AdvacedMathStuff;
using UnityEngine.SceneManagement;

/* There is one turn manager for every game going on, it handles whose turn
 * it is and all the players in the game.
 * Aswell as... other stuff (see below)
 */

public class TurnManager : NetworkBehaviour
{
    public static TurnManager instance;

    public List<Player> players = new List<Player>();
    public UneDrawPile drawPile;

    private void Start()
    {
        instance = this;
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public void GameStart() //When the game start
    {
        //TODO: Deal cards
        Debug.Log("The game has started!");

        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("MainGame"));

        foreach (Player player in players)
        {
            for (int i = 0; i < 8; i++)
            {
                DealCard(player);
            }
        }
    }

    public void GameEnd() //When the game end
    {
        //TODO: Make this called
        //TODO: Unload game scene
        //TODO: Remove match from matchID if lobby closed
        //TODO: Delete this object?
        Debug.Log("The game has ended!");
    }

    public void DealCard(Player player)
    {
        int card = PickCard();

        player.DealCard(card); //Player adds this card to their hand
        drawPile.cards[card].numberInDeck--; //Remove one of this from the draw pile
    }

    public int PickCard() //Returns a random card from the draw pile
    {
        int cardsLeft = drawPile.cards.Count; //Cards in draw pile

        double[] cardWeights = new double[cardsLeft]; //Each double represents how many of that card there is left

        for (int i = 0; i < cardsLeft; i++)
        {
            cardWeights[i] = drawPile.cards[i].numberInDeck;
        }

        int index = AdvMath.Roulette(cardWeights); //Cards with a higher count are more likely to be picked

        return index;
    }
}
