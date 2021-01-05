using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Draw_Card : WildCard
{
    public int cardsToDraw = 4;

    public override void OnCardDrawn()
    {
        base.OnCardDrawn();
    }

    public override void OnCardPlayed()
    {
        if (wildCard)
            base.OnCardPlayed();
    }
}
