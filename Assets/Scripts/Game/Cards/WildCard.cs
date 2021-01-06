using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WildCard : Card
{
    public Sprite yellowSprite, redSprite, blueSprite, greenSprite;

    public override void OnCardDrawn()
    {
        base.OnCardDrawn();
    }

    public override void OnCardPlayed()
    {
        base.OnCardPlayed();

        //myPlayer.chooseColorButtons.SetActive(true);
    }
}
