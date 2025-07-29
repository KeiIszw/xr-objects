using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System.Linq;

// --- JSON Data Structures (For Gemini API Request) ---

// Represents a single part within the 'parts' array (either text or inline_data)
[Serializable]
public class GeminiPart
{
    // These fields are designed to be mutually exclusive for Gemini API's 'oneof' constraint.
    // JsonUtility is expected to omit null fields during serialization.
    public string text;
    public GeminiInlineData inline_data;
}

// Represents inline data for an image within a GeminiPart
[Serializable]
public class GeminiInlineData
{
    public string mime_type;
    public string data; // Base64 encoded image
}

// Represents the 'contents' array, holding a list of GeminiPart objects
[Serializable]
public class GeminiContent
{
    public List<GeminiPart> parts;
}

// The complete JSON request payload sent to the Gemini API
[Serializable]
public class GeminiRequest
{
    public List<GeminiContent> contents;
    // Optional: You can add properties for generationConfig or safetySettings here
    // public GenerationConfig generationConfig;
}


// --- JSON Data Structures (For Gemini API Response) ---

// Represents a candidate response from the Gemini API
[Serializable]
public class GeminiCandidate
{
    public Content content;
    // Additional fields like 'finishReason', 'index', 'safetyRatings' could be added here if needed.
}

// Represents the 'content' within a GeminiCandidate
[Serializable]
public class Content
{
    public List<Part> parts;
    public string role; // Role of the content (e.g., "model", "user")
}

// Represents a single part within the 'parts' array of the response (typically text)
[Serializable]
public class Part
{
    public string text;
    // If the model generates an image, its data would be in an 'inlineData' field here
    // public GeminiInlineData inlineData;
}

// The complete JSON response received from the Gemini API
[Serializable]
public class GeminiResponse
{
    public List<GeminiCandidate> candidates;
    // Additional fields like 'promptFeedback' could be added here
    // public PromptFeedback promptFeedback;
}


// --- JSON Data Structures (For Cleanliness Report Output) ---

// Represents an analysis entry for a disordered area
[Serializable]
public class DisorderAnalysis
{
    public string location;
    public string details;
    public string category;
}

// Represents a suggestion for improvement
[Serializable]
public class ImprovementSuggestion
{
    public string target_area;
    public string suggestion;
    public string priority;
}

// The overall cleanliness report structure
[Serializable]
public class CleanlinessReport
{
    public int overall_cleanliness_score;
    public List<DisorderAnalysis> analysis_of_disorder;
    public List<ImprovementSuggestion> improvement_suggestions;
}


/// <summary>
/// Handles HTTP requests to the Google Gemini API for cleanliness analysis
/// based on an input image.
/// </summary>
public class HttpReqByButton : MonoBehaviour
{
  private string EscapeJson(string input)
  {
    return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
  }
    // Assign this UI Button component in the Unity Editor Inspector
    public Button testBtn;

    [Tooltip("Your Google Gemini API Key from Google AI Studio. DO NOT COMMIT TO VERSION CONTROL!")]
    [SerializeField]
    private string apiKey = "**YOUR_GEMINI_API_KEY**"; // <<< REMEMBER TO REPLACE THIS WITH YOUR ACTUAL API KEY!

    [Header("Analysis Model Settings")]
    [Tooltip("The full API URL for the Gemini Vision model (e.g., https://generativelanguage.googleapis.com/v1beta/models/gemini-pro-vision:generateContent)")]
    // 'gemini-pro-vision' is generally recommended for image analysis.
    [SerializeField]
    private string analysisApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";
    [Tooltip("The name of the Gemini model used for analysis (e.g., gemini-pro-vision). This is primarily for internal reference.")]
    [SerializeField]
    private string analysisModelName = "gemini-pro-vision"; // Internal reference, URL already specifies the model

    [Header("Image Path Settings")]
    [Tooltip("The name of the image file located in Assets/InputImages (e.g., my_room.jpg)")]
    [SerializeField]
    private string imageFileName = "img2.png"; // Set your image file name here

