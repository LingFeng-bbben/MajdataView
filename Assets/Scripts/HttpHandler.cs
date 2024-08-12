using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Assets.Scripts.Types;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HttpHandler : MonoBehaviour
{
    public static bool IsReloding { get; set; } = false;
    private readonly HttpListener http = new();
    private Task listen;
    private string request = "";


    private void Start()
    {
        SceneManager.LoadScene(1);
        http.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        http.Prefixes.Add("http://localhost:8013/");
        http.Start();
        listen = new Task(httpListen);
        listen.Start();
        print("server started");
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(request)) return;

        IsReloding = false;
        var data = JsonConvert.DeserializeObject<EditRequestjson>(request);
        request = string.Empty;

        var loader = GameObject.Find("DataLoader").GetComponent<JsonDataLoader>();
        var timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        var bgManager = GameObject.Find("Background").GetComponent<BGManager>();
        var bgCover = GameObject.Find("BackgroundCover").GetComponent<SpriteRenderer>();
        var screenRecorder = GameObject.Find("ScreenRecorder").GetComponent<ScreenRecorder>();
        var multTouchHandler = GameObject.Find("MultTouchHandler").GetComponent<MultTouchHandler>();
        var objectCounter = GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>();

        InputManager.Mode = (AutoPlayMode)(int)data.editorPlayMethod;

        switch(data.control)
        {
            case EditorControlMethod.Start:
                {
                    timeProvider.SetStartTime(data.startAt, data.startTime, data.audioSpeed);
                    loader.noteSpeed = (float)(107.25 / (71.4184491 * Mathf.Pow(data.noteSpeed + 0.9975f, -0.985558604f)));
                    loader.touchSpeed = data.touchSpeed;
                    loader.smoothSlideAnime = data.smoothSlideAnime;
                    objectCounter.ComboSetActive(data.comboStatusType);
                    loader.LoadJson(File.ReadAllText(data.jsonPath), data.startTime);
                    GameObject.Find("Notes").GetComponent<PlayAllPerfect>().enabled = false;
                    GameObject.Find("MultTouchHandler").GetComponent<MultTouchHandler>().clearSlots();

                    bgManager.LoadBGFromPath(new FileInfo(data.jsonPath).DirectoryName, data.audioSpeed);
                    bgCover.color = new Color(0f, 0f, 0f, data.backgroundCover);
                    //GameObject.Find("Notes").GetComponent<NoteManager>().Refresh();
                }
                break;
            case EditorControlMethod.OpStart:
                {
                    timeProvider.SetStartTime(data.startAt, data.startTime, data.audioSpeed);
                    loader.noteSpeed = (float)(107.25 / (71.4184491 * Mathf.Pow(data.noteSpeed + 0.9975f, -0.985558604f)));
                    loader.touchSpeed = data.touchSpeed;
                    loader.smoothSlideAnime = data.smoothSlideAnime;
                    objectCounter.ComboSetActive(data.comboStatusType);
                    loader.LoadJson(File.ReadAllText(data.jsonPath), data.startTime);
                    GameObject.Find("MultTouchHandler").GetComponent<MultTouchHandler>().clearSlots();

                    bgManager.LoadBGFromPath(new FileInfo(data.jsonPath).DirectoryName, data.audioSpeed);
                    bgCover.color = new Color(0f, 0f, 0f, data.backgroundCover);
                    bgManager.PlaySongDetail();
                    //GameObject.Find("Notes").GetComponent<NoteManager>().Refresh();
                }
                break;
            case EditorControlMethod.Record:
                {
                    var maidataPath = new FileInfo(data.jsonPath).DirectoryName;
                    timeProvider.SetStartTime(data.startAt, data.startTime, data.audioSpeed, true);
                    loader.noteSpeed = (float)(107.25 / (71.4184491 * Mathf.Pow(data.noteSpeed + 0.9975f, -0.985558604f)));
                    loader.touchSpeed = data.touchSpeed;
                    loader.smoothSlideAnime = data.smoothSlideAnime;
                    objectCounter.ComboSetActive(data.comboStatusType);
                    loader.LoadJson(File.ReadAllText(data.jsonPath), data.startTime);
                    multTouchHandler.clearSlots();

                    screenRecorder.CutoffTime = getChartLength();
                    screenRecorder.CutoffTime += 10f;
                    screenRecorder.StartRecording(maidataPath);

                    bgManager.LoadBGFromPath(maidataPath, data.audioSpeed);
                    bgCover.color = new Color(0f, 0f, 0f, data.backgroundCover);
                    bgManager.PlaySongDetail();
                    GameObject.Find("CanvasButtons").SetActive(false);
                    //GameObject.Find("Notes").GetComponent<NoteManager>().Refresh();
                }
                break;
            case EditorControlMethod.Pause:
                timeProvider.isStart = false;
                bgManager.PauseVideo();
                break;
            case EditorControlMethod.Stop:
                screenRecorder.StopRecording();
                timeProvider.ResetStartTime();
                IsReloding = true;
                SceneManager.LoadScene(1);
                break;
            case EditorControlMethod.Continue:
                timeProvider.SetStartTime(data.startAt, data.startTime, data.audioSpeed);
                bgManager.ContinueVideo(data.audioSpeed);
                break;
        }
    }

    private void OnDestroy()
    {
        http.Stop();
        print("server stoped");
    }

    private void httpListen()
    {
        while (http.IsListening)
        {
            var context = http.GetContext();
            print(context.Request.HttpMethod);
            var reader = new StreamReader(context.Request.InputStream);
            var data = reader.ReadToEnd();
            print(data);
            request = data;
            while (request != "") ;
            context.Response.StatusCode = 200;
            var stream = new StreamWriter(context.Response.OutputStream);
            stream.WriteLine("Hello!!!");
            stream.Close();
            context.Response.Close();
        }

        print("exit listen");
    }

    private float getChartLength()
    {
        var length = 0f;
        foreach (var noteData in GameObject.Find("Notes").GetComponentsInChildren<NoteDrop>(true))
        {
            length = Math.Max(length, noteData.time);

            var longData = noteData as NoteLongDrop;
            if (longData != null) length = Math.Max(length, noteData.time + longData.LastFor);
        }

        return length;
    }
}