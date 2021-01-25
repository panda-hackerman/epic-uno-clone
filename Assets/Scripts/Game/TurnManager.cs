using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Lobby;
using AdvacedMathStuff;
using Convert;

/* There is one turn manager for every game going on, it handles whose turn
 * it is and all the players in the game.
 * Aswell as... other stuff (see below) */

public class TurnManager : NetworkBehaviour
{

    //Player stuff
    public SyncListGameObject players = new SyncListGameObject();
    [SyncVar] public int currentPlayer;

    //Draw pile
    public SyncListInt cardNums = new SyncListInt() {
        4, 4, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };

    //Discard pile
    public SyncListGameObject discard = new SyncListGameObject();

    //Misc
    public static TurnManager instance; //Me :)
    [SyncVar] public bool isReversed = false;

    private void Start()
    {
        instance = this;
    }

    public void AddPlayer(Player player)
    {
        players.Add(player.gameObject);
    }

    public void GameStart() //When the game start
    {
        //TODO: Add card to middle when game starts
        Debug.Log("The game has started!");

        for (int i = 0; i < players.Count; i++) //Evvvrybody
        {
            Player player = players[i].GetComponent<Player>();

            player.playerID = i;

            //Deal cards
            for (int j = 0; j < 8; j++)
            {
                DealCard(player);
            }
        }
    }

    public void GameEnd() //When the game ends
    {
        //TODO: Make this called
        //TODO: Unload game scene
        //TODO: Remove match from matchID if lobby closed
        //TODO: Delete this object/ reset?
        Debug.Log("The game has ended!");
    }

    public void DealCard(Player player)
    {
        int[] weights = new int[54];
        cardNums.CopyTo(weights, 0);

        int card = AdvMath.Roulette(weights); //Choose a card based on how many there are (more in deck = more likely to pick)

        player.DealCard(card); //Player adds this card to their hand
        cardNums[card]--; //Remove one of this from the draw pile
    }

    public void NextTurn()
    {
        int upperBound = players.Count - 1; //lower bound is 0

        if (!isReversed) currentPlayer++; //Clockwise
        else currentPlayer--; //Counterclockwise

        if (currentPlayer < 0)
            currentPlayer = upperBound;
        else if (currentPlayer > upperBound)
            currentPlayer = 0;

        Debug.Log($"It is now Player {currentPlayer}'s turn");

        RpcSetTurnDisplay(currentPlayer);

        players[currentPlayer].GetComponent<InputManager>().SetMyTurn(true); //Tell the player they are now able to select cards
    }

    public void Skip()
    {
        int upperBound = players.Count - 1;

        if (!isReversed) currentPlayer++;
        else currentPlayer--;

        if (currentPlayer < 0) //Too low
            currentPlayer = upperBound;
        else if (currentPlayer > upperBound) //Too high
            currentPlayer = 0;

        int skippedPlayer = currentPlayer;

        if (!isReversed) currentPlayer++;
        else currentPlayer--;

        if (currentPlayer < 0) //Too low
            currentPlayer = upperBound;
        else if (currentPlayer > upperBound) //Too high
            currentPlayer = 0;

        Debug.Log($"Skipped Player {skippedPlayer}, it is now Player {currentPlayer}'s turn");

        RpcSetTurnDisplay(currentPlayer);

        players[currentPlayer].GetComponent<InputManager>().SetMyTurn(true);
    }

    public void Plus(int count)
    {
        Debug.Log("Plus !!!!!!!!!!");

        if (count <= 0) return;

        int upperBound = players.Count - 1;
        int nextPlayer = currentPlayer;

        if (!isReversed) nextPlayer++;
        else nextPlayer--;

        if (nextPlayer < 0)
            nextPlayer = upperBound;
        else if (nextPlayer > upperBound)
            nextPlayer = 0;

        Player player = players[nextPlayer].GetComponent<Player>(); //Player who is recieving the cards

        for (int i = 0; i < count; i++)
        {
            Debug.Log($"Run {i} times :)");
            DealCard(player);
        }
    }

    public void SetGameColor(CardType color)
    {
        foreach (GameObject gameObject in players)
        {
            NetworkingInterface nwInterface = gameObject.GetComponent<NetworkingInterface>();

            nwInterface.TargetSetColor(color);
        }
    }

    [ClientRpc]
    public void RpcSetTurnDisplay(int player)
    {
        CanvasInfo.canvas.SetPlayerTurn(player);
    }

}
