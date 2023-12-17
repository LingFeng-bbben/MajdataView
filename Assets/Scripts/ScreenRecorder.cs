using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.UI;

public class ScreenRecorder : MonoBehaviour
{
    public float CutoffTime;


    private bool isRecording;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void StartRecording(string maidata_path)
    {
        StartCoroutine(CaptureScreen(maidata_path));
    }

    public void StopRecording()
    {
        print("stop recording");
        isRecording = false;
    }

    private IEnumerator CaptureScreen(string maidata_path)
    {
        var timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        var bgManager = GameObject.Find("Background").GetComponent<BGManager>();
        if (Screen.width % 2 != 0 || Screen.height % 2 != 0)
        {
            GameObject.Find("ErrText").GetComponent<Text>().text =
                "无法开始编码，因为分辨率宽度或高度不是偶数。\nCan not start render because the width/height is not even.\n当前分辨率:" +
                Screen.width + "x" + Screen.height + "\n";
            yield break;
        }

        if (File.Exists(maidata_path + "\\out.mp4"))
            File.Delete(maidata_path + "\\out.mp4");

        byte[] data;
        var texture = new Texture2D(0, 0);
        using (var pipeServer =
               new NamedPipeServerStream("majdataRec", PipeDirection.Out))
        {
            var wavpath = "out.wav";
            var outputfile = "out.mp4";

            var arguments = string.Format(
                File.ReadAllText(Application.streamingAssetsPath + "\\ffarguments.txt").Trim(),
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
            using (var bw = new BinaryWriter(pipeServer))
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
                    catch
                    {
                    }
                } while (
                    pipeServer.IsConnected &&
                    isRecording &&
                    !p.HasExited
                );
            }

            p.WaitForExit();

            if (File.Exists(maidata_path + "/out.mp4") && p.ExitCode == 0)
            {
                GameObject.Find("ErrText").GetComponent<Text>().text += "渲染成功，视频生成在" + maidata_path +
                                                                        "\\out.mp4\nRender Successed\nExitCode:" +
                                                                        p.ExitCode;
                Process.Start("explorer", "/select,\"" + maidata_path + "\\out.mp4" + "\"");
            }
            else
            {
                GameObject.Find("ErrText").GetComponent<Text>().text +=
                    "编码器已退出\nFFmpeg Exited.\nExitCode:" + p.ExitCode;
            }
        }

        timeProvider.isStart = false;
        bgManager.PauseVideo();
    }
}