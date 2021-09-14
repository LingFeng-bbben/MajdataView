using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ToggleFullScreen : MonoBehaviour
{
    public void ToggleFullscreen()
    {
        print("ToggleFullScreen");
        var resolutions = Screen.resolutions;
        if (Screen.fullScreen)
        {
            var width = 300;
            var height = 300;
            Screen.SetResolution(width, height, false);
        }
        else
        {
            Screen.SetResolution(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height, true);
        }
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleFullscreen();
        }
    }
}
