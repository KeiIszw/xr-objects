using System;  
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.IO;

public class CaptureButton : MonoBehaviour
{
    public ARCameraManager cameraManager;
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
    public void TakeScreenshot()
    {
        if (cameraManager == null)
        {
            Debug.LogError("ARCameraManager が設定されていません。");
            return;
        }

        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            Debug.LogError("最新のCPUイメージを取得できませんでした。");
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
        string filename = Path.Combine(Application.persistentDataPath, $"{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        File.WriteAllBytes(filename, bytes);

        Debug.Log($"📸 スクリーンショットを保存しました: {filename}");

        // 不要になったTexture2Dを破棄
        Destroy(texture);
    }
}