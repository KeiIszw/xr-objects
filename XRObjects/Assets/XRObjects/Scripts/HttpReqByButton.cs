using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System.Linq;

// --- JSONデータ構造 (Gemini APIリクエスト用) ---

// GeminiPartの定義は、'oneof'をJsonUtilityで扱うために慎重な処理が必要です。
// 今回は、このクラスを直接JsonUtility.ToJsonでシリアライズせず、
// 手動でJSON文字列を構築する際に、その構造に沿って利用します。
[Serializable]
public class GeminiPart
{
    public string text;
    public GeminiInlineData inline_data;
}

// GeminiPart内の画像に対するインラインデータを表します
[Serializable]
public class GeminiInlineData
{
    public string mime_type;
    public string data; // Base64エンコードされた画像
}

// 'contents'配列を表し、GeminiPartオブジェクトのリストを保持します
[Serializable]
public class GeminiContent
{
    public List<GeminiPart> parts;
    public string role;
}

// GenerationConfig（応答モダリティ用）
[Serializable]
public class GenerationConfig
{
    public List<string> responseModalities; // 例: ["IMAGE", "TEXT"]
}

// The complete JSON request payload sent to the Gemini API
[Serializable]
public class GeminiRequest
{
    public List<GeminiContent> contents;
    public GenerationConfig generationConfig;
}

// --- JSONデータ構造 (Gemini APIレスポンス用) ---

// Represents a candidate response from the Gemini API
[Serializable]
public class GeminiCandidate
{
    public Content content;
    public string finishReason;
    public List<SafetyRating> safetyRatings;
}

// Represents safety ratings
[Serializable]
public class SafetyRating
{
    public string category;
    public string probability;
}

// Represents the 'content' within a GeminiCandidate
[Serializable]
public class Content
{
    public List<Part> parts;
    public string role;
}

// Represents a single part within the 'parts' array of the response (typically text or inlineData)
[Serializable]
public class Part
{
    public string text;
    public GeminiInlineData inlineData;
}

// The complete JSON response received from the Gemini API
[Serializable]
public class GeminiResponse
{
    public List<GeminiCandidate> candidates;
    public PromptFeedback promptFeedback;
}

// Represents prompt feedback
[Serializable]
public class PromptFeedback
{
    public List<SafetyRating> safetyRatings;
}

