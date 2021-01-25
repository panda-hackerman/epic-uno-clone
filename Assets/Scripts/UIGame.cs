using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lobby;
using Mirror;
using Convert;

public class UIGame : MonoBehaviour
{
    public static UIGame instance;

    public GameObject gameUIPlayerPrefab;
    public Transform gameUiPlayerParent;

    private void Start()
    {
        instance = this;
    }

    public void SetCardColor(int color) //Actived by button push
    {
        Player.networkInterface.SetColor((CardType)color);
        Player.networkInterface.NextTurn();

        CanvasInfo.canvas.chooseColorButtons.SetActive(false);
    }

    public void StartGameSuccess() //Player calls this method locally when the game finishes loading
    {
        foreach (GameObject gameObj in TurnManager.instance.players)
        {
            SpawnPlayerUIPrefab(gameObj.GetComponent<Player>());
        }

        CanvasInfo.canvas.SetPlayerTurn(0);
    }

    public void SpawnPlayerUIPrefab(Player player)
    {
        GameObject newUIPlayer = Instantiate(gameUIPlayerPrefab, gameUiPlayerParent);
        GameUIPlayer gameUiPlayer = newUIPlayer.GetComponent<GameUIPlayer>();

        gameUiPlayer.SetPlayer(player);
        gameUiPlayer.networkMatchChecker.matchId = player.matchID.ToGuid();
    }
}
