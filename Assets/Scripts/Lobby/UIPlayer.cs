using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Convert;

namespace Lobby
{
    /* Representation of the player in the lobby.
     * Simply displays the player icon and name, aswell as updates the other
     * icons and names through a command and rpc.
     */
    public class UIPlayer : NetworkBehaviour
    {
        Player player;
        NetworkMatchChecker networkMatchChecker;

        public RawImage image;
        public Text text;

        private void Awake()
        {
            networkMatchChecker = GetComponent<NetworkMatchChecker>();
        }

        public void SetPlayer(Player player)
        {
            this.player = player;

            image.texture = player.iconData.ToTexture();
            text.text = player.username;

            networkMatchChecker.matchId = player.matchID.ToGuid();

            foreach (Transform child in transform.parent)
            {
                if (hasAuthority)
                    CmdUpdatePlayerInfo(child);
                else
                    break;
            }
        }

        [Command]
        public void CmdUpdatePlayerInfo(Transform player)
        {
            TargetUpdatePlayerInfo(player);
        }

        [TargetRpc]
        public void TargetUpdatePlayerInfo(Transform player)
        {
            UIPlayer UiPlayer = player.GetComponent<UIPlayer>();

            UiPlayer.text.text = UiPlayer.player.username;
            UiPlayer.image.texture = UiPlayer.player.iconData.ToTexture();
        }
    }
}
