using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TidyUpCursor : MonoBehaviour
{
    public GameObject objectToPlace;
    public Button buttonPlaceObject;
    private float distanceFromCamera = 1000f; // distance from the camera to place the object
    // Start is called before the first frame update
    void Start()
    {
        Button buttonPlaceObjectButton = buttonPlaceObject.GetComponent<Button>();
        buttonPlaceObjectButton.onClick.AddListener(buttonPlaceObjectCallback);

        // Set the initial position of the cursor
        Vector3 initialPosition = Camera.main.transform.position + Camera.main.transform.forward * distanceFromCamera;
        transform.position = initialPosition;
        Debug.Log("TidyUpCursor Start: Initial position set to " + initialPosition); 
    }

    // Update is called once per frame
    void Update()
    {
        // ARCoreでレイキャストしてオブジェクトを配置する位置を決定する
        // カーソルの距離更新
        // スクリーンショット
        // クエリ処理
        // 結果を格納＆表示(これはTidyUpList.cs?)
    }

    void buttonPlaceObjectCallback()
    {
        GameObject newContainer = GameObject.Instantiate(objectToPlace, transform.position, transform.rotation);
    }
}
