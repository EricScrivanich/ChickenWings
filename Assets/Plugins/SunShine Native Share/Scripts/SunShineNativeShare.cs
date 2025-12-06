using UnityEngine;


public class SunShineNativeShare : MonoBehaviour
{

    public static string TYPE_VIDEO = "video/*";
    public static string TYPE_AUDIO = "audio/*";
    public static string TYPE_IMAGE = "image/*";
    public static string TYPE_FILE = "file/*";
    public static string TYPE_PDF = "application/pdf*";

    public static string fileProviderName = "com.SmileSoft.unityplugin";

    private INativeShare _nativeShare;

    public static SunShineNativeShare instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        if (IsItAndroidPlatform())
            _nativeShare = GetComponent<AndroidShare>(); //new AndroidShare();

        if (IsItIosPlatform())
            _nativeShare = GetComponent<IosShare>();

    }

    public void ShareText(string message, string shareDialogTitle)
    {
        _nativeShare.ShareText(message, shareDialogTitle);
    }

    public void ShareSingleFile(string path, string fileType, string message, string shareDialogTitle)
    {
        _nativeShare?.ShareSingleFile(path, fileType, message, shareDialogTitle);
    }

    public void ShareMultipleFileOfSameType(string[] path, string fileType, string message, string shareDialogTitle)
    {
        _nativeShare.ShareMultipleFileOfSameType(path, fileType, message, shareDialogTitle);
    }

    public void ShareMultipleFileOfMultipleType(string[] path, string message, string shareDialogTitle)
    {
        _nativeShare.ShareMultipleFileOfSameType(path, SunShineNativeShare.TYPE_FILE, message, shareDialogTitle);
    }

    // Only Work in Android .....................
    public void ShareCallback(string isSuccess)
    {
        Debug.Log("U>> from Unity Is Success " + isSuccess);

        if (isSuccess == "true")
        {
            Debug.Log("U>> from Unity --  Yes --  Success " + isSuccess);
        }
        else
        {
            Debug.Log("U>> from Unity --  Failed --  Success " + isSuccess);
        }
    }


    public static bool IsItAndroidPlatform()
    {
        bool result = false;
#if UNITY_ANDROID
        result = true;
#endif
        return result;
    }

    public static bool IsItIosPlatform()
    {
        bool result = false;
#if UNITY_IPHONE 
        result = true;
#endif
        return result;
    }

}
