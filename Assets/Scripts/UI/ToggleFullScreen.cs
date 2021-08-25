using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ToggleFullScreen : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer sr;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    private void OnMouseEnter()
    {
        sr.color = Color.gray;
    }
    private void OnMouseExit()
    {
        sr.color = new Color(1f, 1f, 1f, 0f);
    }
    private void OnMouseDown()
    {
        sr.color = Color.white;
        ToggleFullscreen();
    }
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
