using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using UnityEngine;
using System.Text;
using System.Linq;
using System.Diagnostics;

public class ScreenRecorder : MonoBehaviour
{
    public void StartRecording(string maidata_path)
    {
        StartCoroutine(CaptureScreen(maidata_path));
    }

    public void StopRecording()
    {
        print("stop recording");
        isRecording = false;
    }

    private const int BUFFERSIZE = 1920*1080 * 4;
    bool isRecording = false;
    IEnumerator CaptureScreen(string maidata_path)
    {
        yield return new WaitForEndOfFrame();
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] data = texture.GetRawTextureData();
        using (NamedPipeServerStream pipeServer = 
            new NamedPipeServerStream("majdataRec", PipeDirection.Out))
        {
            var wavpath = maidata_path + "/out.wav";
            var outputfile = maidata_path + "/out.mp4";
            var arguments = string.Format(@"-y -f rawvideo -vcodec rawvideo -pix_fmt rgba -s 1920x1080 -r 60 -i \\.\pipe\majdataRec -i {0} -vf {1} -c:v libx264 -preset fast -pix_fmt yuv422p -c:a aac {2}", wavpath, "\"vflip\"", outputfile);
            var startinfo = new ProcessStartInfo(Application.streamingAssetsPath + "/ffmpeg.exe", arguments);
            Process.Start(startinfo);
            pipeServer.WaitForConnection();
            print("pipe started");
            isRecording = true;
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
                while (pipeServer.IsConnected && isRecording);
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
