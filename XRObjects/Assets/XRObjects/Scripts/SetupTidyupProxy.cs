using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupTidyupProxy : MonoBehaviour
{
    private bool objectIsSelected = false;
    public Material Material0; // object not selected
    public Material Material1; // object metadata available
    public Material Material2; // object selected

    public GameObject rectMenu, metadataMenu, circularMenu, panelInfoDisplay;

    private GameObject mainObjectProxy, circularPanel;
    public GameObject sphere;

    public string objectTitle;
    public string metadata; //仮
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
