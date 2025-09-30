using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileUtils
{

    /// <summary>
    /// 書き込み可能なディレクトリのパスを返す
    /// ファイルの保存はこのディレクトリの直下ではなく、サブディレクトリを作成して保存する事を推奨します
    /// </summary>
    /// <returns>プラットフォームごとの書き込み可能なディレクトリのパス</returns>
    public static string GetDocumentsPath()
    {
        // Androidプラットフォームでのみ実行
        #if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var environment = new AndroidJavaClass("android.os.Environment"))
            {
                // Environment.DIRECTORY_DOCUMENTS のパスを取得
                using (var dirDocuments = environment.GetStatic<AndroidJavaObject>("DIRECTORY_DOCUMENTS"))
                {
                    using (var file = environment.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", dirDocuments))
                    {
                        return file.Call<string>("getAbsolutePath");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to get Documents path: " + e.Message);
            return null;
        }
        #else
        Debug.Log("This is not an Android build.");
        return Application.persistentDataPath; // Fallback for other platforms
        #endif
    }

}