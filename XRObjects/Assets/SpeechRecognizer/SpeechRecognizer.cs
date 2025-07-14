using UnityEngine;
using UnityEngine.UI;
using static SpeechRecognizerPlugin;

using TMPro;

using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class Translation
{
    public string language;
    public TranslationText text;
}

[System.Serializable]
public class TranslationText
{
    public string startListening;
    public string stopListening;
    public string continuousListening;
    public string language;
    public string maxResults;
    public string resultsTitle;
    public string errorsTitle;
    public string boundingBoxOn;
    public string boundingBoxOff;
    public string DetectionOn;
    public string DetectionOff;
    public string planesShown;
    public string planesHidden;
}

[System.Serializable]
public class TranslationData
{
    public Translation[] translations;
}

public class SpeechRecognizer : MonoBehaviour, ISpeechRecognizerPlugin
{
  [SerializeField] private Button startListeningBtn = null;
  [SerializeField] private Button stopListeningBtn = null;
  [SerializeField] private Toggle continuousListeningTgle = null;
  [SerializeField] private TMP_Dropdown languageDropdown = null;
  [SerializeField] private TMP_InputField maxResultsInputField = null;
  [SerializeField] private TextMeshProUGUI resultsTxt = null;
  [SerializeField] private TextMeshProUGUI speechTranscribedTextDisplay = null;
  [SerializeField] private TextMeshProUGUI errorsTxt = null;

  // Labels for translation
  [SerializeField] private TextMeshProUGUI continuousListeningLabel = null;
  [SerializeField] private TextMeshProUGUI languageLabel = null;
  [SerializeField] private TextMeshProUGUI maxResultsLabel = null;
  [SerializeField] private TextMeshProUGUI resultsTitleLabel = null;
  [SerializeField] private TextMeshProUGUI errorsTitleLabel = null;

  private SpeechRecognizerPlugin plugin = null;
  private TranslationData translationData;

  private GameObject requestingGameObject;
  private ActionClass requestingActionClass;
  private string requestingFunctionName;

  // to show the top-ranking transcript the the user
  private string speechTranscribedText;

  private void Start()
  {
    plugin = SpeechRecognizerPlugin.GetPlatformPluginVersion(this.gameObject.name);

    startListeningBtn.onClick.AddListener(StartListening);
    stopListeningBtn.onClick.AddListener(StopListening);
    continuousListeningTgle.onValueChanged.AddListener(SetContinuousListening);
    languageDropdown.onValueChanged.AddListener(SetLanguage);
    maxResultsInputField.onEndEdit.AddListener(SetMaxResults);

    // Load translations
    string path = Path.Combine(Application.streamingAssetsPath, "translations.json");
    string jsonString = File.ReadAllText(path, System.Text.Encoding.UTF8);
    translationData = JsonUtility.FromJson<TranslationData>(jsonString);

    // Populate dropdown
    languageDropdown.ClearOptions();
    languageDropdown.AddOptions(translationData.translations.Select(t => new TMP_Dropdown.OptionData(t.language)).ToList());

    // Set initial language
    SetLanguage(languageDropdown.value);
  }

  public void StartListening()
  {
    plugin.StartListening();
  }

  // public void StartListeningAndDisplay(TextMeshProUGUI displayPanel, GameObject callingGameObject)
//   public void StartListeningAndDisplay(TextMeshProUGUI displayPanel, GameObject callingGameObject)
  public void StartListeningAndDisplay(TextMeshProUGUI displayPanel, ActionClass callingActionClass)
  {
    speechTranscribedTextDisplay = displayPanel;
    plugin.StartListening();

    // detect who called this function
    var requestingModuleName = new StackFrame(1).GetMethod().Module.Name;
    UnityEngine.Debug.Log("requestingModuleName: " + requestingModuleName);

    requestingFunctionName = new StackFrame(1).GetMethod().Name;
    UnityEngine.Debug.Log("requestingFunctionName: " + requestingFunctionName);

    requestingActionClass = callingActionClass;

    // requestingGameObject = callingGameObject;


  }

  private void StopListening()
  {
    plugin.StopListening();
  }

  private void SetContinuousListening(bool isContinuous)
  {
    plugin.SetContinuousListening(isContinuous);
  }

  private void SetLanguage(int dropdownValue)
  {
    string newLanguage = languageDropdown.options[dropdownValue].text;
    plugin.SetLanguageForNextRecognition(newLanguage);

    // Find the right translation
    Translation translation = System.Array.Find(translationData.translations, t => t.language == newLanguage);

    if (translation != null)
    {
        // Update UI Texts
        startListeningBtn.GetComponentInChildren<TextMeshProUGUI>().text = translation.text.startListening;
        stopListeningBtn.GetComponentInChildren<TextMeshProUGUI>().text = translation.text.stopListening;
        
        if(continuousListeningLabel != null)
            continuousListeningLabel.text = translation.text.continuousListening;
        
        if(languageLabel != null)
            languageLabel.text = translation.text.language;

        if(maxResultsLabel != null)
            maxResultsLabel.text = translation.text.maxResults;

        if(resultsTitleLabel != null)
            resultsTitleLabel.text = translation.text.resultsTitle;
        
        if(errorsTitleLabel != null)
            errorsTitleLabel.text = translation.text.errorsTitle;
    }
  }

  private void SetMaxResults(string inputValue)
  {
    if (string.IsNullOrEmpty(inputValue))
      return;

    int maxResults = int.Parse(inputValue);
    plugin.SetMaxResultsForNextRecognition(maxResults);
  }

  public void OnResult(string recognizedResult)
  {
    char[] delimiterChars = { '~' };
    string[] result = recognizedResult.Split(delimiterChars);

    resultsTxt.text = "";
    for (int i = 0; i < result.Length; i++)
    {
      resultsTxt.text += result[i] + '\n';
    }

    //

    // then pass result[0] to the requesting entity
    speechTranscribedText = result[0];
    // speechTranscribedTextDisplay.text = “<b>" + speechTranscribedText + "?</b>”\n\nthinking...";


    // (requestingGameObject.GetComponent(requestingFunctionName) as ActionClass).onTranscriptionFinished(speechTranscribedText);

    requestingActionClass.onTranscriptionFinished(speechTranscribedText);

    // and then stop listening
    // plugin.StopListening();

  }

  public void OnError(string recognizedError)
  {
    ERROR error = (ERROR)int.Parse(recognizedError);
    switch (error)
    {
      case ERROR.UNKNOWN:
        UnityEngine.Debug.Log("<b>ERROR: </b> Unknown");
        errorsTxt.text += "Unknown";
        break;
      case ERROR.INVALID_LANGUAGE_FORMAT:
        UnityEngine.Debug.Log("<b>ERROR: </b> Language format is not valid");
        errorsTxt.text += "Language format is not valid";
        break;
      default:
        break;
    }
  }
}

