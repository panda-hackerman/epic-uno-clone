using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;

[Serializable]
public class ImageData
{
    public ImageData() { }

    public ImageData(byte[] byteArray, int width, int height, TextureFormat format = TextureFormat.ARGB32)
    {
        this.byteArray = byteArray;
        this.width = width;
        this.height = height;
        this.format = format;
    }

    public byte[] byteArray; //<= 16,372 bytes
    public int width; //+4 bytes
    public int height; //+4 bytes
    public TextureFormat format; //+4 bytes (i think)
}

namespace Convert
{
    /* If you need a custom conversion, this is where the magic happens.
     * Most of the methods are extensions for ease of use (just slap it
     * at the end of your reference as a method).
     * For example, to use ToGuid, you could do the follwing
     * string input = "ABC123"
     * Guid output = input.ToGuid();
     */
    public static class Converter
    {
        //Converts a string to a Guid (code like 0f8fad5b-d9cb-469f-a165-70867728950e)
        public static Guid ToGuid(this string id)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.Default.GetBytes(id);
            byte[] hashBytes = provider.ComputeHash(inputBytes);

            return new Guid(hashBytes);
        }

        public static ImageData ToPixels(this Texture2D image) //Converts a texture into an array of bytes (along with some other useful info)
        {
            byte[] data = image.GetRawTextureData();
            int width = image.width;
            int height = image.height;

            Debug.Log($"Converted {image.format} image {image.name} ({width} x {height}) to byte array | {data.Length}B");
            if (data.Length > 16372) Debug.LogWarning($"Image probably too big. {data.Length} > 16372");

            return new ImageData(data, width, height, image.format);
        }

        public static Texture2D ToTexture(this ImageData input) //Converts and array of bytes into a texture
        {
            var texture = new Texture2D(input.width, input.height, input.format, false);

            texture.LoadRawTextureData(input.byteArray);

            texture.Apply();

            return texture;
        }

        public static Texture2D ChangeFormat(this Texture2D InputTexture, TextureFormat NewFormat) //Converts texture into a format
        {
            Texture2D newTexture = new Texture2D(InputTexture.width, InputTexture.height, NewFormat, false);

            newTexture.SetPixels(InputTexture.GetPixels());
            newTexture.Apply();

            return newTexture;
        }
    }
}
