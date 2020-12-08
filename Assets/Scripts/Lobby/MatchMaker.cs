using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Convert;

namespace Lobby
{
    [System.Serializable]
    public class Match //Match class holds all the information for the match
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
    public class SyncListGameObject : SyncList<GameObject> { }

    [System.Serializable]
    public class SyncListMatch : SyncList<Match> { }

    public class MatchMaker : NetworkBehaviour
    {

        public static MatchMaker instance;

        public SyncListMatch matches = new SyncListMatch();
        public SyncListString matchIDs = new SyncListString();

        public GameObject turnManagerPrefab;

        private void Start()
        {
            instance = this;
        }

        public bool HostGame(string matchID, GameObject player)
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

        public bool JoinGame(string matchID, GameObject player)
        {
            if (matchIDs.Contains(matchID))
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    if (matches[i].matchID == matchID)
                    {
                        matches[i].players.Add(player);
                        break;
                    }
                }
                Debug.Log("Match joined");
                return true;
            }
            else
            {
                Debug.Log("Match ID does not"); //TODO: Get another code
                return false;
            }
        }

        public void BeginGame(string matchID)
        {
            GameObject newTurnManager = Instantiate(turnManagerPrefab);
            newTurnManager.GetComponent<NetworkMatchChecker>().matchId = matchID.ToGuid();
            TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();

            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == matchID)
                {
                    foreach (GameObject player in matches[i].players)
                    {
                        Player _player = player.GetComponent<Player>();
                        turnManager.AddPlayer(_player);
                        _player.StartGame();
                    }
                    break;
                }
            }
        }

        //Generate an ID for the match
        public static string GetRandomMatchID ()
        {
            string id = string.Empty;

            for (int i = 0; i < 5; i++)
            {
                int random = Random.Range(0, 36); // A-Z, 0-9

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