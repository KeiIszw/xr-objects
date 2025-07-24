using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Newtonsoft.Json;

public class SetupTidyupProxy : MonoBehaviour
{
    private bool objectIsSelected = false;
    public Material Material0; // object not selected
    public Material Material1; // object metadata available
    public Material Material2; // object selected

    public GameObject metadataMenu;

    private GameObject mainObjectProxy;
    public GameObject sphere;

    public GameObject tidyUpList;

    public string objectName;
    public string jsonMetadata;
    public string beforeImgPath;
    public string afterImgPath;

    // Start is called before the first frame update
    void Start()
    {
        // turn off metadata menu
        metadataMenu.GetComponent<Canvas>().enabled = true;

        // set the object name to the current time
        System.DateTime now = System.DateTime.Now;
        objectName = now.ToString("HH:mm:ss");

        // set the tidyup point
        string tidyupScore = "Score"; // 仮

        // set the metadata
        string metadata = tidyupScore + "\n" + objectName;

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

                    if (objectIsSelected)
                    {
                        // object was selected previously, let's deselect
                        //transform.Find("Sphere").GetComponent<MeshRenderer>().material = Material0;
                        _raycastHit.collider.GetComponent<MeshRenderer>().material = Material0;
                        metadataMenu.GetComponent<Canvas>().enabled = false;

                        objectIsSelected = false;
                    }
                    else
                    {
                        // object wasn't selected previously, let's select it now
                        //transform.Find("Sphere").GetComponent<MeshRenderer>().material = Material2;
                        _raycastHit.collider.GetComponent<MeshRenderer>().material = Material2;
                        metadataMenu.GetComponent<Canvas>().enabled = true;

                        objectIsSelected = true;
                    }
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
