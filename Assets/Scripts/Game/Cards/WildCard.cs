using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WildCard : Card
{
    public Sprite yellowSprite, redSprite, blueSprite, greenSprite;

    public override void DrawCard()
    {
        base.DrawCard();
    }

    public override void PlayCard()
    {
        base.PlayCard();

        //myPlayer.chooseColorButtons.SetActive(true);
    }
}
