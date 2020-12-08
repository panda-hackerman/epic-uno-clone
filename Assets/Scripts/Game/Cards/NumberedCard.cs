using UnityEngine;

public class NumberedCard : Card
{
    [Range(0, 9)]
    public int cardValue = 0;

    public override void DrawCard()
    {
        base.DrawCard();
    }

    public override void PlayCard()
    {
        base.PlayCard();
    }
}
