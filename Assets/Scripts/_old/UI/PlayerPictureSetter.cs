using UnityEngine;
using UnityEngine.UI;

public class PlayerPictureSetter : MonoBehaviour
{
    private Image currentImage;
    public static Sprite PlayerDisplayImage { get; private set; }

    private void Start() => currentImage = GetComponent<Image>();

    // call from a UI button
    public void SetPlayerPicture(Image image)
    {
        // sets the players image
        currentImage.sprite = image.sprite;
        // sets the player's image sprite to the sprite in the current picture
        PlayerDisplayImage = currentImage.sprite;
    }
}
