using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TidyUpList : MonoBehaviour
{
    public Button buttonTidyUpList;

    [HideInInspector] public bool showTidyUpList = false;
    public GameObject tidyUpList;

    // Start is called before the first frame update
    void Start()
    {
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
}
