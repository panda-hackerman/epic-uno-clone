using UnityEngine;
using UnityEngine.UI;

public class TextureScaler : MonoBehaviour
{
    public Texture2D image_Texture; //Put a cool image here in the inspector

    void Start()
    {
        Texture2D newTexture = Instantiate(image_Texture);
        GetComponent<RawImage>().texture = newTexture;
        TextureScale.Bilinear(newTexture, 70, 70);
    }
}