    /// <summary>
    /// Called when the script instance is being loaded. Sets up the button listener.
    /// </summary>
    void Start()
    {
        if (testBtn != null)
        {
            testBtn.onClick.AddListener(OnTestButtonClick);
            Debug.Log("CleanlinessAnalyzer: Button click listener added.");
        }
        else
        {
            Debug.LogError("CleanlinessAnalyzer: testBtn is not assigned! Please assign a Button component in the Inspector.");
        }

        string inputPath = Path.Combine(Application.dataPath, "InputImages");
        if (!Directory.Exists(inputPath))
        {
            Debug.LogWarning($"CleanlinessAnalyzer: Input image directory '{inputPath}' not found. Please create it and place your image files there.");
        }

        // Validate API Key presence at startup
        if (string.IsNullOrEmpty(apiKey) || apiKey == "**YOUR_GEMINI_API_KEY**")
        {
            Debug.LogError("CleanlinessAnalyzer: Gemini API Key is not set or is still the placeholder. Please set your API key in the Inspector!");
        }
    }

    /// <summary>
    /// Called when the assigned UI button is clicked. Initiates the image analysis coroutine.
    /// </summary>
    private void OnTestButtonClick()
    {
        Debug.Log("CleanlinessAnalyzer: Button clicked. Initiating image analysis...");
        StartCoroutine(AnalyzeImageCoroutine());
    }

    /// <summary>
    /// Coroutine to orchestrate the entire image analysis process:
    /// - Checks for image file existence.
    /// - Calls the GetCleanlinessReport coroutine.
    /// - Saves the received report to a JSON file.
    /// </summary>
    private IEnumerator AnalyzeImageCoroutine()
    {
        string imagePath = Path.Combine(Application.dataPath, "InputImages", imageFileName);

        if (!File.Exists(imagePath))
        {
            Debug.LogError($"CleanlinessAnalyzer: Image file not found: {imagePath}\n" +
                           "Please place your image file in the 'Assets/InputImages' folder and set 'Image File Name' correctly in the Inspector.");
            yield break;
        }

        Debug.Log($"CleanlinessAnalyzer: [Processing Started] Image: {Path.GetFileName(imagePath)}");

        CleanlinessReport report = null; // Variable to hold the parsed report

        // Start the GetCleanlinessReport coroutine and wait for its completion
        yield return StartCoroutine(GetCleanlinessReport(imagePath, (resultReport) => {
            report = resultReport; // Receive the result via callback
        }));

        if (report == null)
        {
            Debug.LogError($"CleanlinessAnalyzer: [Analysis Failed] Failed to get a valid JSON report for: {Path.GetFileName(imagePath)}. Check previous logs for API errors or parsing issues.");
            yield break;
        }

        string outputPath = Path.Combine(Application.dataPath, "Output");
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
            Debug.Log($"CleanlinessAnalyzer: Output directory created: {outputPath}");
        }

