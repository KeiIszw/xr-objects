using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.IO;

// TidyUpCursorにアタッチ
public class DialogUI : MonoBehaviour
{
    public Button buttonCloseDialog;
    public Button buttonShowMenu;
    public Button buttonCapture;
    public Button buttonTidyUpList;
    public GameObject tidyUpList;
    // public GameObject tidyUpListItemPrefab; // prefab for the tidy up list items
    public GameObject dialog;
    public GameObject menu;
    public GameObject outputText;
    public Button buttonBeforeImg;
    public Button buttonAfterImg;
    public Button buttonAnalysisText;
    public Button buttonSuggestionsText;
    // [HideInInspector] public GameObject tidyUpListContent; // content of the tidy up list
    [HideInInspector] public bool showTidyUpList = false;
    [HideInInspector] public GameObject activeHistoryContent; // content of the tidy up list
    // private List<GameObject> tidyUpListItems = new List<GameObject>();
    
    
    

    // Start is called before the first frame update
    void Start()
    {
        // tidyUpListContent = tidyUpList.transform.Find("Viewport/Content").gameObject;

        // Button buttonCaptureButton = buttonCapture.GetComponent<Button>();
        // buttonCaptureButton.onClick.AddListener(buttonCaptureCallback);

        Button buttonTidyUpListButton = buttonTidyUpList.GetComponent<Button>();
        buttonTidyUpListButton.onClick.AddListener(buttonTidyUpListCallback);

        // hide the tidy up list at the start
        tidyUpList.SetActive(false);


        Button buttonCloseDialogButton = buttonCloseDialog.GetComponent<Button>();
        buttonCloseDialogButton.onClick.AddListener(buttonCloseDialogCallback);

        // Button buttonShowDialogButton = buttonShowDialog.GetComponent<Button>();
        // buttonShowDialogButton.onClick.AddListener(buttonShowDialogCallback);

        Button buttonShowMenuButton = buttonShowMenu.GetComponent<Button>();
        buttonShowMenuButton.onClick.AddListener(buttonShowMenuCallback);

        // hide the dialog at the start
        dialog.SetActive(false);
        // hide the menu at the start
        menu.SetActive(false);
        // hide the outputText at the start
        outputText.SetActive(false);

        Button buttonBeforeImgButton = buttonBeforeImg.GetComponent<Button>();
        buttonBeforeImgButton.onClick.AddListener(buttonBeforeImgCallback);

        Button buttonAfterImgButton = buttonAfterImg.GetComponent<Button>();
        buttonAfterImgButton.onClick.AddListener(buttonAfterImgCallback);

        Button buttonAnalysisTextButton = buttonAnalysisText.GetComponent<Button>();
        buttonAnalysisTextButton.onClick.AddListener(buttonAnalysisTextCallback);

        Button buttonSuggestionsTextButton = buttonSuggestionsText.GetComponent<Button>();
        buttonSuggestionsTextButton.onClick.AddListener(buttonSuggestionsTextCallback);

        LanguageManager.Instance.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    void UpdateText()
    {
        if (LanguageManager.Instance == null) return;
        buttonBeforeImg.GetComponentInChildren<TextMeshProUGUI>().text = LanguageManager.Instance.GetText("before");
        buttonAfterImg.GetComponentInChildren<TextMeshProUGUI>().text = LanguageManager.Instance.GetText("after");
        buttonAnalysisText.GetComponentInChildren<TextMeshProUGUI>().text = LanguageManager.Instance.GetText("analysis");
        buttonSuggestionsText.GetComponentInChildren<TextMeshProUGUI>().text = LanguageManager.Instance.GetText("suggestions");

        // // JSON文字列をDictionary<string, object>に変換 (値の型が混合している場合)
        // // object型を使用すると、JSONの様々な型の値を柔軟に受け取れます。
        // Dictionary<string, object> myDictionaryObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(testJson);

        // Debug.Log("--- Newtonsoft.JsonでDictionary<string, object>へ変換 ---");
        // foreach (KeyValuePair<string, object> pair in myDictionaryObject)
        // {
        //     Debug.Log($"Key: {pair.Key}, Value: {pair.Value} (Type: {pair.Value.GetType().Name})");
        // }
    }

    // Update is called once per frame
    void Update()
    {
        // ダイアログが表示されているときは、キャプチャボタンと tidyUpList を非表示にする
        if (dialog.activeSelf)
        {
            buttonCapture.gameObject.SetActive(false);
            showTidyUpList = false; // tidyUpListの表示状態をリセット
            tidyUpList.SetActive(false);
        }
        else
        {
            buttonCapture.gameObject.SetActive(true);
            tidyUpList.SetActive(showTidyUpList);
        }
        UpdateText();
    }

    void buttonTidyUpListCallback()
    {
        // toggle the visibility of the tidy up list
        showTidyUpList = !showTidyUpList;

        if (showTidyUpList)
        {
            tidyUpList.SetActive(true);
        }
        else
        {
            tidyUpList.SetActive(false);
        }
    }

    // void buttonCaptureCallback() // いらないかも
    // {
    //     GameObject newItem = Instantiate(tidyUpListItemPrefab, tidyUpListContent.transform);
    //     // 画像とテキストが入ったダイアログを生成する．
    //     // 画像とテキストをリストに格納しとく
    //     // リストのアイテム名は撮影時間にする

    //     tidyUpListItems.Add(newItem);
    //     if (tidyUpListItems.Count > 0)
    //     {
    //         tidyUpListItems[tidyUpListItems.Count - 1].GetComponent<Button>().onClick.AddListener(buttonItemCallback);
    //     }
    // }

    // void buttonItemCallback()
    // {
    //     buttonCapture.gameObject.SetActive(false);//hide the capture button
    //     dialog.SetActive(true); //どのダイアログを表示するか選択する
    //     tidyUpList.SetActive(false); //hide the tidy up list

    //     // JSON文字列をDictionary<string, object>に変換 (値の型が混合している場合)
    //     // object型を使用すると、JSONの様々な型の値を柔軟に受け取れます。
    //     Dictionary<string, object> myDictionaryObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(testJson);

    //     Debug.Log("--- Newtonsoft.JsonでDictionary<string, object>へ変換 ---");
    //     foreach (KeyValuePair<string, object> pair in myDictionaryObject)
    //     {
    //         Debug.Log($"Key: {pair.Key}, Value: {pair.Value} (Type: {pair.Value.GetType().Name})");
    //     }
    // }

    void buttonCloseDialogCallback()
    {
        // hide the dialog
        dialog.SetActive(false);
        // hide the menu
        menu.SetActive(false);
        // hide the outputText
        outputText.SetActive(false);
        // show the capture button
        buttonCapture.gameObject.SetActive(true);
        // reset the dialog image to default
        dialog.GetComponent<Image>().sprite = null;

        // ファイルを閉じる
    }
    // void buttonShowDialogCallback()
    // {
    //     // show the dialog
    //     dialog.SetActive(true);
    // }
    void buttonShowMenuCallback()
    {
        // toggle the menu visibility
        if (menu.activeSelf)
        {
            menu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
        }

        // hide the outputText
        outputText.SetActive(false);
    }

    void buttonBeforeImgCallback()
    {
        menu.SetActive(false); // hide the menu
        outputText.SetActive(false); // hide the outputText

        // Show the before image in the dialog
        // ここでbeforeImgの表示処理を実装
        Debug.Log("Before image button clicked");

        // 写真を表示
        string documentsPath = FileUtils.GetDocumentsPath();
        string testFolderPath = Path.Combine(documentsPath, "test/2025-09-27");
        string fileName = "before_23-28-11.png";
        string filePath = Path.Combine(testFolderPath, fileName);
        if (File.Exists(filePath))
        {
            byte[] imageData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                dialog.GetComponent<Image>().sprite = sprite;
                dialog.GetComponent<Image>().preserveAspect = true;
            }
            else
            {
                Debug.Log("failure: " + filePath);
            }
        }
        else
        {
            Debug.Log("not found: " + filePath);
        }

    }

