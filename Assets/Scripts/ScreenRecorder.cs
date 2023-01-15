using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using UnityEngine;
using System.Text;
using System.Linq;

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

    private const int BUFFERSIZE = 1920*1080 * 4;

    IEnumerator CaptureScreen()
    {
        yield return new WaitForEndOfFrame();
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] data = texture.GetRawTextureData();
        using (NamedPipeServerStream pipeServer = 
            new NamedPipeServerStream("test111sb", PipeDirection.Out))
        {
            pipeServer.WaitForConnection();
            print("pipe started");
            using (BinaryWriter bw = new BinaryWriter(pipeServer))
            {
                do
                {
                    yield return new WaitForEndOfFrame();
                    texture.Reinitialize(0, 0);
                    texture = ScreenCapture.CaptureScreenshotAsTexture();
/*                    int width = texture.width;
                    int height = texture.height;*/
                    
                    data = texture.GetRawTextureData();

                    bw.Write(data,0,data.Length);
                    bw.Flush();
                    //Thread.Sleep(100);
                }
                while (pipeServer.IsConnected);
            }
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
