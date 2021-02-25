using UnityEngine;
using Mirror;
using AdvacedMathStuff;
using System.Collections;
using System.Collections.Generic;
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
    public SyncListInt cardNums;

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
        Debug.Log($"TurnManager: Game started | {players[0].GetComponent<Player>().matchID}");

        cardNums = new SyncListInt() {
        4, 4, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };

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

        //Add card to middle
        int firstCard = GetInitialCard();
        PlaceInitialCard(firstCard);
    }

    public void ContinueGame()
    {
        foreach (GameObject playerObj in players)
        {
            Player player = playerObj.GetComponent<Player>();
            player.UnloadGame();
        }
    }

    void PlaceInitialCard(int id)
    {
        Player arbitraryPlayer = players[0].GetComponent<Player>(); //Arbitrary but needed for certain values (which should be the same across all players)

        Vector3 position = new Vector3(0f.GiveOrTake(0.1f), 0.01f, 1f.GiveOrTake(0.1f));
        Quaternion rotation = Quaternion.Euler(90, 0, Random.Range(0f, 360f));

        GameObject newCard = Instantiate(arbitraryPlayer.deck.cards[id], position, rotation);

        if (newCard.TryGetComponent(out Collider col))
            Destroy(col);

        newCard.GetComponent<Card>().defaultPos = Vector3.zero;

        newCard.GetComponent<NetworkMatchChecker>().matchId = arbitraryPlayer.matchID.ToGuid();
        NetworkServer.Spawn(newCard.gameObject); //Spawn it on the server

        discard.Insert(0, newCard);
    }

    int GetInitialCard()
    {
        //Alternate card chances, special cards cannot be the first card in the deck
        List<int> weights = new List<int>() {
        0, 0, 1, 1, 1, 1, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0,};

        int card = AdvMath.Roulette(weights.ToArray());
        cardNums[card]--;

        return card;
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

        Debug.Log($"It is now Player {currentPlayer}'s turn | {players[0].GetComponent<Player>().matchID}");

        SetTurnDisplay(currentPlayer);

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

        SetTurnDisplay(currentPlayer);

        players[currentPlayer].GetComponent<InputManager>().SetMyTurn(true);
    }

    public void Plus(int count)
    {
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

    public void SetTurnDisplay(int playerNum) //Display at the top to indicate whose turn it is
    {
        foreach (GameObject gameObj in players)
        {
            NetworkingInterface player = gameObj.GetComponent<NetworkingInterface>();

            player.TargetSetTurnDisplay(playerNum);
        }
    }

    public void UpdateCardCount()
    {
        foreach (GameObject mainObj in players) //We will run the TargetRpc on every player
        {
            NetworkingInterface mainPlayer = mainObj.GetComponent<NetworkingInterface>(); //Player that the TargetRpc will be called on

            foreach (GameObject targetObj in players) //Update the card count for all of these players
            {
                Player targetPlayer = targetObj.GetComponent<Player>();
                mainPlayer.TargetSetCardCount(targetObj, targetPlayer.cardCount);
            }
        }
    }

    public void UpdateHostDisplay()
    {
        foreach (GameObject playerObj in players)
        {
            NetworkingInterface player = playerObj.GetComponent<NetworkingInterface>();

            player.TargetSetHost(); //TODO: Store host in match class?
        }
    }

    public void DeclareWinner(string winnerName)
    {
        foreach (GameObject playerObject in players)
        {
            Player player = playerObject.GetComponent<Player>();

            player.TargetDeclareWinner(winnerName);
        }
    }

}
