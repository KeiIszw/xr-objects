using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

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
    public GameObject text;
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
        // hide the text at the start
        text.SetActive(false);

        Button buttonBeforeImgButton = buttonBeforeImg.GetComponent<Button>();
        buttonBeforeImgButton.onClick.AddListener(buttonBeforeImgCallback);

        Button buttonAfterImgButton = buttonAfterImg.GetComponent<Button>();
        buttonAfterImgButton.onClick.AddListener(buttonAfterImgCallback);

        Button buttonAnalysisTextButton = buttonAnalysisText.GetComponent<Button>();
        buttonAnalysisTextButton.onClick.AddListener(buttonAnalysisTextCallback);

        Button buttonSuggestionsTextButton = buttonSuggestionsText.GetComponent<Button>();
        buttonSuggestionsTextButton.onClick.AddListener(buttonSuggestionsTextCallback);

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
        // hide the text
        text.SetActive(false);
        // show the capture button
        buttonCapture.gameObject.SetActive(true);

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

        // hide the text
        text.SetActive(false);
    }

    void buttonBeforeImgCallback()
    {
        menu.SetActive(false); // hide the menu
        text.SetActive(false); // hide the text

        // Show the before image in the dialog
        // ここでbeforeImgの表示処理を実装
        Debug.Log("Before image button clicked");
    }

    void buttonAfterImgCallback()
    {
        menu.SetActive(false); // hide the menu
        text.SetActive(false); // hide the text

        // Show the after image in the dialog
        // ここでafterImgの表示処理を実装
        Debug.Log("After image button clicked");
    }

    void buttonAnalysisTextCallback()
    {
        menu.SetActive(false); // hide the menu
        text.SetActive(true); // show the text

        // Show the analysis text in the dialog
        // ここでanalysisTextの表示処理を実装
        Debug.Log("Analysis text button clicked");
    }

    void buttonSuggestionsTextCallback()
    {
        menu.SetActive(false); // hide the menu
        text.SetActive(true); // show the text

        // Show the suggestions text in the dialog
        // ここでsuggestionsTextの表示処理を実装
        Debug.Log("Suggestions text button clicked");
    }
    const string testJson = @"
                          {
                          ""analysis_of_disorder"": [
                          {
                          ""category"": ""散乱物・放置物・配線、整理不足"",
                          ""details"": ""モニター左下にスマートフォンが置かれ、その手前には付箋が貼られたボードや名刺入れらしき物が見える。左手前には飲みかけのペットボトル、使い終わったらしき紙タオルや布、未開封の箱などが無造作に置かれている。マイクの周囲にも複数のケーブルが絡まり、整理されていない。"",
                          ""location"": ""デスク上（中央から左側）""
                          },
                          {
                          ""category"": ""散乱物・放置物・配線、整理不足"",
                          ""details"": ""MacBookが置かれたスタンドの下やその周囲に、複数の充電アダプター、バッテリーチャージャー、大量のケーブル、ヘッドホン、使用済みの紙類、領収書らしきもの、小物などが雑然と積み重ねられており、作業スペースが狭くなっている。特に右奥は物が完全に山になっている状態。"",
                          ""location"": ""デスク上（中央から右側）""
                          },
                          {
                          ""category"": ""散乱物・放置物、整理不足"",
                          ""details"": ""デスク右奥の棚には、タブレットが無造作に立てかけられ、その周辺には袋に入った物、書類、その他の小物類が積み重なって置かれている。棚板の上に物が置かれすぎており、収納スペースとして機能しているとは言い難い状態。"",
                          ""location"": ""棚（デスク右奥）""
                          },
                          {
                          ""category"": ""散乱物・放置物・配線"",
                          ""details"": ""モニター裏、デスク上、電源タップ周辺、そして棚の周囲に至るまで、非常に多くのケーブルが絡まり、乱雑に散らばっている。見た目が非常に悪く、埃が溜まりやすいだけでなく、安全面にも懸念がある。"",
                          ""location"": ""全体の配線""
                          }
                          ],
                          ""improvement_suggestions"": [
                          {
                          ""priority"": ""高"",
                          ""suggestion"": ""デスク上の不要な物（飲み物の空き容器、使い終わった紙類、布など）をまず片付け、ごみは捨てる。使用頻度の低い小物や書類は適切な収納場所に移動させる。"",
                          ""target_area"": ""デスク上全体""
                          },
                          {
                          ""priority"": ""高"",
                          ""suggestion"": ""モニター裏や電源タップ周辺のケーブルを、ケーブルタイやケーブルボックス、スリーブなどを使って束ね、隠す。不要なケーブルは処分するか、収納ケースにしまう。"",
                          ""target_area"": ""配線""
                          },
                          {
                          ""priority"": ""中"",
                          ""suggestion"": ""多数のアダプターや充電器をまとめるために、ケーブルボックスや電源タップ収納ボックスを導入する。書類や紙類はファイルボックスやトレイを使って整理し、定位置を決める。MacBookスタンドの下のスペースを有効活用できるよう、スリムな引き出しやオーガナイザーを設置する。"",
                          ""target_area"": ""デスク上（右側）""
                          },
                          {
                          ""priority"": ""中"",
                          ""suggestion"": ""棚の物を一度すべて取り出し、必要・不必要を判断する。必要な物は種類ごとに収納ボックスやファイルボックスを活用し、見た目もスッキリさせる。タブレットは専用のスタンドを導入するなどして、倒れないように配置する。"",
                          ""target_area"": ""棚（デスク右奥）""
                          },
                          {
                          ""priority"": ""低"",
                          ""suggestion"": ""定期的にデスク上の物をリセットする時間を設ける。作業の終わりに、その日のうちに散らかった物を元の場所に戻す習慣をつける。"",
                          ""target_area"": ""全体の維持""
                          }
                          ],
                          ""overall_cleanliness_score"": 35
                          }
                          ";
}
