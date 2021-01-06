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

    public List<int> cardNums = new List<int>(); //Pos in list = card id, value = number currently in deck

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
        //TODO: Add card to middle when game starts
        Debug.Log("The game has started!");

        foreach (Player player in players)
        {
            Debug.Log($"Dealing cards to {player.name}");

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
        int card = AdvMath.Roulette(cardNums.ToArray()); //Choose a card based on how many there are (more in deck = more likely to pick)

        Debug.Log($"Chose card #{card}!");

        player.DealCard(card); //Player adds this card to their hand
        cardNums[card]--; //Remove one of this from the draw pile
    }
}
