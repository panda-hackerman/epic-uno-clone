using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Draw_Card : Card
{
    public int cardsToDraw = 4;

    public override void DrawCard()
    {
        base.DrawCard();
    }

    public override void PlayCard()
    {
        base.PlayCard();
    }

    [Command]
    private void CmdGiveCards(int cards)
    {
        //TODO: Give cards to the next player over
        //TODO: Stacking?
    }
}
