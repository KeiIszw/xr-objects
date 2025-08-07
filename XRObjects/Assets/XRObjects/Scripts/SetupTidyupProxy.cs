using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Newtonsoft.Json;

// TidyUpProxyプレハブ(Assets/XRObjects/Prefabs/TidyUpProxy.prefab)にアタッチ
public class SetupTidyupProxy : MonoBehaviour
{
    private bool objectIsSelected = false;
    public Material Material0; // object not selected
    public Material Material1; // object metadata available
    public Material Material2; // object selected

    public GameObject metadataMenu;

    private GameObject mainObjectProxy;
    public GameObject sphere;

    // public GameObject tidyUpList;
    [HideInInspector] public GameObject historyContent; // content of the tidy up list

    [HideInInspector] public string captureTime;
    [HideInInspector] public string AnalysisjsonPath;
    [HideInInspector] public string beforeImgPath;
    [HideInInspector] public string afterImgPath;

    // Start is called before the first frame update
    void Start()
    {
        // turn off metadata menu
        metadataMenu.GetComponent<Canvas>().enabled = true;

        // set the object name to the current time
        // jsonから時間取ったほうがいいと思う
        System.DateTime now = System.DateTime.Now;
        captureTime = now.ToString("HH:mm:ss");


        // 以下llmの応答を待つからstart()で設定しないほうがいいかも
        // updateMetadata関数定義して、llmの応答を受け取ったときに呼び出す
        // llmのスクリプトをproxyにアタッチしておく

        // set the tidyup point
        string tidyupScore = "Score"; // 仮

        // set the metadata
        string metadata = tidyupScore + "\n" + captureTime;

        metadataMenu.GetComponentInChildren<TextMeshProUGUI>().text = metadata;

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
        // check if touching the object proxy
        // if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        // check if a touch exists an it's not an a UI element (e.g. button)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !IsPointerOverUIObject())
        {

            // RaycastHit _raycastHit;
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);


            RaycastHit[] _raycastHits;
            _raycastHits = Physics.RaycastAll(raycast, Mathf.Infinity);
            // sort by distance to hit the closest one
            System.Array.Sort(_raycastHits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var _raycastHit in _raycastHits)
            {

                // check if the sphere has been touched
                if (_raycastHit.collider.gameObject == sphere)
                { //_raycastHit.collider.CompareTag("RealObjectSphere") &&
                  // object has been touched, let's change material (color)

                    // SetupHistoryContent.csのbuttonShowDialogCallback()を呼び出す
                    historyContent.GetComponent<SetupHistoryContent>().buttonShowDialogCallback();

                    // if (objectIsSelected)
                    // {
                    //     // object was selected previously, let's deselect
                    //     //transform.Find("Sphere").GetComponent<MeshRenderer>().material = Material0;
                    //     _raycastHit.collider.GetComponent<MeshRenderer>().material = Material0;
                    //     metadataMenu.GetComponent<Canvas>().enabled = false;

                    //     objectIsSelected = false;
                    // }
                    // else
                    // {
                    //     // object wasn't selected previously, let's select it now
                    //     //transform.Find("Sphere").GetComponent<MeshRenderer>().material = Material2;
                    //     _raycastHit.collider.GetComponent<MeshRenderer>().material = Material2;
                    //     metadataMenu.GetComponent<Canvas>().enabled = true;

                    //     objectIsSelected = true;
                    // }
                }
            }


        }
    }

    public void deselectObject()
    {
        sphere.GetComponent<MeshRenderer>().material = Material0;
        metadataMenu.GetComponent<Canvas>().enabled = false;

        objectIsSelected = false;

    }

    private bool IsPointerOverUIObject()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        for (int touchIndex = 0; touchIndex < Input.touchCount; touchIndex++)
        {
            Touch touch = Input.GetTouch(touchIndex);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return true;
        }

        return false;
    }

}
