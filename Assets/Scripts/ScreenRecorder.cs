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
    public float CutoffTime;
    
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
        var timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        var bgManager = GameObject.Find("Background").GetComponent<BGManager>();
        if (Screen.width % 2 != 0 || Screen.height % 2 != 0)
        {
            GameObject.Find("ErrText").GetComponent<Text>().text = "�޷���ʼ���룬��Ϊ�ֱ��ʿ�Ȼ�߶Ȳ���ż����\nCan not start render because the width/height is not even.\n��ǰ�ֱ���:"+Screen.width+"x"+Screen.height+"\n";
            yield break;
        }
        if (File.Exists(maidata_path + "\\out.mp4"))
            File.Delete(maidata_path + "\\out.mp4");
        
        byte[] data;
        var texture = new Texture2D(0,0);
        using (NamedPipeServerStream pipeServer = 
            new NamedPipeServerStream("majdataRec", PipeDirection.Out))
        {
            var wavpath = "out.wav";
            var outputfile = "out.mp4";
            
            var arguments = string.Format(
              File.ReadAllText(Application.streamingAssetsPath+ "\\ffarguments.txt").Trim(),
                Screen.width, Screen.height,
                wavpath, outputfile,
                CutoffTime
              );
            var startinfo = new ProcessStartInfo(Application.streamingAssetsPath + "\\ffmpeg.exe", arguments);
            startinfo.UseShellExecute = false;
            startinfo.CreateNoWindow = true;
            startinfo.WorkingDirectory = maidata_path;
            startinfo.EnvironmentVariables.Add("FFREPORT", "file=out.log:level=24");
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
                while (
                  pipeServer.IsConnected &&
                  isRecording &&
                  !p.HasExited
                );
            }
            p.WaitForExit();

            if (File.Exists(maidata_path + "/out.mp4") && p.ExitCode == 0)
            {
                GameObject.Find("ErrText").GetComponent<Text>().text += "��Ⱦ�ɹ�����Ƶ������" + maidata_path + "\\out.mp4\nRender Successed\nExitCode:" + p.ExitCode;
                Process.Start("explorer", "/select,\"" + maidata_path + "\\out.mp4" + "\"");
            }
            else
                GameObject.Find("ErrText").GetComponent<Text>().text += "���������˳�\nFFmpeg Exited.\nExitCode:" + p.ExitCode;
        }
        timeProvider.isStart = false;
        bgManager.PauseVideo();
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
