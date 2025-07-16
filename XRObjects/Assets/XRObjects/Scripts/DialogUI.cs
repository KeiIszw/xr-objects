using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// attach this script to the AR Cursor GameObject
public class DialogUI : MonoBehaviour
{
    public Button buttonCloseDialog;
    public Button buttonShowMenu;
    public Button buttonCapture;
    public Button buttonTidyUpList;
    public GameObject tidyUpList;
    public GameObject tidyUpListItemPrefab; // prefab for the tidy up list items
    public GameObject dialog;
    public GameObject menu;

    [HideInInspector] public GameObject tidyUpListContent; // content of the tidy up list
    [HideInInspector] public bool showTidyUpList = false;
    private List<GameObject> tidyUpListItems = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        tidyUpListContent = tidyUpList.transform.Find("Viewport/Content").gameObject;

        Button buttonCaptureButton = buttonCapture.GetComponent<Button>();
        buttonCaptureButton.onClick.AddListener(buttonCaptureCallback);

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
    }

    // Update is called once per frame
    void Update()
    {

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

    void buttonCaptureCallback() //temp
    {
        GameObject newItem = Instantiate(tidyUpListItemPrefab, tidyUpListContent.transform);
        // 画像とテキストが入ったダイアログを生成する．
        // 画像とテキストをリストに格納しとく
        // リストのアイテム名は撮影時間にする

        tidyUpListItems.Add(newItem);
        if (tidyUpListItems.Count > 0)
        {
            tidyUpListItems[tidyUpListItems.Count - 1].GetComponent<Button>().onClick.AddListener(buttonItemCallback);
        }
    }

    void buttonItemCallback()
    {
        buttonCapture.gameObject.SetActive(false);//hide the capture button
        dialog.SetActive(true); //どのダイアログを表示するか選択する
    }

    void buttonCloseDialogCallback()
    {
        // hide the dialog
        dialog.SetActive(false);
        // hide the menu
        menu.SetActive(false);
        // show the capture button
        buttonCapture.gameObject.SetActive(true);
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
    }
}
