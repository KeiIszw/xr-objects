using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// attach this script to the AR Cursor GameObject
public class DialogUI : MonoBehaviour
{
    public Button buttonCloseDialog;
    // public Button buttonShowDialog;
    public Button buttonShowMenu;
    public GameObject dialog;

    // Start is called before the first frame update
    void Start()
    {
        Button buttonCloseDialogButton = buttonCloseDialog.GetComponent<Button>();
        buttonCloseDialogButton.onClick.AddListener(buttonCloseDialogCallback);

        // Button buttonShowDialogButton = buttonShowDialog.GetComponent<Button>();
        // buttonShowDialogButton.onClick.AddListener(buttonShowDialogCallback);

        Button buttonShowMenuButton = buttonShowMenu.GetComponent<Button>();
        buttonShowMenuButton.onClick.AddListener(buttonShowMenuCallback);

        // hide the dialog at the start
        dialog.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void buttonCloseDialogCallback()
    {
        // hide the dialog
        dialog.SetActive(false);
    }
    // void buttonShowDialogCallback()
    // {
    //     // show the dialog
    //     dialog.SetActive(true);
    // }
    void buttonShowMenuCallback()
    {
        // show the menu
    }
}
