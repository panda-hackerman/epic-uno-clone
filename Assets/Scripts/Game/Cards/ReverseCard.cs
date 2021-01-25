using UnityEngine;

public class ReverseCard : Card
{
    public override void OnCardDrawn()
    {
        base.OnCardDrawn();
    }

    public override void OnCardPlayed()
    {
        Player.networkInterface.Reverse();
        Player.networkInterface.NextTurn();
    }
}
