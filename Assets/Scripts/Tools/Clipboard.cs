using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Clipboard
{
    public static void CopyToClipboard(this string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }
}
