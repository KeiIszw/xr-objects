using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TidyUpCursor : MonoBehaviour
{
    public GameObject objectToPlace;
    public Button buttonPlaceObject;
    // Start is called before the first frame update
    void Start()
    {
        Button buttonPlaceObjectButton = buttonPlaceObject.GetComponent<Button>();
        buttonPlaceObjectButton.onClick.AddListener(buttonPlaceObjectCallback);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void buttonPlaceObjectCallback()
    {
        GameObject newContainer = GameObject.Instantiate(objectToPlace, new Vector3(1000, 500, 0), Quaternion.identity);
    }
}
