using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using System.IO;

// ButtonHistoryContentプレハブ(Assets/XRObjects/Prefabs/ButtonHistoryContent.prefab)にアタッチ
public class SetupHistoryContent : MonoBehaviour
{
    [HideInInspector] public GameObject tidyUpObject; // 設置したアンカーオブジェクト(ObjectProxy の Prefab)
    [HideInInspector] public GameObject dialog;
    [HideInInspector] public GameObject TidyUpCursor;

    // Start is called before the first frame update
    void Start()
    {
        // Get the dialog GameObject from the scene
        GameObject Canvas = GameObject.Find("Canvas");
        dialog = Canvas.transform.Find("DialogPanel").gameObject;
        if (dialog == null)
        {
            Debug.LogError("DialogPanel not found in the scene.");
        }

        // Get the TidyUpCursor GameObject from the scene
        TidyUpCursor = GameObject.Find("TidyUpCursor");

        Button buttonShowDialogButton = GetComponent<Button>();
        buttonShowDialogButton.onClick.AddListener(buttonShowDialogCallback);

        // 履歴コンテンツのテキストを設定
        string captureDateTime = tidyUpObject.GetComponent<SetupTidyupProxy>().captureDateTime; // yyyy-MM-dd:HH-mm-ss
        GetComponentInChildren<TextMeshProUGUI>().text = captureDateTime;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void buttonShowDialogCallback()
    {
        // DialogUI.cs の activeHistoryContent にコンテンツをアタッチ
        TidyUpCursor.GetComponent<DialogUI>().activeHistoryContent = tidyUpObject;
        TidyUpCursor.GetComponent<DialogUI>().buttonBeforeImgCallback();

        // ダイアログを表示
        dialog.SetActive(true);
    }
}
