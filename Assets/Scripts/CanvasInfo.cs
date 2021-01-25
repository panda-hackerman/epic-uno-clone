using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasInfo : MonoBehaviour
{
    [System.Serializable]
    public class Arrow
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

        private bool reversed = false;
        public void SetReverse(bool value)
        {
            reversed = value;
            Animator.SetBool("isReversed", value);
            SpriteRenderer.flipX = value;
            Debug.Log($"Reversed is now {reversed}");
        }
    }

    public Arrow arrow = new Arrow(); //Arrow that shows the direction of the game

    public static CanvasInfo canvas; //me :)

    public Transform playerDisplay; //GameUiPlayer transform

    public GameObject chooseColorButtons; //For wild cards

    public void SetPlayerTurn(int playerNum)
    {
        foreach (Transform child in playerDisplay)
        {
            if (child.TryGetComponent(out GameUIPlayer uiPlayer))
                uiPlayer.outline.enabled = uiPlayer.player.playerID == playerNum;
        }
    }

    public void ChooseColor()
    {
        chooseColorButtons.SetActive(true);
        Player.localPlayer.inputManager.getSelection = false;
    }

    private void Awake()
    {
        canvas = this;
        SetPlayerTurn(0);
    }
}
