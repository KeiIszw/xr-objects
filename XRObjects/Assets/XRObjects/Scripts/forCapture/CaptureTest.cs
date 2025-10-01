using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureTest : MonoBehaviour
{
    public Button captureButton;

    // Start is called before the first frame update
    void Start()
    {
        captureButton.onClick.AddListener(OnCaptureButtonClicked);
    }

    private void OnCaptureButtonClicked()
    {
        CameraCapturer cameraCapturer = new CameraCapturer();
        cameraCapturer.CaptureAndSave();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
