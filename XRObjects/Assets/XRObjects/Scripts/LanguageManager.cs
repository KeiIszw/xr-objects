using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

[System.Serializable]
public class LanguageData
{
    public string startListening;
    public string stopListening;
    public string continuousListening;
    public string language;
    public string maxResults;
    // public string resultsTitle;
    // public string errorsTitle;
    public string boundingBoxOn;
    public string boundingBoxOff;
    public string detectionOn;
    public string detectionOff;
    public string planesShown;
    public string planesHidden;
    public string history;
    public string deleteObjects;
    public string speechMenu;
    public string before;
    public string after;
    public string analysis;
    public string suggestions;
}

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }

    public TMP_Dropdown languageDropdown;

    private LanguageData english;
    private LanguageData japanese;
    private LanguageData currentLanguage;

    private List<string> languageList = new List<string> { "English", "日本語" };

    public event System.Action OnLanguageChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadLanguages();
    }

    void Start()
    {
        SetupDropdown();
    }

    void LoadLanguages()
    {
        TextAsset englishJson = Resources.Load<TextAsset>("english");
        TextAsset japaneseJson = Resources.Load<TextAsset>("japanese");

        if (englishJson != null)
        {
            // string dataAsJson = File.ReadAllText(englishPath);
            english = JsonUtility.FromJson<LanguageData>(englishJson.text);
        }
        else
        {
            Debug.LogError("Cannot find " + englishJson);
        }

        if (japaneseJson != null)
        {
            // string dataAsJson = File.ReadAllText(japanesePath);
            japanese = JsonUtility.FromJson<LanguageData>(japaneseJson.text);
        }
        else
        {
            Debug.LogError("Cannot find " + japaneseJson);
        }

        currentLanguage = english;
    }

    void SetupDropdown()
    {
        if (languageDropdown != null)
        {
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(languageList);
            languageDropdown.onValueChanged.AddListener(delegate
            {
                ChangeLanguage(languageDropdown.value);
            });
        }
    }

    public void ChangeLanguage(int index)
    {
        if (index == 0)
        {
            currentLanguage = english;
        }
        else if (index == 1)
        {
            currentLanguage = japanese;
        }

        OnLanguageChanged.Invoke();
    }

    public string GetText(string key)
    {
        if (currentLanguage == null) return "";

        switch (key)
        {
            case "startListening": return currentLanguage.startListening;
            case "stopListening": return currentLanguage.stopListening;
            case "continuousListening": return currentLanguage.continuousListening;
            case "language": return currentLanguage.language;
            case "maxResults": return currentLanguage.maxResults;
            // case "resultsTitle": return currentLanguage.resultsTitle;
            // case "errorsTitle": return currentLanguage.errorsTitle;
            case "boundingBoxOn": return currentLanguage.boundingBoxOn;
            case "boundingBoxOff": return currentLanguage.boundingBoxOff;
            case "detectionOn": return currentLanguage.detectionOn;
            case "detectionOff": return currentLanguage.detectionOff;
            case "planesShown": return currentLanguage.planesShown;
            case "planesHidden": return currentLanguage.planesHidden;
            case "history": return currentLanguage.history;
            case "deleteObjects": return currentLanguage.deleteObjects;
            case "speechMenu": return currentLanguage.speechMenu;
            case "before": return currentLanguage.before;
            case "after": return currentLanguage.after;
            case "analysis": return currentLanguage.analysis;
            case "suggestions": return currentLanguage.suggestions;
            default: return "not found";
        }
    }
}
