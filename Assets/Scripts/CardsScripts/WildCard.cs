using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WildCard : Card
{
    public override void DrawCard()
    {
        base.DrawCard();
    }

    public override void PlayCard()
    {
        base.PlayCard();
    }

    [Command]
    private void CmdChangeColor()
    {
        //TODO: Tell server to change the color
    }
}