        string jsonReportContent = JsonUtility.ToJson(report, true); // Serialize to pretty-printed JSON
        string outputJsonPath = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(imageFileName)}_report.json");
        File.WriteAllText(outputJsonPath, jsonReportContent); // Save the JSON report
        Debug.Log($"CleanlinessAnalyzer: [Analysis Success] Analysis report saved to: {outputJsonPath}");

        // Reminder that image generation is not typical for vision models
        Debug.LogWarning("CleanlinessAnalyzer: Note that direct image generation is not typically supported by Gemini Vision models. This process focuses on text-based report generation from images.");
    }

    /// <summary>
    /// Coroutine to send an HTTP POST request to the Gemini API for cleanliness analysis.
    /// This method constructs the JSON payload, sends the request, and parses the response.
    /// </summary>
    /// <param name="imagePath">The full path to the image file to analyze.</param>
    /// <param name="callback">A callback function to return the parsed CleanlinessReport result (null if failed).</param>
    private IEnumerator GetCleanlinessReport(string imagePath, System.Action<CleanlinessReport> callback)
{
    // --- 1. 画像読み込みとBase64エンコード ---
    byte[] imageBytes = File.ReadAllBytes(imagePath);
    string base64Image = Convert.ToBase64String(imageBytes);
    string mimeType = GetMimeType(imagePath);

    // --- 2. Prompt作成 ---
    string prompt = "以下の画像に写っている空間の整理整頓状況を分析し、100点満点で採点してください。..." +
                    "(中略) JSON形式で出力してください。";

    // --- ✅ 3. 手動でJSON文字列構築 ---
    string jsonPayload = "{"
        + "\"contents\":[{"
        + "\"role\":\"user\","
        + "\"parts\":["
        + "{\"text\":\"" + EscapeJson(prompt) + "\"},"
        + "{\"inline_data\":{\"mime_type\":\"" + mimeType + "\",\"data\":\"" + base64Image + "\"}}"
        + "]"
        + "}]}";

    // --- 4. POSTリクエスト ---
    using (UnityWebRequest webRequest = new UnityWebRequest($"{analysisApiUrl}?key={apiKey}", "POST"))
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("CleanlinessAnalyzer: Sending request with hand-written JSON...");
        yield return webRequest.SendWebRequest();

        CleanlinessReport resultReport = null;

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"CleanlinessAnalyzer: [API Error] Failed. Code: {webRequest.responseCode}\n{webRequest.downloadHandler.text}");
        }
        else
        {
            try
            {
                string responseText = webRequest.downloadHandler.text;
                Debug.Log("CleanlinessAnalyzer: Raw Response: " + responseText);

                GeminiResponse geminiResponse = JsonUtility.FromJson<GeminiResponse>(responseText);
                string output = geminiResponse?.candidates?[0]?.content?.parts?[0]?.text;
              
                if (!string.IsNullOrEmpty(output))
                {
                    int braceStart = output.IndexOf('{');
                    int braceEnd = output.LastIndexOf('}');
                    if (braceStart != -1 && braceEnd > braceStart)
                    {
                      string jsonReportRaw = output.Substring(braceStart, braceEnd - braceStart + 1);
                      Debug.Log($"✅ Extracted JSON from Model Output:\n{jsonReportRaw}");

                      try
                      {
                        CleanlinessReport parsedReport = JsonUtility.FromJson<CleanlinessReport>(jsonReportRaw);

                        // ✅ 出力先ディレクトリ準備
                        string outputPath = Path.Combine(Application.dataPath, "Output");
                        if (!Directory.Exists(outputPath))
                        {
                          Directory.CreateDirectory(outputPath);
                          Debug.Log($"CleanlinessAnalyzer: Output directory created: {outputPath}");
                        }

                        // ✅ ① 手動抽出JSONの保存
                        string outputJsonPathRaw = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(imagePath)}_raw_output.json");
                        File.WriteAllText(outputJsonPathRaw, jsonReportRaw);
                        Debug.Log($"✅ Raw Gemini JSON Saved to: {outputJsonPathRaw}");

                        // ✅ ② Geminiレスポンス全体の保存
                        string outputJsonPathFull = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(imagePath)}_gemini_full_response.json");
                        File.WriteAllText(outputJsonPathFull, responseText);
                        Debug.Log($"✅ Full Gemini Response Saved to: {outputJsonPathFull}");

                        // ✅ ③ パース済み構造体の保存
                        string outputJsonPathClean = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(imagePath)}_cleanliness_report.json");
                        File.WriteAllText(outputJsonPathClean, JsonUtility.ToJson(parsedReport, true));
                        Debug.Log($"✅ Parsed Cleanliness Report Saved to: {outputJsonPathClean}");

                        callback?.Invoke(parsedReport);
                        yield break;
                      }
                      catch (Exception parseEx)
                      {
                          Debug.LogError($"❌ Failed to deserialize CleanlinessReport from model output: {parseEx.Message}\nRaw JSON: {jsonReportRaw}");
                          // パースに失敗した場合は null を渡してコルーチンを終了
                          callback?.Invoke(null);
                          yield break; // ここでコルーチンを終了させる
                      }
                    }
                    else
                    {
                        Debug.LogError("❌ JSON構造 (中括弧) が見つかりませんでした。");
                        callback?.Invoke(null);
                        yield break; // ここでコルーチンを終了させる
                    }
                }
                else
                {
                    Debug.LogError("❌ Geminiからのtext出力が空です。");
                    callback?.Invoke(null);
                    yield break; // ここでコルーチンを終了させる
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[Parse Error] {e.Message}");
            }
        }

        callback?.Invoke(resultReport);
    }
}

    /// <summary>
    /// Determines the MIME type of an image based on its file extension.
    /// </summary>
    /// <param name="imagePath">The full path to the image file.</param>
    /// <returns>The appropriate MIME type string (e.g., "image/jpeg", "image/png").</returns>
    private string GetMimeType(string imagePath)
    {
        string ext = Path.GetExtension(imagePath).ToLower();
        switch (ext)
        {
            case ".jpg":
            case ".jpeg": return "image/jpeg";
            case ".png": return "image/png";
            case ".webp": return "image/webp"; // WebP is also a supported format for Gemini Vision
            default:
                Debug.LogWarning($"CleanlinessAnalyzer: Unknown image file extension '{ext}'. Defaulting to 'application/octet-stream'. " +
                                 "This might cause issues with the Gemini API if the actual image type is not correctly inferred.");
                return "application/octet-stream"; // Fallback for unknown types
        }
    }
}

