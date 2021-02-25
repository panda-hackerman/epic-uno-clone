using System;
using UnityEngine;
using Mirror;
using Lobby;

/* This script handles interfacing with the turnmanager and other networking stuff
 * since for some reason mirror gives an authority error if I call it on the turnmanager
 * directly. 
 */

public class NetworkingInterface : NetworkBehaviour
{
    public void NextTurn() => CmdNextTurn();
    public void Skip() => CmdSkip();
    public void Plus(int count) => CmdPlus(count);
    public void SetColor(CardType color) => CmdSetColor(color);
    public void Reverse() => CmdReverse();
    public void LeaveMatch(Player player, GameObject turnmanager = null) => CmdLeaveMatch(player.gameObject, turnmanager);

    [Command]
    private void CmdNextTurn()
    {
        TurnManager.instance.NextTurn();
    }

    [TargetRpc]
    public void TargetSetTurnDisplay(int player)
    {
        UIGame.instance.SetPlayerTurn(player);
    }

    [Command]
    private void CmdSkip()
    {
        TurnManager.instance.Skip();
    }

    [Command]
    private void CmdPlus(int count)
    {
        TurnManager.instance.Plus(count);
    }

    [Command]
    private void CmdSetColor(CardType color) //From wild cards
    {
        TurnManager.instance.SetGameColor(color);
    }

    [TargetRpc]
    public void TargetSetColor(CardType color)
    {
        if (TurnManager.instance.discard[0].TryGetComponent(out WildCard card))
            card.SetColor(color);
        else
            Debug.LogWarning("Top discard card isn't a wild card, somehow");
    }

    [Command]
    private void CmdReverse()
    {
        TurnManager.instance.isReversed = !TurnManager.instance.isReversed; //Switch

        foreach (GameObject gameobj in TurnManager.instance.players)
        {
            NetworkingInterface nwI = gameobj.GetComponent<NetworkingInterface>();

            nwI.TargetSetReverse(TurnManager.instance.isReversed);
        }
    }

    [TargetRpc]
    private void TargetSetReverse(bool value) //Reverses or unreverses the game
    {
        UIGame.instance.arrow.SetReverse(value);
    }

    [TargetRpc]
    public void TargetSetCardCount(GameObject player, int count)
    {
        UIGame.instance.SetCardCount(player.GetComponent<Player>(), count);
    }

    [TargetRpc]
    public void TargetSetHost()
    {
        UIGame.instance.SetPlayerHost();
    }

    [Command]
    private void CmdLeaveMatch(GameObject player, GameObject turnmanager)
    {
        if (turnmanager == null)
            MatchMaker.instance.LeaveMatch(player);
        else
            MatchMaker.instance.LeaveMatch(player, turnmanager.GetComponent<TurnManager>());
    }
}