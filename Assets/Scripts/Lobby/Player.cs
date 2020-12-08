using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Convert;

namespace Lobby
{
    public class Player : NetworkBehaviour
    {
        public static Player localPlayer; //me :)

        [SyncVar] public string matchID;

        NetworkMatchChecker networkMatchChecker; //Guid goes here

        private void Start()
        {

            networkMatchChecker = GetComponent<NetworkMatchChecker>();

            if (isLocalPlayer)
            {
                localPlayer = this; //Finding myself
            }
            else
            {
                UILobby.instance.SpawnPlayerUIPrefab(this);
            }

        }

        #region HOST
        public void HostGame() //Host a game and generate a code
        {
            string matchID = MatchMaker.GetRandomMatchID();

            CmdHostGame(matchID);
        }

        [Command] //Runs on the host
        void CmdHostGame(string matchID) //Attempt to host a game
        {
            this.matchID = matchID;
            if (MatchMaker.instance.HostGame(matchID, gameObject))
            {
                Debug.Log("Game hosted succesfully");
                networkMatchChecker.matchId = matchID.ToGuid();
                TargetHostGame(true, matchID); //Successfully hosted the game
            }
            else
            {
                Debug.LogWarning("Game host failed");
                TargetHostGame(false, matchID); //Tell the host the host failed
            }
        }

        //Tell the host client if the host was successful
        [TargetRpc]
        void TargetHostGame(bool success, string matchID)
        {
            UILobby.instance.HostSuccess(success);
        }

        #endregion

        #region JOIN

        public void JoinGame(string inputID) //Join a game with the given code
        {
            CmdJoinGame(inputID);
        }

        [Command] //Runs on the host
        void CmdJoinGame(string matchID) //Attempt to join a game
        {
            this.matchID = matchID;
            if (MatchMaker.instance.JoinGame(matchID, gameObject))
            {
                Debug.Log("Game Joined succesfully");
                networkMatchChecker.matchId = matchID.ToGuid();
                TargetJoinGame(true, matchID); //Successfully Joined the game
            }
            else
            {
                Debug.LogWarning("Game Join failed");
                TargetJoinGame(false, matchID); //Tell the Join the Join failed
            }
        }

        //Tell the Join if the game Join was successful
        [TargetRpc]
        void TargetJoinGame(bool success, string matchID)
        {
            UILobby.instance.JoinSuccess(success);
        }

        #endregion

        #region BEGIN

        public void BeginGame() //Start the game
        {
            CmdBeginGame();
        }

        [Command] //Runs on the host
        void CmdBeginGame() //Start the game on the host
        {
            MatchMaker.instance.BeginGame(matchID);
            Debug.Log("Beginning game");
        }

        public void StartGame()
        {
            TargetBeginGame();
        }

        //Load the game scene
        [TargetRpc] //Runs on every client
        void TargetBeginGame()
        {
            Debug.Log("Beginning game | " + matchID);

            //Additively load game scene
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
        }

        #endregion
    }
}
