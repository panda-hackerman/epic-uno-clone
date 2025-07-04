﻿using UnityEngine;

public class NumberedCard : Card
{
    [Range(0, 9)]
    public int cardValue = 0;

    public override void OnCardDrawn()
    {
        base.OnCardDrawn();
    }

    public override void OnCardPlayed()
    {
        base.OnCardPlayed();
    }
}
