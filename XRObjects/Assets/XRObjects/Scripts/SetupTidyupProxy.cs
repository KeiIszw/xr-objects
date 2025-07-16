using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SetupTidyupProxy : MonoBehaviour
{
    private bool objectIsSelected = false;
    public Material Material0; // object not selected
    public Material Material1; // object metadata available
    public Material Material2; // object selected

    public GameObject metadataMenu;

    private GameObject mainObjectProxy;
    public GameObject sphere;
    public string objectName;
    public string tidyupPoint;

    // Start is called before the first frame update
    void Start()
    {
        // turn off metadata menu
        metadataMenu.GetComponent<Canvas>().enabled = true;

        // set the object name to the current time
        System.DateTime now = System.DateTime.Now;
        objectName = now.ToString("HH:mm:ss");

        // set the tidyup point
        tidyupPoint = "Point"; // 仮

        // set the metadata
        string metadata = tidyupPoint + "\n" + objectName;

        metadataMenu.GetComponentInChildren<TextMeshProUGUI>().text = metadata;
    }

    // Update is called once per frame
    void Update()
    {
        // check if touching the object proxy
        // if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        // check if a touch exists an it's not an a UI element (e.g. button)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !IsPointerOverUIObject())
        {

            // RaycastHit _raycastHit;
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);


            RaycastHit[] _raycastHits;
            _raycastHits = Physics.RaycastAll(raycast, Mathf.Infinity);
            // sort by distance to hit the closest one
            System.Array.Sort(_raycastHits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var _raycastHit in _raycastHits)
            {

                // check if the sphere has been touched
                if (_raycastHit.collider.gameObject == sphere)
                { //_raycastHit.collider.CompareTag("RealObjectSphere") &&
                  // object has been touched, let's change material (color)

                    if (objectIsSelected)
                    {
                        // object was selected previously, let's deselect
                        //transform.Find("Sphere").GetComponent<MeshRenderer>().material = Material0;
                        _raycastHit.collider.GetComponent<MeshRenderer>().material = Material0;
                        metadataMenu.GetComponent<Canvas>().enabled = false;

                        objectIsSelected = false;
                    }
                    else
                    {
                        // object wasn't selected previously, let's select it now
                        //transform.Find("Sphere").GetComponent<MeshRenderer>().material = Material2;
                        _raycastHit.collider.GetComponent<MeshRenderer>().material = Material2;
                        metadataMenu.GetComponent<Canvas>().enabled = true;

                        objectIsSelected = true;
                    }
                }
            }


        }
    }

    public void deselectObject()
    {
        sphere.GetComponent<MeshRenderer>().material = Material0;
        metadataMenu.GetComponent<Canvas>().enabled = false;

        objectIsSelected = false;

    }

    private bool IsPointerOverUIObject()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        for (int touchIndex = 0; touchIndex < Input.touchCount; touchIndex++)
        {
            Touch touch = Input.GetTouch(touchIndex);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return true;
        }

        return false;
    }
}
