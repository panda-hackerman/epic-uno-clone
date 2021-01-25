using UnityEngine;

public class Draw_Card : Card
{
    public int cardsToDraw = 2;

    public override void OnCardDrawn()
    {
        base.OnCardDrawn();
    }

    public override void OnCardPlayed()
    {
        Player.networkInterface.Plus(cardsToDraw);
        Player.networkInterface.NextTurn();
    }
}
