using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using AdvacedMathStuff;

public class UnoInputManager : NetworkBehaviour
{
    ServerGameManager sgm;
    PlayerManager playerManager;
    Camera cam;
    Card selectedCard;

    public LayerMask layerMask;

    private void Start()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();

        cam = Camera.main;

        sgm = FindObjectOfType<ServerGameManager>();
        if (!sgm) Debug.LogWarning("Couldn't find the server game manager");

    }

    private void Update()
    {
        selectedCard = null;

        if (Physics.Raycast(cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 50, layerMask))
        {
            selectedCard = hit.collider.GetComponent<Card>();
        }

        foreach (Card card in playerManager.myHand)
        {
            if (card)
                card.IsSelected = card == selectedCard;
        }
    }

    [Command]
    public void CmdAddCardToDiscard(int id)
    {
        GameObject newCard = Instantiate(playerManager.cardPrefabs[id]);
        Destroy(newCard.GetComponent<Collider>());

        NetworkServer.Spawn(newCard);

        newCard.transform.position = new Vector3(0f.GiveOrTake(0.1f), 0.01f, 0f.GiveOrTake(0.1f));
        newCard.transform.eulerAngles = new Vector3(90, 0, Random.Range(0f, 360f));
        sgm.discardPile.Add(newCard);
        sgm.UpdateDiscardPile();
    }

    public void OnSelect()
    {
        if (!selectedCard || !hasAuthority) return;
        if (!playerManager.myHand.Contains(selectedCard))
        {
            Debug.LogWarning("Player attempting to select card not in their hand");
            return;
        }

        selectedCard.PlayCard(); //You've been played! B)

        //Get out of my hand and into the discard pile
        playerManager.myHand.Remove(selectedCard);
        CmdAddCardToDiscard(selectedCard.ID);

        Destroy(selectedCard.gameObject); //Death
        selectedCard = null; //Object deleted so set var to null

        playerManager.UpdateCardPlacement(); //Make sure the other cards are where they need to be
    }

}
