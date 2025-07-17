using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class TidyUpCursor : MonoBehaviour
{
  public Button buttonPlaceObject;
  private float distanceFromCamera = 1000f; // distance from the camera to place the object
  public ARRaycastManager raycastManager;
  public GameObject objectToPlace;
  public List<GameObject> spawnedAnchors = new List<GameObject>();

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
    //GameObject newContainer = GameObject.Instantiate(objectToPlace, transform.position, transform.rotation);
    // 画面中央の座標を取得
    Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

    // レイキャスト実行
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    bool hitDetected = raycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Depth);

    if (hitDetected && hits.Count > 0)
    {
      ARRaycastHit hit = hits[0];
      GameObject newAnchor = GameObject.Instantiate(objectToPlace, hit.pose.position, hit.pose.rotation);

      if (newAnchor != null)
      {
        // 生成されたアンカーをリストに追加
        spawnedAnchors.Add(newAnchor);
      }
      else
      {
        Debug.LogWarning("Failed to instantiate new anchor.");
      }
    }
  }
}
