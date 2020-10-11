using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ReverseCard : Card
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
    private void CmdReverse()
    {
        //TODO: Tell the server to reverse :)
    }
}
