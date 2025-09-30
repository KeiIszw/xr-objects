using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidPermissionHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!RuntimeAndroidSettingHelper.HasUserAuthorizedPermission())
        {
            RuntimeAndroidSettingHelper.Request_SettingsIntent();
        }
    }
}
