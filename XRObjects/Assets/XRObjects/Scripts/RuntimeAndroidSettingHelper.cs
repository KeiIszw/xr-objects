
using System.Collections.Generic;
using UnityEngine;
using System;
public class RuntimeAndroidSettingHelper
{
    private RuntimeAndroidSettingHelper() { }
    private static AndroidJavaObject GetActivity()
    {
        using (var UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            return UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }
    public static void Request_SettingsIntent()
    {
        using (var activity = GetActivity())
        {
            using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.MANAGE_ALL_FILES_ACCESS_PERMISSION"))
            {
                activity.Call("startActivity", intentObject);
            }
        }
    }
    //https://developer.android.com/reference/android/os/Environment#isExternalStorageManager()
    public static bool HasUserAuthorizedPermission()
    {
        var isExternalStorageManager = false;
        try
        {
            // Androidの外部ストレージマネージャのパーミッションチェック
            AndroidJavaClass environmentClass = new AndroidJavaClass("android.os.Environment");
            isExternalStorageManager = environmentClass.CallStatic<bool>("isExternalStorageManager");
        }
        catch (Exception e)
        {
            return false;
        }

        return isExternalStorageManager;
    }
}