using UnityEngine;

public class WildCard : Card
{
    public Sprite yellowSprite, redSprite, blueSprite, greenSprite;

    public int cardsToDraw = 4;

    public override void OnCardDrawn()
    {
        base.OnCardDrawn();
    }

    public override void OnCardPlayed()
    {
        UIGame.instance.chooseColorButtons.SetActive(true);

        Player.networkInterface.Plus(cardsToDraw);
    }

    public void SetColor(CardType color)
    {
        type = color;

        switch (color)
        {
            case CardType.yellow:
                spriteRenderer.sprite = yellowSprite;
                break;
            case CardType.red:
                spriteRenderer.sprite = redSprite;
                break;
            case CardType.blue:
                spriteRenderer.sprite = blueSprite;
                break;
            case CardType.green:
                spriteRenderer.sprite = greenSprite;
                break;
            default:
                Debug.LogWarning("Invalid color");
                break;
        }
    }
}
