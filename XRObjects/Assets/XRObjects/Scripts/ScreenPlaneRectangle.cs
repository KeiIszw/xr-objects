using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// TidyUpCursor.csの方法を参考に、ARレイキャストで画面四隅の角マーカーと線を表示するクラス
/// </summary>
public class ScreenPlaneRectangle : MonoBehaviour
{
    [Header("UI Components")]
    public Button captureButton;

    [Header("AR Settings")]
    [SerializeField] private float planeDistance = 2.0f; // フォールバック時の距離

    [Header("Debug Settings")]
    [SerializeField] private bool showCornerMarkers = true;
    [SerializeField] private float markerSize = 0.05f; // 少し大きくして見やすく

    // AR Components
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARAnchorManager anchorManager;
    private Camera arCamera;

    // Objects
    private List<GameObject> cornerMarkers = new List<GameObject>();
    private List<ARAnchor> cornerAnchors = new List<ARAnchor>(); // 角マーカーのARアンカー

    void Start()
    {
        // AR コンポーネントを自動取得
        InitializeARComponents();

        // ボタンイベントを設定
        SetupCaptureButton();
    }

    /// <summary>
    /// ARコンポーネントの初期化
    /// </summary>
    private void InitializeARComponents()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
        anchorManager = FindObjectOfType<ARAnchorManager>();
        arCamera = Camera.main;
    }

    /// <summary>
    /// Captureボタンのイベント設定
    /// </summary>
    private void SetupCaptureButton()
    {
        if (captureButton != null)
        {
            captureButton.onClick.AddListener(PlaceRectangleOnScreenPlane);
        }
        else
        {
            Debug.LogError("CaptureButtonが設定されていません。");
        }
    }

    /// <summary>
    /// TidyUpCursor.csの方法を参考に、ARレイキャストで画面四隅の角マーカーと線を表示（メイン処理）
    /// </summary>
    public void PlaceRectangleOnScreenPlane()
    {
        Debug.Log("開始");

        // ARコンポーネントの確認
        if (arCamera == null)
        {
            return;
        }

        if (raycastManager == null)
        {
            Debug.LogError("ARRaycastManager未設定");
            return;
        }


        // 1. ARレイキャストで画面中央の実際のAR空間位置を取得（TidyUpCursor.csと同じ方法）
        Vector3 arHitPosition = GetScreenCenterAnchor();

        if (arHitPosition == Vector3.zero)
        {
            return;
        }


        // 2. AR位置を基準に画面四隅の3D座標を計算
        Vector3[] screenCorners3D = CalculateScreenCornersFromARPosition(arHitPosition);

        if (screenCorners3D == null || screenCorners3D.Length != 4)
        {
            return;
        }

        // 3. 角マーカーと線を表示
        if (showCornerMarkers)
        {
            ShowCornerMarkers(screenCorners3D);
        }
    }

    /// <summary>
    /// AR位置を基準に画面四隅の3D座標を計算
    /// </summary>
    private Vector3[] CalculateScreenCornersFromARPosition(Vector3 arPosition)
    {
        if (arCamera == null)
        {
            return null;
        }

        Debug.Log($"🔧 AR位置基準で画面四隅を計算: {arPosition}");

        // AR位置までの距離を計算
        float distanceToAR = Vector3.Distance(arCamera.transform.position, arPosition);
        Debug.Log($"📏 AR位置までの距離: {distanceToAR:F2}m");

        // カメラの視野角とアスペクト比を取得
        float halfFOV = arCamera.fieldOfView * 0.5f * Mathf.Deg2Rad;
        float aspect = arCamera.aspect;

        // AR距離での画面の高さと幅を計算
        float height = distanceToAR * Mathf.Tan(halfFOV) * 2f;
        float width = height * aspect;

        Debug.Log($"📐 計算された画面サイズ - 幅: {width:F2}m, 高さ: {height:F2}m");

        // AR位置を中心とした四隅座標を計算
        Vector3 right = arCamera.transform.right * (width * 0.5f);
        Vector3 up = arCamera.transform.up * (height * 0.5f);

        // 四隅の座標（左下、右下、右上、左上の順）
        Vector3[] corners = new Vector3[]
        {
            arPosition - right - up,  // 左下
            arPosition + right - up,  // 右下
            arPosition + right + up,  // 右上
            arPosition - right + up   // 左上
        };

        for (int i = 0; i < corners.Length; i++)
        {
            Debug.Log($"   角{i}: {corners[i]}");
        }

        return corners;
    }

    /// <summary>
    /// ARRaycastManagerを使用して画面中央の実際のAR空間位置を取得（TidyUpCursor.csと同じ方法）
    /// </summary>
    private Vector3 GetScreenCenterAnchor()
    {
        // 画面中央の座標を取得（TidyUpCursor.csと完全に同じ方法）
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        if (raycastManager == null)
        {
            return Vector3.zero;
        }

        // レイキャスト実行（TidyUpCursor.csと同じ処理）
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        bool hitDetected = raycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Depth);

        if (hitDetected && hits.Count > 0)
        {
            ARRaycastHit hit = hits[0];
            Vector3 hitPosition = hit.pose.position;

            return hitPosition;
        }

        // AR深度取得に失敗した場合、Planeを試行
        bool planeHitDetected = raycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds);

        if (planeHitDetected && hits.Count > 0)
        {
            ARRaycastHit hit = hits[0];
            Vector3 hitPosition = hit.pose.position;

            return hitPosition;
        }

        // 全てのAR手法が失敗した場合のフォールバック
        Vector3 fallbackPosition = arCamera.transform.position + arCamera.transform.forward * planeDistance;

        return fallbackPosition;
    }

    /// <summary>
    /// デバッグ用の角マーカーと線を作成（ARアンカーで空間固定）
    /// </summary>
    private void ShowCornerMarkers(Vector3[] corners)
    {
        ClearCornerMarkers();

        if (anchorManager == null)
        {
            return;
        }

        // 角マーカーを作成（ARアンカーで固定）
        for (int i = 0; i < corners.Length; i++)
        {
            // ARアンカーを作成（新しいAPI）
            GameObject anchorObject = new GameObject($"CornerAnchor_{i}");
            anchorObject.transform.position = corners[i];
            anchorObject.transform.rotation = Quaternion.identity;

            ARAnchor anchor = anchorObject.AddComponent<ARAnchor>();

            if (anchor != null)
            {
                cornerAnchors.Add(anchor);

                // マーカーオブジェクトを作成
                GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.name = $"ScreenCorner_{i}";
                marker.transform.SetParent(anchor.transform, false);
                marker.transform.localPosition = Vector3.zero;
                marker.transform.localScale = Vector3.one * markerSize;

                // マーカーの色を設定
                Renderer markerRenderer = marker.GetComponent<Renderer>();
                Material markerMaterial = new Material(Shader.Find("Standard"));

                Color[] markerColors = { Color.red, Color.green, Color.blue, Color.yellow };
                markerMaterial.color = markerColors[i % markerColors.Length];
                markerRenderer.material = markerMaterial;

                cornerMarkers.Add(marker);

            }
            else
            {
                Debug.LogError($"失敗");
            }
        }

        Debug.Log($"完了");
    }

    /// <summary>
    /// 角マーカーとARアンカーをクリア
    /// </summary>
    private void ClearCornerMarkers()
    {
        // 角マーカーを削除
        foreach (var marker in cornerMarkers)
        {
            if (marker != null)
            {
                DestroyImmediate(marker);
            }
        }
        cornerMarkers.Clear();

        // 角マーカーのARアンカーを削除
        foreach (var anchor in cornerAnchors)
        {
            if (anchor != null && anchor.gameObject != null)
            {
                DestroyImmediate(anchor.gameObject);
            }
        }
        cornerAnchors.Clear();

        Debug.Log("アンカーをクリア");
    }

    /// <summary>
    /// 平面距離を設定
    /// </summary>
    public void SetPlaneDistance(float distance)
    {
        planeDistance = Mathf.Clamp(distance, 0.5f, 10f);
    }

    private void OnDestroy()
    {
        // リソースをクリーンアップ
        ClearCornerMarkers();

        // イベントを解除
        if (captureButton != null)
        {
            captureButton.onClick.RemoveListener(PlaceRectangleOnScreenPlane);
        }
    }

    // エディタ用のテストメソッド
    [ContextMenu("Test Place Rectangle")]
    public void TestPlaceRectangle()
    {
        PlaceRectangleOnScreenPlane();
    }

    [ContextMenu("Clear All Rectangles")]
    public void TestClearRectangles()
    {
        ClearCornerMarkers();
    }

    [ContextMenu("Toggle Corner Markers")]
    public void TestToggleCornerMarkers()
    {
        showCornerMarkers = !showCornerMarkers;
    }

    [ContextMenu("Toggle Temporary Plane")]
    public void TestToggleTemporaryPlane()
    {
    }
}
