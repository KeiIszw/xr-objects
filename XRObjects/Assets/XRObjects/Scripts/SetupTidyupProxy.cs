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

    [HideInInspector] public string captureDateTime; // yyyy-MM-dd:HH-mm-ss
    [HideInInspector] public int cleanlinessScore;
    [HideInInspector] public string analysisText;
    [HideInInspector] public string suggestionsText;
    [HideInInspector] public string beforeImgPath; // いらないかも
    [HideInInspector] public string afterImgPath; // いらないかも


    // Start is called before the first frame update
    void Start()
    {
        // turn on metadata menu
        metadataMenu.GetComponent<Canvas>().enabled = true;

        // set the object name to the current time
        // jsonから時間取ったほうがいいと思う
        // captureDateTime = System.DateTime.Now.ToString("yyyy-MM-dd:HH-mm-ss");
        captureDateTime = "2025-09-27:23-28-11"; // 仮

        analysisText = "カテゴリ: 散乱物・放置物・配線、整理不足\n" + "詳細: モニター左下にスマートフォンが置かれ、その手前には付箋が貼られたボードや名刺入れらしき物が見える。左手前には飲みかけのペットボトル、使い終わったらしき紙タオルや布、未開封の箱などが無造作に置かれている。マイクの周囲にも複数のケーブルが絡まり、整理されていない。\n" + "場所: デスク上（中央から左側）";
        suggestionsText = "優先度: 高\n" + "提案: デスク上の不要な物（飲み物の空き容器、使い終わった紙類、布など）をまず片付け、ごみは捨てる。使用頻度の低い小物や書類は適切な収納場所に移動させる。\n" + "対象エリア: デスク上全体\n";

        // 以下llmの応答を待つからstart()で設定しないほうがいいかも
        // updateMetadata関数定義して、llmの応答を受け取ったときに呼び出す
        // llmのスクリプトをproxyにアタッチしておく

        // set the tidyup point
        cleanlinessScore = 100; // 仮

        // set the metadata
        string metadata = cleanlinessScore.ToString() + "\n" + captureDateTime;

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

        // Debug.Log("SetupTidyupProxy Update");
        // LLMの応答を待つ
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