// --- JSONデータ構造 (For Cleanliness Report Output) ---

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
/// based on an input image, and for generating a cleaned image.
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
    [Tooltip("The full API URL for the Gemini Vision model (e.g., https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent)")]
    [SerializeField]
    private string analysisApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";
    [Tooltip("The name of the Gemini model used for analysis (e.g., gemini-2.5-flash).")]
    [SerializeField]
    private string analysisModelName = "gemini-2.5-flash";

    [Header("Image Generation Model Settings")]
    [Tooltip("The full API URL for the Gemini image generation model (e.g., https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-preview-image-generation:generateContent)")]
    [SerializeField]
    private string imageGenerationApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-preview-image-generation:generateContent";
    [Tooltip("The name of the Gemini model used for image generation (e.g., gemini-2.0-flash-preview-image-generation).")]
    [SerializeField]
    private string imageGenerationModelName = "gemini-2.0-flash-preview-image-generation";


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
            Debug.Log("CleanlinessAnalyzer: ボタンクリックリスナーを追加しました。");
        }
        else
        {
            Debug.LogError("CleanlinessAnalyzer: testBtnが割り当てられていません！インスペクターでButtonコンポーネントを割り当ててください。");
        }

        string inputPath = Path.Combine(Application.dataPath, "InputImages");
        if (!Directory.Exists(inputPath))
        {
            Debug.LogWarning($"CleanlinessAnalyzer: 入力画像ディレクトリ '{inputPath}' が見つかりません。作成して画像ファイルを配置してください。");
        }

        // Validate API Key presence at startup
        if (string.IsNullOrEmpty(apiKey) || apiKey == "**YOUR_GEMINI_API_KEY**")
        {
            Debug.LogError("CleanlinessAnalyzer: Gemini APIキーが設定されていないか、まだプレースホルダーです。インスペクターでAPIキーを設定してください！");
        }
    }

    /// <summary>
    /// Called when the assigned UI button is clicked. Initiates the image analysis coroutine.
    /// </summary>
    private void OnTestButtonClick()
    {
        Debug.Log("CleanlinessAnalyzer: ボタンがクリックされました。画像分析と生成を開始します...");
        StartCoroutine(AnalyzeAndGenerateCoroutine());
    }

    /// <summary>
    /// Coroutine to orchestrate the entire image analysis and generation process.
    /// </summary>
    private IEnumerator AnalyzeAndGenerateCoroutine()
    {
        string imagePath = Path.Combine(Application.dataPath, "InputImages", imageFileName);

        if (!File.Exists(imagePath))
        {
            Debug.LogError($"CleanlinessAnalyzer: 画像ファイルが見つかりません: {imagePath}\n" +
                           "画像ファイルを 'Assets/InputImages' フォルダーに配置し、インスペクターで 'Image File Name' を正しく設定してください。");
            yield break;
        }

        Debug.Log($"CleanlinessAnalyzer: [処理開始] 画像: {Path.GetFileName(imagePath)}");

        CleanlinessReport report = null; // Variable to hold the parsed report

        // --- Step 1: Get Cleanliness Report (using gemini-2.5-flash) ---
        yield return StartCoroutine(GetCleanlinessReport(imagePath, (resultReport) => {
            report = resultReport;
        }));

        if (report == null)
        {
            Debug.LogError($"CleanlinessAnalyzer: [分析失敗] {Path.GetFileName(imagePath)}の有効なJSONレポートを取得できませんでした。以前のログでAPIエラーまたはパースの問題を確認してください。");
            yield break;
        }

        string outputPath = Path.Combine(Application.dataPath, "Output");
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
            Debug.Log($"CleanlinessAnalyzer: 出力ディレクトリが作成されました: {outputPath}");
        }

        string jsonReportContent = JsonUtility.ToJson(report, true);
        string outputJsonPath = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(imageFileName)}_report.json");
        File.WriteAllText(outputJsonPath, jsonReportContent);
        Debug.Log($"CleanlinessAnalyzer: [分析成功] 分析レポートが保存されました: {outputJsonPath}");

        // --- Step 2: Generate Cleaned Image (using gemini-2.0-flash-preview-image-generation) ---
        Debug.Log("CleanlinessAnalyzer: Geminiからきれいな画像の生成をリクエストしています...");

        // Construct a prompt for image generation based on the analysis report
        StringBuilder imageGenPromptBuilder = new StringBuilder();
        imageGenPromptBuilder.AppendLine("以下の清掃分析レポートに基づき、この部屋が**完璧に整理整頓され、清潔になった後の状態**を想像し、その画像を生成してください。");
        imageGenPromptBuilder.AppendLine("レポートで指摘された散らかりや不備はすべて修正され、部屋は明るく、整然としており、快適な空間になっているはずです。");
        imageGenPromptBuilder.AppendLine("詳細な指示としては：");
        imageGenPromptBuilder.AppendLine("- 散らかった物が取り除かれ、すべてが適切な場所に収納されていること。");
        imageGenPromptBuilder.AppendLine("- 表面には埃がなく、光沢があること。");
        imageGenPromptBuilder.AppendLine("- 家具や装飾品は美しく配置され、空間全体に調和が取れていること。");
        imageGenPromptBuilder.AppendLine("- 必要であれば、自然光が差し込み、部屋全体が明るく見えるようにしてください。");
        imageGenPromptBuilder.AppendLine("- 全体的に、清潔感があり、居心地の良い、完璧に整理された部屋のイメージを生成してください。");
        imageGenPromptBuilder.AppendLine("\n--- 清掃レポート ---");
        imageGenPromptBuilder.AppendLine($"全体的な清潔度スコア: {report.overall_cleanliness_score}/100");

        if (report.analysis_of_disorder != null && report.analysis_of_disorder.Any())
        {
            imageGenPromptBuilder.AppendLine("\n散らかり分析:");
            foreach (var item in report.analysis_of_disorder)
            {
                imageGenPromptBuilder.AppendLine($"- 場所: {item.location}, 詳細: {item.details}, カテゴリ: {item.category}");
            }
        }

        if (report.improvement_suggestions != null && report.improvement_suggestions.Any())
        {
            imageGenPromptBuilder.AppendLine("\n改善提案:");
            foreach (var item in report.improvement_suggestions)
            {
                imageGenPromptBuilder.AppendLine($"- 対象エリア: {item.target_area}, 提案: {item.suggestion}, 優先度: {item.priority}");
            }
        }
        imageGenPromptBuilder.AppendLine("\n--- レポートの終わり ---");
        imageGenPromptBuilder.AppendLine("\n元の画像を基に、このレポートの内容が完全に改善された状態の画像を生成してください。");

        string imageGenPrompt = imageGenPromptBuilder.ToString();
        Texture2D generatedTexture = null;

        yield return StartCoroutine(GenerateCleanedImage(imagePath, imageGenPrompt, (texture) => {
            generatedTexture = texture;
        }));

        if (generatedTexture != null)
        {
            byte[] imageBytes = generatedTexture.EncodeToPNG(); // PNGとして保存
            string outputImagePath = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(imageFileName)}_cleaned.png");
            File.WriteAllBytes(outputImagePath, imageBytes);
            Debug.Log($"CleanlinessAnalyzer: [画像生成成功] きれいな画像が保存されました: {outputImagePath}");
        }
        else
        {
            // This is the line that will be hit if generatedTexture is null
            Debug.LogError("CleanlinessAnalyzer: [画像生成失敗] きれいな画像を生成できませんでした。");
        }
    }

    /// <summary>
    /// Coroutine to send an HTTP POST request to the Gemini API for cleanliness analysis.
    /// This method constructs the JSON payload, sends the request, and parses the response.
    /// </summary>
    /// <param name="imagePath">The full path to the image file to analyze.</param>
    /// <param name="callback">A callback function to return the parsed CleanlinessReport result (null if failed).</param>
    private IEnumerator GetCleanlinessReport(string imagePath, System.Action<CleanlinessReport> callback)
    {
        byte[] imageBytes = File.ReadAllBytes(imagePath);
        string base64Image = Convert.ToBase64String(imageBytes);
        string mimeType = GetMimeType(imagePath);

        // Prompt for analysis
        string prompt = "以下の画像に写っている空間の整理整頓状況を分析し、100点満点で採点してください。" +
                        "散らかりの具体的な場所、詳細、カテゴリ（例: '散乱', '埃', '不整理'）を特定してください。" +
                        "次に、特定された各エリアに対する具体的な改善提案を、優先度（高, 中, 低）を付けて行ってください。" +
                        "最後に、分析結果を以下のJSON形式で出力してください。JSON全体が有効なオブジェクトであり、JSON以外のテキストを含まないようにしてください。" +
                        "JSONの構造は以下の通りです:\n" +
                        "```json\n" +
                        "{\n" +
                        "  \"overall_cleanliness_score\": int,\n" +
                        "  \"analysis_of_disorder\": [\n" +
                        "    {\n" +
                        "      \"location\": \"string\",\n" +
                        "      \"details\": \"string\",\n" +
                        "      \"category\": \"string\" \n" +
                        "    }\n" +
                        "  ],\n" +
                        "  \"improvement_suggestions\": [\n" +
                        "    {\n" +
                        "      \"target_area\": \"string\",\n" +
                        "      \"suggestion\": \"string\",\n" +
                        "      \"priority\": \"string\"\n" +
                        "    }\n" +
                        "  ]\n" +
                        "}\n" +
                        "```";

        string jsonPayload = "{"
            + "\"contents\":[{"
            + "\"role\":\"user\","
            + "\"parts\":["
            + "{\"text\":\"" + EscapeJson(prompt) + "\"},"
            + "{\"inline_data\":{\"mime_type\":\"" + mimeType + "\",\"data\":\"" + base64Image + "\"}}"
            + "]"
            + "}]}";

        using (UnityWebRequest webRequest = new UnityWebRequest($"{analysisApiUrl}?key={apiKey}", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("CleanlinessAnalyzer: 分析リクエストを送信中...");
            yield return webRequest.SendWebRequest();

            CleanlinessReport resultReport = null;

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"CleanlinessAnalyzer: [APIエラー - 分析] 失敗しました。コード: {webRequest.responseCode}\n{webRequest.downloadHandler.text}");
            }
            else
            {
                try
                {
                    string responseText = webRequest.downloadHandler.text;
                    Debug.Log("CleanlinessAnalyzer: 生の分析レスポンス: " + responseText);

                    GeminiResponse geminiResponse = JsonUtility.FromJson<GeminiResponse>(responseText);
                    string output = geminiResponse?.candidates?[0]?.content?.parts?[0]?.text;

                    if (!string.IsNullOrEmpty(output))
                    {
                        int braceStart = output.IndexOf('{');
                        int braceEnd = output.LastIndexOf('}');
                        if (braceStart != -1 && braceEnd > braceStart)
                        {
                            string jsonReportRaw = output.Substring(braceStart, braceEnd - braceStart + 1);
                            Debug.Log($"✅ 分析モデル出力からJSONを抽出しました:\n{jsonReportRaw}");

                            try
                            {
                                CleanlinessReport parsedReport = JsonUtility.FromJson<CleanlinessReport>(jsonReportRaw);
                                callback?.Invoke(parsedReport);
                                yield break;
                            }
                            catch (Exception parseEx)
                            {
                                Debug.LogError($"❌ モデル出力からCleanlinessReportをデシリアライズできませんでした: {parseEx.Message}\n生のJSON: {jsonReportRaw}");
                            }
                        }
                        else
                        {
                            Debug.LogError("❌ 分析レスポンスにJSON構造（中括弧）が見つかりませんでした。");
                        }
                    }
                    else
                    {
                        Debug.LogError("❌ Geminiが分析のために空のテキスト出力を返しました。");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[パースエラー - 分析] {e.Message}");
                }
            }

            callback?.Invoke(resultReport);
        }
    }

    /// <summary>
    /// Coroutine to generate a cleaned image using Gemini's image generation model.
    /// This will send the original image and a text prompt to the API.
    /// </summary>
    /// <param name="originalImagePath">The full path to the original image file.</param>
    /// <param name="generationPrompt">The text prompt describing the desired cleaned image.</param>
    /// <param name="callback">A callback function to return the generated Texture2D (null if failed).</param>
    private IEnumerator GenerateCleanedImage(string originalImagePath, string generationPrompt, System.Action<Texture2D> callback)
    {
        byte[] originalImageBytes = File.ReadAllBytes(originalImagePath);
        string base64OriginalImage = Convert.ToBase64String(originalImageBytes);
        string mimeType = GetMimeType(originalImagePath);

        // GenerationConfigをJsonUtilityでシリアライズするためのダミーオブジェクト
        // これを直接リクエストペイロードに埋め込みます
        GenerationConfig genConfig = new GenerationConfig
        {
            responseModalities = new List<string> { "IMAGE", "TEXT" } // 画像とテキストの両方をリクエスト
        };
        string genConfigJson = JsonUtility.ToJson(genConfig);

        // JSONペイロードを手動で構築（特に'parts'配列を正確に制御するため）
        string jsonPayload = "{"
                             + "\"contents\":[{"
                             + "\"role\":\"user\","
                             + "\"parts\":["
                             + "{\"text\":\"" + EscapeJson(generationPrompt) + "\"},"
                             + "{\"inline_data\":{\"mime_type\":\"" + mimeType + "\",\"data\":\"" + base64OriginalImage + "\"}}"
                             + "]"
                             + "}]," // ←ここ！カンマが必要なのは `contents` の直後だけ
                             + "\"generationConfig\":" + genConfigJson
                             + "}";


        Debug.Log("CleanlinessAnalyzer: 画像生成リクエストペイロード（最初の500文字）: " + jsonPayload.Substring(0, Mathf.Min(jsonPayload.Length, 500)) + "...");

        using (UnityWebRequest webRequest = new UnityWebRequest($"{imageGenerationApiUrl}?key={apiKey}", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("CleanlinessAnalyzer: 画像生成リクエストを " + imageGenerationApiUrl + " に送信中...");
            yield return webRequest.SendWebRequest();

            Texture2D generatedTexture = null;

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"CleanlinessAnalyzer: [APIエラー - 画像生成] 失敗しました。コード: {webRequest.responseCode}\nエラー: {webRequest.error}\nレスポンス: {webRequest.downloadHandler.text}");
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;
                Debug.Log("CleanlinessAnalyzer: 生の画像生成レスポンス（フル）: " + responseText);

                try
                {
                    GeminiResponse geminiResponse = JsonUtility.FromJson<GeminiResponse>(responseText);
                    Debug.Log($"CleanlinessAnalyzer: GeminiResponseをパースしました。候補の数: {geminiResponse?.candidates?.Count ?? 0}");

                    // Check for safety ratings or prompt feedback
                    if (geminiResponse.promptFeedback != null && geminiResponse.promptFeedback.safetyRatings != null && geminiResponse.promptFeedback.safetyRatings.Any())
                    {
                        foreach (var sr in geminiResponse.promptFeedback.safetyRatings)
                        {
                            if (sr.probability != "NEGLIGIBLE")
                            {
                                Debug.LogWarning($"CleanlinessAnalyzer: プロンプト安全フィードバック: カテゴリ='{sr.category}', 確率='{sr.probability}'。生成がブロックされた可能性があります。");
                            }
                        }
                    }

                    if (geminiResponse?.candidates != null && geminiResponse.candidates.Count > 0)
                    {
                        Debug.Log($"CleanlinessAnalyzer: 最初の候補のロール: {geminiResponse.candidates[0].content?.role ?? "N/A"}");
                        if (geminiResponse.candidates[0].content?.parts != null && geminiResponse.candidates[0].content.parts.Count > 0)
                        {
                            Debug.Log($"CleanlinessAnalyzer: 最初の候補のパーツ数: {geminiResponse.candidates[0].content.parts.Count}");

                            foreach (var part in geminiResponse.candidates[0].content.parts)
                            {
                                if (part.inlineData != null && !string.IsNullOrEmpty(part.inlineData.data))
                                {
                                    Debug.Log($"CleanlinessAnalyzer: inlineDataが見つかりました。Mimeタイプ: {part.inlineData.mime_type}, データ長: {part.inlineData.data.Length}文字。");
                                    
                                    try
                                    {
                                        byte[] imageBytes = Convert.FromBase64String(part.inlineData.data);
                                        Debug.Log($"CleanlinessAnalyzer: Base64画像データをデコードしました。バイト長: {imageBytes.Length}。");

                                        generatedTexture = new Texture2D(2, 2);
                                        if (generatedTexture.LoadImage(imageBytes))
                                        {
                                            Debug.Log("CleanlinessAnalyzer: 生成された画像データをTexture2Dに正常にロードしました。");
                                            break;
                                        }
                                        else
                                        {
                                            Debug.LogError("CleanlinessAnalyzer: 画像データをTexture2Dにロードできませんでした。imageBytesが破損しているか、有効な画像形式ではありません。");
                                            generatedTexture = null;
                                        }
                                    }
                                    catch (FormatException fe)
                                    {
                                        Debug.LogError($"CleanlinessAnalyzer: Base64デコードに失敗しました: {fe.Message}。データが不正な形式かもしれません。最初の50文字: {part.inlineData.data.Substring(0, Mathf.Min(part.inlineData.data.Length, 50))}");
                                    }
                                    catch (Exception imageLoadEx)
                                    {
                                        Debug.LogError($"CleanlinessAnalyzer: Texture2D.LoadImage中の例外: {imageLoadEx.Message}");
                                    }
                                }
                                else if (part.text != null)
                                {
                                    Debug.Log($"CleanlinessAnalyzer: 画像生成レスポンスでテキストパートが見つかりました: {part.text.Substring(0, Mathf.Min(part.text.Length, 200))}...");
                                }
                            }

                            if (generatedTexture == null)
                            {
                                Debug.LogError("CleanlinessAnalyzer: API呼び出しと候補/パーツのパースが成功したにもかかわらず、画像生成レスポンスに有効なinlineData（画像データ）が含まれていませんでした。");
                            }
                        }
                        else
                        {
                            Debug.LogError("CleanlinessAnalyzer: 最初の候補のコンテンツパーツ配列が空かnullです。モデルが指定されたプロンプトに対して何も生成しなかった可能性があります。");
                        }
                    }
                    else
                    {
                        Debug.LogError("CleanlinessAnalyzer: GeminiResponseに候補がありません。これは通常、プロンプトの安全性に関する問題、またはモデルがコンテンツを生成できないことを示します。");
                        if (geminiResponse.promptFeedback != null && geminiResponse.promptFeedback.safetyRatings != null && geminiResponse.promptFeedback.safetyRatings.Any(sr => sr.probability != "NEGLIGIBLE"))
                        {
                             Debug.LogError("CleanlinessAnalyzer: promptFeedbackの安全評価を確認してください。プロンプトの安全性の懸念により生成がブロックされた可能性があります。");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[パースエラー - 画像生成] JSONパースまたはテクスチャ処理中に例外が発生しました: {e.Message}\nスタックトレース: {e.StackTrace}");
                }
            }
            callback?.Invoke(generatedTexture);
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
            case ".webp": return "image/webp";
            default:
                Debug.LogWarning($"CleanlinessAnalyzer: 不明な画像ファイル拡張子 '{ext}'。'application/octet-stream'にデフォルト設定されます。" +
                                 "実際の画像タイプが正しく推測されない場合、Gemini APIで問題が発生する可能性があります。");
                return "application/octet-stream";
        }
    }
}
