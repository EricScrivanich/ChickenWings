using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidShare : MonoBehaviour, INativeShare
{
    private static AndroidJavaObject _share;


    private void Start()
    {
        SetUpShare();
    }

    private static void SetUpShare()
    {
        if (_share == null)
        {
            _share = new AndroidJavaObject("com.SmileSoft.unityplugin.Share.ShareFragment");
            _share.Call("SetUp", SunShineNativeShare.fileProviderName, "NativeShare", "ShareCallback");
        }

    }


    public void ShareMultipleFileOfSameType(string[] path, string fileType, string message, string shareDialogTitle)
    {
        SetUpShare();
        _share.Call("ShareMultipleFileOfSameFileType", path, fileType, message, shareDialogTitle);
    }

    public void ShareSingleFile(string path, string fileType, string message, string shareDialogTitle)
    {
        SetUpShare();
        _share.Call("ShareSingleFile", path, fileType, message, shareDialogTitle);
    }

    public void ShareText(string message, string shareDialogTitle)
    {
        SetUpShare();
        _share.Call("ShareText", message, shareDialogTitle);
    }

	public void ShareMultipleFileOfMultipleType(string[] path, string message, string shareDialogTitle)
	{
		ShareMultipleFileOfSameType(path, SunShineNativeShare.TYPE_FILE, message, shareDialogTitle);
	}

    
}
