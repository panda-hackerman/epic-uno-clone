using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Convert;
using Lobby;
using Unity.Collections;

[System.Serializable]
public class SyncListImageData : SyncList<ImageData> { }

/* This is the script for both the player in the lobby and in game
 * a player is spawned as soon as you start the lobby scene, and
 * continues when you start the game. The lobby player and game player
 * are not seperate because that would require more work.
 */

public class Player : NetworkBehaviour
{
    [Header("My scripts")]
    public static Player localPlayer; //me :)
    public static NetworkingInterface networkInterface; //Command shit goes here :)
    public InputManager inputManager;
    private NetworkMatchChecker networkMatchChecker; //Guid goes here

    [Header("Info")]
    [SyncVar] public bool isHost; //TODO: Less oportunity for this to break if the host is stored as in the turn manager or better, the Match class. 
    [SyncVar] public int playerID;
    [SyncVar] public string matchID;
    [SyncVar] public string username;
    [SyncVar, HideInInspector]
    public ImageData iconData; //Raw bytes of image because mirror amirite

    [Header("Card shit")]
    public UneDeck deck; //List of card prefabs
    public CardHand myHand;
    [SyncVar] public int cardCount;
    private List<int> cardIDs = new List<int>(); //List of ids that represent the player's cards
    public List<Card> cardObjs = new List<Card>(); //Physical card objs (local only)

    [Header("Misc")]
    public GameObject gameUIPlayerPrefab;

    private void Start()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
        inputManager = GetComponent<InputManager>();

        if (isLocalPlayer)
        {
            localPlayer = this; //Finding myself
            networkInterface = GetComponent<NetworkingInterface>();
            name = $"Local Player";
        }
        else
        {
            UILobby.instance.SpawnPlayerUIPrefab(this); //Spawn the uiplayers that aren't me
            name = $"Player ({username})";
        }
    }

    public void Init() //When the host starts the game
    {
        BroadcastMessage("OnInit");

        UIGame.instance.StartGameSuccess();

        myHand.gameObject.SetActive(true);
    }

    public void AddCard(int value)
    {
        cardIDs.Add(value);
        CmdSetCardCount(cardIDs.Count);
    }

    public void RemoveCardAt(int index)
    {
        cardIDs.RemoveAt(index);
        CmdSetCardCount(cardIDs.Count);
    }

    public void DealCard(int id)
    {
        TargetAddCard(id);
    }

    [TargetRpc]
    public void TargetAddCard(int id)
    {
        GameObject prefab = deck.cards[id];
        GameObject newCard = Instantiate(prefab, myHand.Cards);
        Card card = newCard.GetComponent<Card>();

        card.OnCardDrawn();
        card.ID = id;

        AddCard(id);
        cardObjs.Add(card);
    }

    [Command]
    public void CmdUpdateNameAndIcon(GameObject player, string username, ImageData icon)
    {
        Player _player = player.GetComponent<Player>();

        _player.username = username;
        _player.iconData = icon;

    }

    [Command]
    public void CmdSetCardCount(int value)
    {
        cardCount = value;
        TurnManager.instance.UpdateCardCount();
        Debug.Log($"CardCount is now {value}!");

        //Declare le winner
        if (value <= 0) //This means this player has won; <= not == just incase I guess
        {
            TurnManager.instance.DeclareWinner(username);
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
            Debug.Log($"Game hosted succesfully with ID {matchID}");
            networkMatchChecker.matchId = matchID.ToGuid();
            TargetHostGame(true, matchID); //Successfully hosted the game

            isHost = true;
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
        UILobby.instance.HostSuccess(success, matchID);
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
            Debug.Log($"Game joined succesfully with ID {matchID}");
            networkMatchChecker.matchId = matchID.ToGuid();
            TargetJoinGame(true, matchID); //Successfully joined the game

            isHost = false;
        }
        else
        {
            Debug.LogWarning("Game Join failed");
            TargetJoinGame(false, matchID); //Tell the client the Join failed
        }
    }

    //Tell the client if the game Join was successful
    [TargetRpc]
    void TargetJoinGame(bool success, string matchID)
    {
        UILobby.instance.JoinSuccess(success, matchID);
    }

    #endregion

    #region BEGIN

    public void BeginGame() //Start the game
    {
        CmdBeginGame();
    }

    [Command] //Runs on the server
    void CmdBeginGame() //Start the game on the server
    {
        MatchMaker.instance.BeginGame(matchID);
        Debug.Log("Beginning game");
    }

    public void StartGame()
    {
        TargetBeginGame();
    }

    //Load the game scene
    [TargetRpc] //Like ClientRpc but only runs on one client
    void TargetBeginGame()
    {
        Debug.Log($"Beginning game | {matchID}");

        UILobby.instance.fallingCards.SetActive(false); //TODO: re-activate these when the game finishes

        StartCoroutine(AsyncLoadGame());
    }

    IEnumerator AsyncLoadGame()
    {
        //Additively load game scene; meaning load it ontop of the lobby scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainGame", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            Debug.Log("Loading scene...");
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainGame")); //Make the game scene the main scene
        SceneManager.MoveGameObjectToScene(TurnManager.instance.gameObject, SceneManager.GetSceneByName("MainGame"));

        localPlayer.Init();
    }

    #endregion

    #region END_GAME

    [TargetRpc]
    public void TargetDeclareWinner(string winnerName)
    {
        UIGame.instance.DeclareWinnerSuccess(winnerName, isHost);
        inputManager.getSelection = false;
    }

    public void ContinueGame()
    {
        CmdContinueGame();
    }

    [Command]
    public void CmdContinueGame()
    {
        TurnManager.instance.ContinueGame();
        Debug.Log("Telling tm to continue...");
    }

    public void UnloadGame()
    {
        TargetUnloadGame();
    }

    [TargetRpc]
    public void TargetUnloadGame()
    {
        foreach (Card card in cardObjs)
            Destroy(card.gameObject);

        cardObjs.Clear();
        cardIDs.Clear();

        StartCoroutine(AsyncUnloadGame());
    }

    IEnumerator AsyncUnloadGame()
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync("MainGame");

        while (!asyncUnload.isDone)
        {
            Debug.Log("Unloading scene...");
            yield return null;
        }
    }

    public void LeaveGame()
    {
        networkMatchChecker.matchId = System.Guid.Empty;
        matchID = string.Empty;

        TargetLeaveGame();
    }

    [TargetRpc]
    void TargetLeaveGame()
    {
        StartCoroutine(AsyncUnloadGame());
    }

    public void OtherPlayerLeft(Player player)
    {
        TargetOtherPlayerLeft(player.gameObject);
    }

    [TargetRpc]
    void TargetOtherPlayerLeft(GameObject playerObject)
    {
        Player player = playerObject.GetComponent<Player>();

        UILobby.instance.OtherPlayerLeft(player);

        if (UIGame.instance)
            UIGame.instance.OtherPlayerLeft(player);
    }

    #endregion
}