using System;  
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.IO;

public class CameraCapturer : MonoBehaviour
{
    // public ARCameraManager cameraManager;
    public string captureDateTime; // yyyy-MM-dd:HH-mm-ss

    private Texture2D FlipTextureHorizontally(Texture2D original)
    {
        int width = original.width;
        int height = original.height;
        Texture2D flipped = new Texture2D(width, height, original.format, false);
        Color[] pixels = original.GetPixels();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                flipped.SetPixel(x, y, pixels[y * width + (width - x - 1)]);
            }
        }

        flipped.Apply();
        return flipped;
    }

    public void CaptureAndSave()
    {
        GameObject arCamera = GameObject.Find("AR Session Origin/AR Camera");
        ARCameraManager cameraManager = arCamera.GetComponent<ARCameraManager>();

        if (cameraManager == null)
        {
            // Debug.LogError("ARCameraManager が設定されていません。");
            return;
        }

        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            // Debug.LogError("最新のCPUイメージを取得できませんでした。");
            return;
        }

        var conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width, image.height),
            outputFormat = TextureFormat.RGBA32
        };

        int size = image.GetConvertedDataSize(conversionParams);
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        unsafe
        {
            image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);
        }

        image.Dispose();

        Texture2D texture = new Texture2D(
            conversionParams.outputDimensions.x,
            conversionParams.outputDimensions.y,
            conversionParams.outputFormat,
            false);

        texture.LoadRawTextureData(buffer);
        texture.Apply();
        texture = FlipTextureHorizontally(texture);

        buffer.Dispose();

        byte[] bytes = texture.EncodeToPNG();

        // ↓↓↓ 保存ディレクトリ＆ファイル名の生成 ↓↓↓
        string dateDirName = DateTime.Now.ToString("yyyy-MM-dd");
        string directoryPath = Path.Combine(FileUtils.GetDocumentsPath(), dateDirName);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string timeStamp = DateTime.Now.ToString("HH-mm-ss");
        string filename = Path.Combine(directoryPath, $"before_{timeStamp}.png");

        File.WriteAllBytes(filename, bytes);
        // Debug.Log($"📸 スクリーンショットを保存しました: {filename}");

        Destroy(texture);

        captureDateTime = dateDirName + ":" + timeStamp; // yyyy-MM-dd:HH-mm-ss
    }
}
