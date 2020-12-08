using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Draw_Card : WildCard
{
    public int cardsToDraw = 4;

    public override void DrawCard()
    {
        base.DrawCard();
    }

    public override void PlayCard()
    {
        if (wildCard)
            base.PlayCard();
    }
}
