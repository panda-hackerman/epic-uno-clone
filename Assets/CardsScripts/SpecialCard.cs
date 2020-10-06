using UnityEngine;

public class SpecialCard : Card
{
    public Effect cardEffect;
    public enum Effect {Skip, Reverse, ColorSwitch, Draw2, Draw4}

    public override void DrawCard()
    {
        base.DrawCard();
        
    }
}
