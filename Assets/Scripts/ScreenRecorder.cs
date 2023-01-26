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
using UnityEngine.UI;

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

   
    bool isRecording = false;
    IEnumerator CaptureScreen(string maidata_path)
    {
        if (Screen.width % 2 != 0)
        {
            GameObject.Find("ErrText").GetComponent<Text>().text = "无法开始编码，因为分辨率宽度不是偶数。\n请调到全屏模式或改变窗口大小来尝试解决该问题\n当前:"+Screen.width+"x"+Screen.height;
        }
        if (File.Exists(maidata_path + "/out.mp4"))
            File.Delete(maidata_path + "/out.mp4");
        byte[] data;
        var texture = new Texture2D(0,0);
        using (NamedPipeServerStream pipeServer = 
            new NamedPipeServerStream("majdataRec", PipeDirection.Out))
        {
            var wavpath = "out.wav";
            var outputfile = "out.mp4";
            var arguments = string.Format(@"-y -f rawvideo -vcodec rawvideo -pix_fmt rgba -s {3}x{4} -r 60 -i \\.\pipe\majdataRec -i {0} -vf {1} -c:v libx264 -preset fast -pix_fmt yuv420p -c:a aac {2}",
                wavpath, "\"vflip\"", outputfile , Screen.width, Screen.height);
            var startinfo = new ProcessStartInfo(Application.streamingAssetsPath + "/ffmpeg.exe", arguments);
            startinfo.WorkingDirectory = maidata_path;
            print(arguments);
            
            var p = Process.Start(startinfo);
            pipeServer.WaitForConnection();
            isRecording = true;
            using (BinaryWriter bw = new BinaryWriter(pipeServer))
            {
                do
                {
                    yield return new WaitForEndOfFrame();
                    try
                    {
                        texture.Reinitialize(0, 0);
                        texture = ScreenCapture.CaptureScreenshotAsTexture();
                        /*                    int width = texture.width;
                                            int height = texture.height;*/

                        data = texture.GetRawTextureData();

                        bw.Write(data, 0, data.Length);
                        bw.Flush();
                        //Thread.Sleep(100);
                    }
                    catch { }
                }
                while (pipeServer.IsConnected && isRecording && !p.HasExited);
            }
            p.WaitForExit();
            
            if(File.Exists(maidata_path + "/out.mp4"))
                GameObject.Find("ErrText").GetComponent<Text>().text = "编码器已退出，视频生成在铺面目录out.mp4\n"+ p.ExitCode;
            else
                GameObject.Find("ErrText").GetComponent<Text>().text = "编码器已退出\n" + p.ExitCode;
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
