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
    public GameObject scoreText;
    public GameObject outputText;
    public GameObject outputImage;
    public Button buttonBeforeImg;
    public Button buttonAfterImg;
    public Button buttonAnalysisText;
    // public Button buttonSuggestionsText;
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
        // hide the scoreText at the start
        scoreText.SetActive(false);
        // hide the outputText at the start
        outputText.SetActive(false);
        // hide the outputImage at the start
        outputImage.SetActive(false);

        Button buttonBeforeImgButton = buttonBeforeImg.GetComponent<Button>();
        buttonBeforeImgButton.onClick.AddListener(buttonBeforeImgCallback);

        Button buttonAfterImgButton = buttonAfterImg.GetComponent<Button>();
        buttonAfterImgButton.onClick.AddListener(buttonAfterImgCallback);

        Button buttonAnalysisTextButton = buttonAnalysisText.GetComponent<Button>();
        buttonAnalysisTextButton.onClick.AddListener(buttonAnalysisTextCallback);

        // Button buttonSuggestionsTextButton = buttonSuggestionsText.GetComponent<Button>();
        // buttonSuggestionsTextButton.onClick.AddListener(buttonSuggestionsTextCallback);

        LanguageManager.Instance.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    void UpdateText()
    {
        if (LanguageManager.Instance == null) return;
        buttonBeforeImg.GetComponentInChildren<TextMeshProUGUI>().text = LanguageManager.Instance.GetText("before");
        buttonAfterImg.GetComponentInChildren<TextMeshProUGUI>().text = LanguageManager.Instance.GetText("after");
        buttonAnalysisText.GetComponentInChildren<TextMeshProUGUI>().text = LanguageManager.Instance.GetText("analysis");
        // buttonSuggestionsText.GetComponentInChildren<TextMeshProUGUI>().text = LanguageManager.Instance.GetText("suggestions");
        buttonTidyUpList.GetComponentInChildren<TextMeshProUGUI>().text = LanguageManager.Instance.GetText("history");
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
        // hide the scoreText
        scoreText.SetActive(false);
        // hide the outputText
        outputText.SetActive(false);
        // hide the outputImage
        outputImage.SetActive(false);
        // show the capture button
        buttonCapture.gameObject.SetActive(true);
        // reset the dialog image to default
        dialog.GetComponent<Image>().sprite = null;

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
        // outputText.SetActive(false);
    }

    public void buttonBeforeImgCallback()
    {
        menu.SetActive(false); // hide the menu
        outputText.SetActive(false); // hide the outputText
        scoreText.SetActive(true); // show the scoreText
        outputImage.SetActive(true); // show the outputImage

        // スコアを表示
        if (activeHistoryContent.GetComponent<SetupTidyupProxy>().cleanlinessScore != -1)
        {
            scoreText.GetComponent<TextMeshProUGUI>().text = activeHistoryContent.GetComponent<SetupTidyupProxy>().cleanlinessScore.ToString() + "点";
        }
        else
        {
            scoreText.GetComponent<TextMeshProUGUI>().text = "- 点";
        }

        // 写真を表示
        string captureDateTime = activeHistoryContent.GetComponent<SetupTidyupProxy>().captureDateTime; // yyyy-MM-dd:HH-mm-ss
        string captureDate = captureDateTime.Split(':')[0]; // yyyy-MM-dd
        string captureTime = captureDateTime.Split(':')[1]; // HH-mm-ss

        string documentsPath = FileUtils.GetDocumentsPath();
        string FolderPath = Path.Combine(documentsPath, captureDate);
        string fileName = "before_" + captureTime + ".png";
        string filePath = Path.Combine(FolderPath, fileName);
        if (File.Exists(filePath))
        {
            byte[] imageData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                outputImage.GetComponent<Image>().sprite = sprite;
                outputImage.GetComponent<Image>().preserveAspect = true;
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
        scoreText.SetActive(false); // hide the scoreText
        outputImage.SetActive(true); // show the outputImage

        // 写真を表示
        string captureDateTime = activeHistoryContent.GetComponent<SetupTidyupProxy>().captureDateTime; // yyyy-MM-dd:HH-mm-ss
        string captureDate = captureDateTime.Split(':')[0]; // yyyy-MM-dd
        string captureTime = captureDateTime.Split(':')[1]; // HH-mm-ss

        string documentsPath = FileUtils.GetDocumentsPath();
        string FolderPath = Path.Combine(documentsPath, captureDate);
        string fileName = "after_" + captureTime + ".png";
        string filePath = Path.Combine(FolderPath, fileName);
        if (File.Exists(filePath))
        {
            byte[] imageData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                outputImage.GetComponent<Image>().sprite = sprite;
                outputImage.GetComponent<Image>().preserveAspect = true;
            }
            else
            {
                Debug.Log("failure: " + filePath);
            }
        }
        else
        {
            outputImage.SetActive(false); // hide the outputImage
            outputText.GetComponentInChildren<TextMeshProUGUI>().text = "Now Loading...";
            outputText.SetActive(true); // show the outputText
            Debug.Log("not found: " + filePath);
        }

    }

    void buttonAnalysisTextCallback()
    {
        // reset the dialog image to default
        dialog.GetComponent<Image>().sprite = null;
        menu.SetActive(false); // hide the menu
        outputText.SetActive(true); // show the outputText
        scoreText.SetActive(false); // hide the scoreText
        outputImage.SetActive(false); // hide the outputImage

        // Show the analysis outputText in the dialog
        // ここでanalysisTextの表示処理を実装
        // Debug.Log("Analysis outputText button clicked");

        // テキストを表示
        // outputText.GetComponent<TextMeshProUGUI>().text = activeHistoryContent.GetComponent<SetupTidyupProxy>().analysisText;
        outputText.GetComponentInChildren<TextMeshProUGUI>().text = activeHistoryContent.GetComponent<SetupTidyupProxy>().analysisText;

    }

    // void buttonSuggestionsTextCallback()
    // {
    //     // reset the dialog image to default
    //     dialog.GetComponent<Image>().sprite = null;
    //     menu.SetActive(false); // hide the menu
    //     outputText.SetActive(true); // show the outputText
    //     scoreText.SetActive(false); // hide the scoreText
    //     outputImage.SetActive(false); // hide the outputImage

    //     // Show the suggestions outputText in the dialog
    //     // ここでsuggestionsTextの表示処理を実装
    //     // Debug.Log("Suggestions outputText button clicked");

    //     // テキストを表示
    //     // outputText.GetComponent<TextMeshProUGUI>().text = activeHistoryContent.GetComponent<SetupTidyupProxy>().suggestionsText;
    //     outputText.GetComponentInChildren<TextMeshProUGUI>().text = activeHistoryContent.GetComponent<SetupTidyupProxy>().suggestionsText;

    // }
}
