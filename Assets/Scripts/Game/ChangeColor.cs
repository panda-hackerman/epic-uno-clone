using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    private PlayerManager player;
    private UnoInputManager inputManager;

    private void Start()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        player = networkIdentity.GetComponent<PlayerManager>();
        inputManager = networkIdentity.GetComponent<UnoInputManager>();

        if (!player) Debug.LogWarning("No player found");
    }

    public void ChooseColor(int color)
    {
        if (!player) return;

        player.CmdPickColor(color);
        player.chooseColorButtons.SetActive(false);

        inputManager.CmdNextTurn();
    }

}
