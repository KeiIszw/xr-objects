// Copyright 2024 Google LLC

// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

// Class for dummy control of physical gadgets

public class ActionControl : MonoBehaviour
{

  public GameObject controlMenu;
  private Button actionButton;
  private bool menuIsVisible = false;

  //  Awake function is called on all objects in the Scene before any object's Start function is called
  void Awake()
  {
    // controlMenu = GetComponentInChildren<GridLayoutGroup>(true).gameObject;
    actionButton = gameObject.GetComponent<Button>();
    actionButton.onClick.AddListener(toggleMenuVisibility);
    controlMenu.SetActive(false);

  }
  void OnEnable(){

    // recalculate menu button layout so they shift to include the "control" button
    var radialLayout = FindParentWithTag(gameObject, "panelCircular").GetComponent<RadialLayout>();
    radialLayout.CalculateRadial();

  }

  void onDisable(){

    // recalculate menu button layout so they shift to exclude the "control" button
    var radialLayout = FindParentWithTag(gameObject, "panelCircular").GetComponent<RadialLayout>();
    radialLayout.CalculateRadial();

  }

  public void toggleMenuVisibility()
  {
    menuIsVisible = !controlMenu.activeSelf;
    // menuIsVisible = !menuIsVisible;

    if (menuIsVisible)
    {
      // make the main action button go to front for visibility
      // puts to the front using SetAsLastSibling as it is now the last UI element to be drawn.
      GameObject _mainActionButton = FindParentWithTag(controlMenu, "mainActionButton");
      _mainActionButton.transform.SetAsLastSibling();

      // close the panels of other "mainActionButton"s
      GameObject[] mainActionButtons = GameObject.FindGameObjectsWithTag("mainActionButton");

      foreach (GameObject mainActionButton in mainActionButtons)
      {
        if (mainActionButton!=_mainActionButton)
        {
          // close its panel
          try
          {
            mainActionButton.GetComponentInChildren<GridLayoutGroup>().gameObject.SetActive(false);
          }
          catch
          {
            Debug.Log("this action doesn't have submenus");
          }

        }
      }

      // show my own submenu
      controlMenu.SetActive(true);

      // but close all of my own children submenus
      // Component[] GridLayoutGroups = controlMenu.GetComponentsInChildren<GridLayoutGroup>().Skip(1).ToArray(); // exclude the object itself

      // foreach (Component gridLayoutGroup in GridLayoutGroups)
      // {
      //   gridLayoutGroup.gameObject.SetActive(false);
      // }

    }
    else
    {
      controlMenu.SetActive(false);
    }
  }

  private GameObject FindParentWithTag(GameObject child, string tag)
  {
    Transform childTransform = child.transform;

    while (childTransform.parent != null)
    {
      if (childTransform.parent.tag == tag)
      {
        // the parent at hand has the correct tag
        return childTransform.parent.gameObject;
      }
      childTransform = childTransform.parent.transform;
    }

    return null;
  }


}
