using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Convert;
using UnityEngine.UI;

[System.Serializable]
public class Arrow //(you can collapse this)
{
    public Arrow() { }

    public GameObject gameObject;

    private Animator _animator = null;
    private Animator Animator
    {
        get
        {
            if (!_animator) _animator = gameObject.GetComponent<Animator>();
            return _animator;
        }
    }

    private SpriteRenderer _spriteRenderer = null;
    private SpriteRenderer SpriteRenderer
    {
        get
        {
            if (!_spriteRenderer) _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            return _spriteRenderer;
        }
    }

    public void SetReverse(bool value)
    {
        Animator.SetBool("isReversed", value);
        SpriteRenderer.flipX = value;
    }
}

public class UIGame : MonoBehaviour
{
    public static UIGame instance;

    public Arrow arrow = new Arrow(); //Arrow that shows the direction of the game

    public GameObject gameUIPlayerPrefab;
    public Transform gameUiPlayerParent;

    public Transform playerDisplay;
    public GameObject chooseColorButtons;

    [Header("Win state panel stuff")]
    public GameObject winStatePanel;
    public Text playerWonText;
    public Text waitingForHostText;
    public Button hostContinueButton;

    private void Start()
    {
        instance = this;
    }

    public void Continue()
    {
        Player.localPlayer.ContinueGame();
    }

    public void SetCardColor(int color) //Actived by button push
    {
        Player.networkInterface.SetColor((CardType)color);
        Player.networkInterface.NextTurn();

        chooseColorButtons.SetActive(false);
    }

    public void SetPlayerTurn(int playerNum)
    {
        foreach (Transform child in playerDisplay)
        {
            if (child.TryGetComponent(out GameUIPlayer uiPlayer))
                uiPlayer.outline.enabled = uiPlayer.player.playerID == playerNum;
        }
    }

    public void SetCardCount(Player player, int count)
    {
        foreach (Transform child in playerDisplay) //Loop through and find the player
        {
            GameUIPlayer uiPlayer = child.GetComponent<GameUIPlayer>();
            if (uiPlayer.player == player)
            {
                uiPlayer.cardCount.text = count.ToString();
                break;
            }
        }
    }

    public void StartGameSuccess() //Player calls this method locally when the game finishes loading
    {
        foreach (GameObject gameObj in TurnManager.instance.players)
        {
            SpawnPlayerUIPrefab(gameObj.GetComponent<Player>());
        }

        SetPlayerTurn(0);
    }

    public void DeclareWinnerSuccess(string winnerName, bool isHost)
    {
        winStatePanel.SetActive(true);
        playerWonText.text = winnerName + " Won!";

        if (isHost)
        {
            hostContinueButton.gameObject.SetActive(true);
            waitingForHostText.gameObject.SetActive(false);
        }
    }

    public void SpawnPlayerUIPrefab(Player player)
    {
        GameObject newUIPlayer = Instantiate(gameUIPlayerPrefab, gameUiPlayerParent);
        GameUIPlayer gameUiPlayer = newUIPlayer.GetComponent<GameUIPlayer>();

        gameUiPlayer.SetPlayer(player);
        gameUiPlayer.networkMatchChecker.matchId = player.matchID.ToGuid();
    }
}