    void buttonAfterImgCallback()
    {
        menu.SetActive(false); // hide the menu
        outputText.SetActive(false); // hide the outputText

        // Show the after image in the dialog
        // ここでafterImgの表示処理を実装
        Debug.Log("After image button clicked");

        // 写真を表示
        string documentsPath = FileUtils.GetDocumentsPath();
        string testFolderPath = Path.Combine(documentsPath, "test/2025-09-27");
        string fileName = "after_23-28-11.png";
        string filePath = Path.Combine(testFolderPath, fileName);
        if (File.Exists(filePath))
        {
            byte[] imageData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                dialog.GetComponent<Image>().sprite = sprite;
                dialog.GetComponent<Image>().preserveAspect = true;
            }
            else
            {
                Debug.Log("failure: " + filePath);
            }
        }
        else
        {
            Debug.Log("not found: " + filePath);
        }

    }

    void buttonAnalysisTextCallback()
    {
        // reset the dialog image to default
        dialog.GetComponent<Image>().sprite = null;
        menu.SetActive(false); // hide the menu
        outputText.SetActive(true); // show the outputText


        // Show the analysis outputText in the dialog
        // ここでanalysisTextの表示処理を実装
        Debug.Log("Analysis outputText button clicked");

        // テキストを表示
        // outputText.GetComponent<TextMeshProUGUI>().text = activeHistoryContent.GetComponent<SetupTidyupProxy>().analysisText;
        outputText.GetComponent<TextMeshProUGUI>().text = "カテゴリ: 散乱物・放置物・配線、整理不足\n" + "詳細: モニター左下にスマートフォンが置かれ、その手前には付箋が貼られたボードや名刺入れらしき物が見える。左手前には飲みかけのペットボトル、使い終わったらしき紙タオルや布、未開封の箱などが無造作に置かれている。マイクの周囲にも複数のケーブルが絡まり、整理されていない。\n" + "場所: デスク上（中央から左側）";
        
    }

    void buttonSuggestionsTextCallback()
    {
        // reset the dialog image to default
        dialog.GetComponent<Image>().sprite = null;
        menu.SetActive(false); // hide the menu
        outputText.SetActive(true); // show the outputText

        // Show the suggestions outputText in the dialog
        // ここでsuggestionsTextの表示処理を実装
        Debug.Log("Suggestions outputText button clicked");

        // テキストを表示
        // outputText.GetComponent<TextMeshProUGUI>().text = activeHistoryContent.GetComponent<SetupTidyupProxy>().suggestionsText;
        outputText.GetComponent<TextMeshProUGUI>().text = "優先度: 高\n" + "提案: デスク上の不要な物（飲み物の空き容器、使い終わった紙類、布など）をまず片付け、ごみは捨てる。使用頻度の低い小物や書類は適切な収納場所に移動させる。\n" + "対象エリア: デスク上全体\n";

    }
    
}
