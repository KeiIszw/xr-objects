using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// attach this script to the AR Cursor GameObject
public class TidyUpList : MonoBehaviour
{
    public Button buttonCapture; //temp
    public Button buttonTidyUpList;

    [HideInInspector] public bool showTidyUpList = false;
    public GameObject dialog;
    public GameObject tidyUpList;
    [HideInInspector] public GameObject tidyUpListContent; // content of the tidy up list
    public GameObject tidyUpListItemPrefab; // prefab for the tidy up list items
    private List<GameObject> tidyUpListItems = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        tidyUpListContent = tidyUpList.transform.Find("Viewport/Content").gameObject;

        Button buttonCaptureButton = buttonCapture.GetComponent<Button>(); // temp
        buttonCaptureButton.onClick.AddListener(buttonCaptureCallback);

        Button buttonTidyUpListButton = buttonTidyUpList.GetComponent<Button>();
        buttonTidyUpListButton.onClick.AddListener(buttonTidyUpListCallback);

        // hide the  tidy up list at the start
        tidyUpList.SetActive(false);
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

        tidyUpListItems.Add(newItem);
        if (tidyUpListItems.Count > 0)
        {
            tidyUpListItems[tidyUpListItems.Count - 1].GetComponent<Button>().onClick.AddListener(buttonItemCallback);
        }
    }

    void buttonItemCallback()
    {
        dialog.SetActive(true); //どのダイアログを表示するか選択する
    }
}
