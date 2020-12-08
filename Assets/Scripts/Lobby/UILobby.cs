using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class UILobby : MonoBehaviour //This script handles everything the UI needs to intercact with, such as pressing a button to host or join a game
    {
        public static UILobby instance; //Its a me

        [Header("Host")]
        public InputField joinMatchInput;
        public Button joinButton;
        public Button hostButton;
        public Canvas lobbyCanvas;

        [Header("Lobby")]
        public Transform UIPlayerParent;
        public GameObject UIPlayerPrefab;
        public GameObject beginGameButton;

        [Header("Other")]
        public GameObject fallingCards;

        private void Start()
        {
            instance = this;
        }

        #region HOST
        public void Host() //Deactivate buttons and tell the player to host the game
        {
            joinMatchInput.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;

            Player.localPlayer.HostGame();
        }

        public void HostSuccess(bool success)
        {
            if (success) //If the host was a success (we get this info from player's target rpc) then enable the lobby canvas
            {
                lobbyCanvas.enabled = true;
                beginGameButton.SetActive(true);
                SpawnPlayerUIPrefab(Player.localPlayer); //Add me to the display
            }
            else //Otherwise re-enable the buttons
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        #endregion

        #region JOIN

        public void Join() //Deactivate buttons and tell the player to attempt to join a game
        {
            joinMatchInput.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;

            Player.localPlayer.JoinGame(joinMatchInput.text);
        }

        public void JoinSuccess(bool success)
        {
            if (success) //If the join was a success then enable the lobby canvas
            {
                lobbyCanvas.enabled = true;
                SpawnPlayerUIPrefab(Player.localPlayer); //Add me to the display
            }
            else //Otherwise re-enable the buttons
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        public void SpawnPlayerUIPrefab(Player player)
        {
            GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
            newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
        }

        #endregion

        public void BeginGame()
        {
            Player.localPlayer.BeginGame();
            fallingCards.SetActive(false);
        }

    }
}
