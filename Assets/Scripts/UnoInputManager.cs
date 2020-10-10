using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using AdvacedMathStuff;

public class UnoInputManager : NetworkBehaviour
{
    PlayerManager playerManager;
    Camera cam;
    Card selectedCard;

    public LayerMask layerMask;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        cam = Camera.main;
    }

    private void Update()
    {
        if (Physics.Raycast(cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 50, layerMask))
        {
            GameObject cardObj = hit.collider.gameObject;
            if (cardObj.GetComponent<NetworkIdentity>().hasAuthority)
            {
                selectedCard = cardObj.GetComponent<Card>();
            }
        }
        else
        {
            selectedCard = null;
        }

        foreach (Card card in playerManager.myHand)
        {
            card.transform.position = card == selectedCard ? card.selectedPos : card.restingPos;
        }
    }

    public void OnSelect()
    {
        if (!selectedCard) return;
        if (!playerManager.myHand.Contains(selectedCard))
        {
            Debug.LogError("Player attempting to select card not in their hand");
            return;
        }

        //TODO: Could instead destroy cardObj and create new card on top of deck
        selectedCard.gameObject.transform.position = Vector3.zero.Y(0.01f);
        selectedCard.transform.eulerAngles = new Vector3(90, 0, Random.Range(0, 360));
        selectedCard.GetComponent<SpriteRenderer>().sortingOrder = -1;
        selectedCard.GetComponent<Collider>().enabled = false;

        selectedCard.PlayCard();

        playerManager.myHand.Remove(selectedCard);
        playerManager.UpdateCardPlacement();
    }
}
