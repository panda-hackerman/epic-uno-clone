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

        public bool HostGame(string matchID, GameObject player) //Self explanitory; attempt to host the game
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
                for (int i = 0; i < matches.Count; i++)
                {
                    if (matches[i].matchID == matchID)
                    {
                        //TODO: Check if there are already 8 players in the lobby
                        matches[i].players.Add(player);
                        break;
                    }
                }
                Debug.Log("Match joined");
                return true;
            }
            else
            {
                Debug.Log("Match ID does not exist"); //TODO: Maybe tell the player this through a pop up or error.
                return false;
            }
        }

        public void BeginGame(string matchID) //When the host clicks 'begin game'
        {
            GameObject newTurnManager = Instantiate(turnManagerPrefab);
            NetworkServer.Spawn(newTurnManager);
            newTurnManager.GetComponent<NetworkMatchChecker>().matchId = matchID.ToGuid();
            TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();

            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == matchID) //Loop thru the matches and look for the one with a matching id
                {
                    foreach (GameObject player in matches[i].players) //Call this on every player
                    {
                        Player _player = player.GetComponent<Player>();
                        turnManager.AddPlayer(_player);
                        _player.StartGame(); //Tell the player to start the game. Loads the scene and other stuff.
                    }

                    turnManager.GameStart();
                    break; //we found the right match so stop searching
                }
            }
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