using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SkipCard : Card
{
    // only cool things happen in this script ;)
    public override void DrawCard()
    {
        base.DrawCard();
    }

    public override void PlayCard()
    {
        base.PlayCard();
    }

    [Command]
    void CmdSkipPlayer()
    {
        //TODO: Skip, maybe just call NextTurn() twice?
    }
}
