using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using AdvacedMathStuff;

public class UnoInputManager : NetworkBehaviour
{
    public GameObject testCardPrefab;
    PlayerManager playerManager;
    Camera cam;
    Card selectedCard;

    public LayerMask layerMask;

    private void Start()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();

        cam = Camera.main;
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
            card.IsSelected = card == selectedCard;
        }
    }

    [Command]
    public void CmdAddCardToDeck()
    {
        GameObject newCard = Instantiate(testCardPrefab);
        NetworkServer.Spawn(newCard);

        newCard.transform.position = Vector3.zero.Y(0.01f);
        newCard.transform.eulerAngles = new Vector3(90, 0, Random.Range(0, 360));
        newCard.GetComponent<SpriteRenderer>().sortingOrder = -1;
        Destroy(newCard.GetComponent<Collider>());
    }

    public void OnSelect()
    {
        if (!selectedCard || !hasAuthority) return;
        if (!playerManager.myHand.Contains(selectedCard))
        {
            Debug.LogWarning("Player attempting to select card not in their hand");
            return;
        }

        selectedCard.PlayCard();

        CmdAddCardToDeck();

        playerManager.myHand.Remove(selectedCard);

        Destroy(selectedCard.gameObject);
        selectedCard = null;

        playerManager.UpdateCardPlacement();
    }

}
