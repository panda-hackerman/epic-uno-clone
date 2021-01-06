using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Lobby;

public class InputManager : MonoBehaviour
{
    public bool getSelection = true; //TODO: No need to get the player's selected card if it isn't their turn

    private GameObject selection;
    private Card selectedCard
    {
        get
        {
            if (!selection) return null;
            if (selection.TryGetComponent(out Card card))
                return card;
            else return null;
        }
    }

    private Player player;
    private Camera cam;

    private void Start()
    {
        player = GetComponent<Player>();
        getSelection = false;
    }

    public void OnInit() //Runs when the match starts, called by player
    {
        cam = GameObject.FindGameObjectWithTag("GameCamera").GetComponent<Camera>();
        getSelection = true;
    }

    private void Update()
    {
        if (getSelection)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(mousePos); //TODO: Cache camera

            if (Physics.Raycast(ray, out RaycastHit hit))
                selection = hit.transform.gameObject;
            else
                selection = null;
        }
        else
        {
            selection = null;
        }

        foreach (Card card in player.cardObjs)
        {
            card.isSelected = card == selectedCard;
        }
    }
}
