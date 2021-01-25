using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SkipCard : Card
{
    // only cool things happen in this script ;)
    public override void OnCardDrawn()
    {
        base.OnCardDrawn();
    }

    public override void OnCardPlayed()
    {
        Player.networkInterface.Skip();
    }

}
