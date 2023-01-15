using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRecorder : MonoBehaviour
{
    public void StartRecording()
    {
        StartCoroutine(CaptureScreen());
    }

    public void StopRecording()
    {
        StopCoroutine(CaptureScreen());
    }

    IEnumerator CaptureScreen()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            print(texture.format);
            //texture.GetRawTextureData
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
