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
    //Put cool image you want to change in the inspector
    public Toggle toggle; //optional
    public RawImage output;
    public int resX = 70;
    public int resY = 70;

    public void OnPointerDown(PointerEventData eventData) { }

    void Start()
    {
        // Listens to button input without dragging it in
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
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
        TextureScale.Bilinear(newTexture, resX, resY); // Change resolution
        
        output.texture = newTexture.ChangeFormat(TextureFormat.RGB565); // It's ourrr image

        if (toggle) toggle.isOn = true; //Select it
    }
}

