using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lobby;
using UnityEngine.UI;
using Convert;
using Mirror;
using TMPro;

public class GameUIPlayer : NetworkBehaviour
{
    public Player player;

    public RawImage image;
    public Text text;
    public TMP_Text cardCount;
    public GameObject outline;
    public Image crown;

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
        Debug.Log("Updating player info...");
        GameUIPlayer gameUIPlayer = uiPlayer.GetComponent<GameUIPlayer>();

        uiPlayer.SetSiblingIndex(position);

        gameUIPlayer.text.text = gameUIPlayer.player.username;
        gameUIPlayer.image.texture = gameUIPlayer.player.iconData.ToTexture();
        gameUIPlayer.cardCount.text = gameUIPlayer.player.cardCount.ToString();
        gameUIPlayer.crown.enabled = gameUIPlayer.player.isHost;

        Debug.Log("Updated player info!" +
            $"Player '{gameUIPlayer.player.name}' has {gameUIPlayer.cardCount} cards, and" +
            $"{(gameUIPlayer.player.isHost ? "is": "is not")} the host", gameUIPlayer);
    }
}