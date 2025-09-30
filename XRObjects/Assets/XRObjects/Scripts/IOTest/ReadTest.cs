using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class ReadTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI readText;
    [SerializeField] private Image readImage;

    // Start is called before the first frame update
    void Start()
    {
        // path表示
        // readText.text = FileUtils.GetDocumentsPath();

        string documentsPath = FileUtils.GetDocumentsPath();
        string testFolderPath = Path.Combine(documentsPath, "test");
        string fileName = "test.png";
        string filePath = Path.Combine(testFolderPath, fileName);


        // textファイルを読み込む
        // if (File.Exists(filePath))
        // {
        //     string content = File.ReadAllText(filePath);
        //     readText.text = content;
        // }
        // else
        // {
        //     readText.text = "ファイルが存在しません: " + filePath;
        // }


        // 画像を読み込む
        if (File.Exists(filePath))
        {
            readText.text = "success: " + filePath;
            byte[] imageData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                readImage.sprite = sprite;
                readImage.preserveAspect = true;
            }
            else
            {
                readText.text = "failure: " + filePath;
            }
        }
        else
        {
            readText.text = "not found: " + filePath;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
