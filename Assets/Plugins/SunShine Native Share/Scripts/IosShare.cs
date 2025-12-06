using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class IosShare : MonoBehaviour, INativeShare
{

    #if UNITY_IPHONE && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern int _shareText(string message);

    [DllImport("__Internal")]
    private static extern int _shareFile(string path, string message);

    #endif
  
    public void ShareSingleFile(string path, string fileType, string message, string shareDialogTitle)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
_shareFile(path, message);
#endif

    }

    public void ShareText(string message, string shareDialogTitle)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
_shareText(message);
#endif

    }

    public void ShareMultipleFileOfSameType(string[] path, string fileType, string message, string shareDialogTitle)
	{
#if UNITY_IPHONE && !UNITY_EDITOR
 _shareFile(GenerateBigString(path), message);
#endif

    }

    public void ShareMultipleFileOfMultipleType(string[] path, string message, string shareDialogTitle)
	{
		ShareMultipleFileOfSameType(path, SunShineNativeShare.TYPE_FILE, message, shareDialogTitle);
	}



	public string GenerateBigString(string[] pathList)
    {
        string bigString = "";
        string separator = "<smile123>";
        for (int i = 0; i < pathList.Length; i++)
        {
            if (i == pathList.Length - 1)
            {
                bigString = bigString + pathList[i];
            }
            else
            {
                bigString = bigString + pathList[i] + separator;
            }

        }

        Debug.Log("UNITY>> in unity Try to share screeShot  p " + bigString);
        return bigString;
    }

	
}
