using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WriteTest : MonoBehaviour
{
    void Start()
    {
        string documentsPath = FileUtils.GetDocumentsPath();
        if (string.IsNullOrEmpty(documentsPath))
        {
            return;
        }

        // Documents フォルダ内に "test" フォルダへのパスを生成
        string testFolderPath = Path.Combine(documentsPath, "test");

        string fileName = "test.txt";
        string filePath = Path.Combine(testFolderPath, fileName);
        string content = "これはDocumentsフォルダに保存されたテストファイルです。";

        try
        {
            // サブディレクトリ "test" が存在しない場合に作成
            Directory.CreateDirectory(testFolderPath);

            // ファイルを書き込み
            File.WriteAllText(filePath, content);
        }
        catch (System.Exception e)
        {
            Debug.LogError("ファイルの保存中にエラーが発生しました: " + e.Message);
            
        }
    }
}
