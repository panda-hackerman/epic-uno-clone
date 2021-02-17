using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Convert;

[System.Serializable]
public class SyncListGameObject : SyncList<GameObject> { } //Sync for a list of gameobjects

namespace Lobby
{
    [System.Serializable]
    public class Match //The Match class holds all the information for the match, such as the id and the players
    {
        public Match() { } //Contructor

        public Match(string matchID, GameObject player) //Set default values and such
        {
            this.matchID = matchID;
            players.Add(player);
        }

        public string matchID; //The 6 digit ID

        public SyncListGameObject players = new SyncListGameObject(); //List of all the players in this match
    }

    [System.Serializable]
    public class SyncListMatch : SyncList<Match> { } //Sync for a list of matches

    /* The match maker... makes matches. As well as generally handling
     * matches, and joining an already made match. Also generates matchIDs.
     */

    public class MatchMaker : NetworkBehaviour
    {

        public static MatchMaker instance;

        public SyncListMatch matches = new SyncListMatch(); //TODO: Remove a match if every player leaves
        public SyncListString matchIDs = new SyncListString();

        public GameObject turnManagerPrefab;

        private void Start()
        {
            instance = this;
        }

        public bool HostGame(string matchID, GameObject player) //Attempt to host the game
        {
            if (!matchIDs.Contains(matchID))
            {
                matchIDs.Add(matchID);
                matches.Add(new Match(matchID, player));
                Debug.Log("Match generated");
                return true;
            }
            else
            {
                Debug.Log("Match ID already exists"); //TODO: Get another code
                return false;
            }
        }

        public bool JoinGame(string matchID, GameObject player) //Attempt to join the game
        {
            if (matchIDs.Contains(matchID))
            {
                FindMatchByID(matchID, out Match match);
                match.players.Add(player);

                Debug.Log("Match joined");
                return true;
            }
            else
            {
                Debug.Log("Match ID does not exist"); //TODO: Maybe tell the player this through a pop up thingy.
                return false;
            }
        }

        public void BeginGame(string matchID) //When the host clicks 'begin game'
        {
            GameObject newTurnManager = Instantiate(turnManagerPrefab);
            NetworkServer.Spawn(newTurnManager);
            newTurnManager.GetComponent<NetworkMatchChecker>().matchId = matchID.ToGuid();
            TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();

            FindMatchByID(matchID, out Match match);

            foreach (GameObject playerObject in match.players) //Call this on every player
            {
                Player player = playerObject.GetComponent<Player>();
                turnManager.AddPlayer(player);
                player.StartGame(); //Tell the player to start the game. Loads the scene and other stuff.
            }

            turnManager.GameStart();
        }

        public bool LeaveMatch(GameObject playerObject, TurnManager turnmanager = null) //When a player wants to leave the match
        {
            if (!playerObject.TryGetComponent(out Player player))
                throw new System.Exception("Couldn't find Player script of player object.");

            if (!FindMatchByID(player.matchID, out Match match))
                throw new System.Exception("Couldn't find match");

            if (turnmanager != null) //TurnManager will be null if the game scene is not loaded
            {
                turnmanager.players.RemoveAt(player.playerID);

                //Reset the playerIDs
                for (int i = 0; i < turnmanager.players.Count; i++)
                    turnmanager.players[i].GetComponent<Player>().playerID = i;

                //Turn goes to the next person if they leave on their turn
                if (turnmanager.currentPlayer == player.playerID)
                    turnmanager.NextTurn();

                //We need a new host if the host leaves
                if (player.isHost)
                    turnmanager.players[0].GetComponent<Player>().isHost = true;
            }

            match.players.Remove(playerObject);

            //Tell every other player that a player left (so they can update the display)
            foreach (GameObject otherPlayerObj in match.players)
            {
                Player otherPlayer = otherPlayerObj.GetComponent<Player>();
                otherPlayer.OtherPlayerLeft(player);
            }

            if (match.players.Count == 0) //This means there is no-one left in the match
            {
                Debug.Log($"Closing match | {player.matchID}");

                matches.Remove(match);
                matchIDs.Remove(match.matchID);
            }

            player.LeaveGame();

            return true;
        }

        public bool FindMatchByID(string matchID, out Match match)
        {
            match = null;

            if (matchIDs.Contains(matchID))
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    if (matches[i].matchID == matchID)
                    {
                        match = matches[i];
                        return true;
                    }
                }
            }

            return false;
        }

        //Generate an ID for the match
        public static string GetRandomMatchID()
        {
            string id = "";

            for (int i = 0; i < 5; i++)
            {
                int random = Random.Range(0, 36); //A-Z, 0-9

                if (random < 26)
                {
                    id += (char)(random + 65); //Start with capital A
                }
                else
                {
                    id += (random - 26).ToString();
                }
            }

            Debug.Log("Random Match ID: " + id);
            return id;
        }
    }
}