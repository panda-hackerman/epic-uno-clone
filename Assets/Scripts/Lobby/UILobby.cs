using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Convert;
using UnityEngine.SceneManagement;

namespace Lobby
{
    /* This script handles every interaction you need to have with
     * the UI and making stuff do stuff. For example, if you need a button to
     * start the game or change a bool on the server, make a method here that
     * does that.
     */

    public class UILobby : MonoBehaviour
    {
        public static UILobby instance; //Its a me

        [Header("Host")]
        public InputField joinMatchInput;
        public Button joinButton;
        public Button hostButton;
        public Canvas lobbyCanvas;
        public GameObject connectCanvas;

        [Header("Lobby")]
        public Transform UIPlayerParent;
        public GameObject UIPlayerPrefab;
        public GameObject beginGameButton;
        public Text matchIdText;

        [Header("Player Info")]
        [SerializeField] string playerName;
        [SerializeField] Texture2D playerIcon;
        public Texture2D defaultIcon;

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

        public void HostSuccess(bool success, string matchID)
        {
            if (success) //If the host was a success (we get this info from player's target rpc) then enable the lobby canvas
            {
                lobbyCanvas.enabled = true;
                beginGameButton.SetActive(true);
                matchIdText.text = matchID;
                SpawnPlayerUIPrefab(Player.localPlayer); //Add me to the display
            }
            else //Otherwise try again
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;

                Debug.Log("Trying again");
                Host();
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

        public void JoinSuccess(bool success, string matchID)
        {
            if (success) //If the join was a success then enable the lobby canvas
            {
                lobbyCanvas.enabled = true;
                matchIdText.text = matchID;
                SpawnPlayerUIPrefab(Player.localPlayer); //Add me to the display
            }
            else //Otherwise re-enable the buttons
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        public void SpawnPlayerUIPrefab(Player player) //Add player to the lobby display
        {
            GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
            newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
        }

        public void BeginGame()
        {
            Player.localPlayer.BeginGame();
        }

        #endregion

        public void ExitMatch()
        {
            if (!TurnManager.instance) //The turnmanager will not exist if we are only in the lobby
                Player.networkInterface.LeaveMatch(Player.localPlayer);
            else
                Player.networkInterface.LeaveMatch(Player.localPlayer, TurnManager.instance.gameObject);

            lobbyCanvas.enabled = false;
            joinMatchInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;

            foreach (Transform item in UIPlayerParent) //Clear display
            {
                Destroy(item.gameObject);
            }
        }

        public void ChangeName(string input)
        {
            playerName = input;
        }

        public void ChangeIcon(Toggle toggle)
        {
            if (toggle.isOn) playerIcon = (Texture2D)toggle.transform.GetChild(0).GetComponent<RawImage>().texture;
            else playerIcon = defaultIcon;
        }

        public void SubmitNameAndIcon()
        {
            Player.localPlayer.CmdUpdateNameAndIcon(Player.localPlayer.gameObject, playerName, playerIcon.ToPixels());

            connectCanvas.SetActive(true);
        }

        public void Copy()
        {
            matchIdText.text.CopyToClipboard();
        }

        public void OtherPlayerLeft(Player player)
        {
            foreach (Transform transform in UIPlayerParent)
            {
                UIPlayer uiPlayer = transform.GetComponent<UIPlayer>();

                if (uiPlayer.GetPlayer() == player)
                {
                    Destroy(uiPlayer.gameObject);
                    break;
                }
            }
        }

        public void CloseConnectPanel()
        {
            connectCanvas.SetActive(false);
        }

        public void ExitToMainMenu()
        {
            if (UneNetworkManager.instance)
            {
                Destroy(UneNetworkManager.instance.gameObject);
            }

            StartCoroutine(LoadMainMenu());
        }

        IEnumerator LoadMainMenu()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenu");

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}
