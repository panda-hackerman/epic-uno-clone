using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class UIPlayer : MonoBehaviour //Representation of the player in the lobby
    {
        public Text text;
        Player player;

        public void SetPlayer(Player player)
        {
            this.player = player;
            text.text = "Example Name"; //TODO: Player names
        }
    }
}
