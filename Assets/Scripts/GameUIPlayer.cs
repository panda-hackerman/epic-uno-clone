using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lobby;
using UnityEngine.UI;
using Convert;
using Mirror;

public class GameUIPlayer : NetworkBehaviour
{
    public Player player;

    public RawImage image;
    public Text text;
    public Outline outline;

    public NetworkMatchChecker networkMatchChecker;

    public void SetPlayer(Player player)
    {
        this.player = player;

        image.texture = player.iconData.ToTexture();
        text.text = player.username;

        networkMatchChecker.matchId = player.matchID.ToGuid();

        foreach (Transform child in transform.parent)
        {
            if (hasAuthority)
                CmdUpdatePlayerInfo(child, player.playerID);
            else
                break;
        }
    }

    [Command]
    public void CmdUpdatePlayerInfo(Transform player, int position)
    {
        TargetUpdatePlayerInfo(player.parent, position);
    }

    [TargetRpc]
    public void TargetUpdatePlayerInfo(Transform uiPlayer, int position)
    {
        GameUIPlayer gameUIPlayer = uiPlayer.GetComponent<GameUIPlayer>();

        uiPlayer.SetSiblingIndex(position);

        gameUIPlayer.text.text = gameUIPlayer.player.username;
        gameUIPlayer.image.texture = gameUIPlayer.player.iconData.ToTexture();
    }
}
