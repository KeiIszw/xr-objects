using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

/// <summary>
/// 画面中央にアンカーを配置するクラス
/// </summary>
public class CenterAnchorPlacer : MonoBehaviour
{
  [Header("AR Components")]
  public ARRaycastManager raycastManager;
  public GameObject objectToPlace; // アンカー位置に配置するオブジェクト

  [Header("UI")]
  public Button buttonCapture;

  private List<GameObject> spawnedAnchors = new List<GameObject>();

  void Start()
  {
    // ButtonCaptureボタンにクリックイベントを追加
    if (buttonCapture != null)
    {
      buttonCapture.onClick.AddListener(PlaceAnchorAtScreenCenter);
    }
    else
    {
      Debug.LogWarning("ButtonCapture Not Registered");
    }
  }

  /// <summary>
  /// 画面中央にアンカーを配置する
  /// </summary>
  private void PlaceAnchorAtScreenCenter()
  {
    // 画面の中央座標を取得
    Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

    // ARRaycastManagerを使用してレイキャストを実行
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // ARCursorと同様にDepthトラッキングを使用
    bool hitDetected = raycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Depth);

    if (hitDetected && hits.Count > 0)
    {
      ARRaycastHit hit = hits[0];

      GameObject newAnchor = GameObject.Instantiate(objectToPlace, hit.pose.position, hit.pose.rotation);

      if (newAnchor != null)
      {
        // 生成されたアンカーをリストに追加
        spawnedAnchors.Add(newAnchor);
        // バイブレーション
        //Vibrate(50, 50);
      }
      else
      {
        Debug.LogWarning("Failed to instantiate anchor object");
      }
    }
    else
    {
      Debug.LogWarning("Laycast did not hit at screen center.");
    }
  }

  /// <summary>
  /// すべてのアンカーを削除
  /// </summary>
  public void ClearAllAnchors()
  {
    foreach (GameObject anchor in spawnedAnchors)
    {
      if (anchor != null)
      {
        Destroy(anchor);
      }
    }
    spawnedAnchors.Clear();
    Debug.Log("Deleted all anchors.");
  }

  private void OnDestroy()
  {
    // イベントを解除
    if (buttonCapture != null)
    {
      buttonCapture.onClick.RemoveListener(PlaceAnchorAtScreenCenter);
    }
  }
}