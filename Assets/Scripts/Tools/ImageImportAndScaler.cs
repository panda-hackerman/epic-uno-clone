using System.Text;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Convert;
using SFB;

public class ImageImportAndScaler : MonoBehaviour, IPointerDownHandler
{
    // Put cool image you want to change in the inspector
    public Toggle toggle; // optional
    public RawImage output;
    public int resX = 70;
    public int resY = 70;

    public void OnPointerDown(PointerEventData eventData) { }

    void Start()
    {
        // Listens to button input without dragging it in
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        // Checks for the last saved image
        Texture2D setupTexture = ReadTextureFromPlayerPrefs("LastImage");
        if (setupTexture != null)
        {
            SetImageOutput(setupTexture);
        }
        else Debug.Log("No image found");
    }

    public void OnClick()
    {
        // File search for compatible images
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "png", false);
        // Not 100% sure what this does but I think its prevent selecting multiple images
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }

    private IEnumerator OutputRoutine(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url); // Gimmeh that image
        yield return www.SendWebRequest();
        
        Texture2D newTexture = DownloadHandlerTexture.GetContent(www); // I am the image
        SetImageOutput(newTexture); // Ourrr image
        
        // Convert texture to bytes for a PNG
        byte[] imageBytes = newTexture.EncodeToPNG();
        // Turn bytes into a string
        string newTextureBytes = System.Convert.ToBase64String(imageBytes);
        // Saves the string in PlayerPrefs
        PlayerPrefs.SetString("LastImage", newTextureBytes);

        if (toggle) toggle.isOn = true; //Select it
    }

    public static Texture2D ReadTextureFromPlayerPrefs(string tag)
    {
        // Load string from playerpref
        string imageBytes = PlayerPrefs.GetString(tag, null);
        // Checks if the string isn't empty
        if (!string.IsNullOrEmpty(imageBytes))
        {
            Debug.Log("Last Image Found!");

            // Convert back into byte array
            byte[] TextureBytes = System.Convert.FromBase64String(imageBytes);
            // Scaling
            Texture2D tex = new Texture2D(2, 2);

            // If all goes well than load the texture and return
            if (tex.LoadImage(TextureBytes)) return tex;
        }
        return null;
    }
    public void SetImageOutput(Texture2D TextureToOutput)
    {
        TextureScale.Bilinear(TextureToOutput, resX, resY); // Change resolution
        output.texture = TextureToOutput.ChangeFormat(TextureFormat.RGB565); // Change format and set output
    }
}

