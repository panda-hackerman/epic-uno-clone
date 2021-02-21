using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Convert;
using UnityEngine.UI;
using Lobby;

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

    public bool localPause { get { return pauseMenu.activeSelf; } }
    public GameObject pauseMenu;
    public GameObject settingsMenu;

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

    public void ExitMatch()
    {
        UILobby.instance.ExitMatch();
    }

    public void SetCardColor(int color) //Actived by button push
    {
        chooseColorButtons.SetActive(false);

        Player.networkInterface.SetColor((CardType)color);

        if (TurnManager.instance.discard[0].TryGetComponent(out WildCard card))
        {
            if (card.skipPlayerNextTurn)
                Player.networkInterface.Skip();
            else
                Player.networkInterface.NextTurn();
        }
        else
        {
            Debug.LogWarning("Top discard card isn't of type WildCard. This probably shouldn't have happened, but that's okay.", TurnManager.instance.discard[0]);
            Player.networkInterface.NextTurn();
        }

    }

    public void SetPlayerTurn(int playerNum)
    {
        if (playerDisplay.childCount <= 0)
        {
            Debug.LogWarning("Where are the player displays? This probably shouldn't have happened. " +
                "Generating new player displays");

            foreach (GameObject gameObj in TurnManager.instance.players)
            {
                SpawnPlayerUIPrefab(gameObj.GetComponent<Player>());
            }
        }

        foreach (Transform child in playerDisplay)
        {
            if (child.TryGetComponent(out GameUIPlayer uiPlayer))
                uiPlayer.outline.SetActive(uiPlayer.player.playerID == playerNum);
        }
    }

    public void SetCardCount(Player player, int count)
    {
        if (playerDisplay.childCount <= 0)
        {
            Debug.LogWarning("Where are the player displays? This probably shouldn't have happened. " +
                "Generating new player displays");

            foreach (GameObject gameObj in TurnManager.instance.players)
            {
                SpawnPlayerUIPrefab(gameObj.GetComponent<Player>());
            }
        }

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

    public void OtherPlayerLeft(Player player)
    {
        foreach (Transform transform in gameUiPlayerParent)
        {
            GameUIPlayer gameUIPlayer = transform.GetComponent<GameUIPlayer>();

            if (gameUIPlayer.player == player)
            {
                Destroy(gameUIPlayer.gameObject);
                break;
            }
        }
    }

    public void PressedEsc()
    {
        if (!pauseMenu.activeSelf) //Paused menu closed
        {
            pauseMenu.SetActive(true);
        }
        else //Pause menu open
        {
            if (settingsMenu.activeSelf) //Close settings before the pause menu
                settingsMenu.SetActive(false);
            else
                pauseMenu.SetActive(false);
        }
    }
}
