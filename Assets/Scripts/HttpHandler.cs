using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class HttpHandler : MonoBehaviour
{

    HttpListener http = new HttpListener();
    Task listen;
    string request = "";


    void Start()
    {
        SceneManager.LoadScene(1);
        http.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        http.Prefixes.Add("http://localhost:8013/");
        http.Start();
        listen = new Task(httpListen);
        listen.Start();
        print("server started");
    }

    void httpListen()
    {
        while (http.IsListening)
        {
            var context = http.GetContext();
            print(context.Request.HttpMethod);
            StreamReader reader = new StreamReader(context.Request.InputStream);
            var data = reader.ReadToEnd();
            print(data);
            request = data;
            while (request != "") ;
            context.Response.StatusCode = 200;
            StreamWriter stream = new StreamWriter(context.Response.OutputStream);
            stream.WriteLine("Hello!!!");
            stream.Close();
            context.Response.Close();


        }
        print("exit listen");
    }

    private void OnDestroy()
    {
        http.Stop();
        print("server stoped");
    }

    private void Update()
    {
        
        if (request == "") return;
        var data = JsonConvert.DeserializeObject<EditRequestjson>(request);
        var loader = GameObject.Find("DataLoader").GetComponent<JsonDataLoader>();
        var timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        var bgManager = GameObject.Find("Background").GetComponent<BGManager>();
        var bgCover = GameObject.Find("BackgroundCover").GetComponent<SpriteRenderer>();
        var SongDetail = GameObject.Find("CanvasSongDetail");

        if (data.control == EditorControlMethod.Start)
        {
            request = "";
            timeProvider.SetStartTime(data.startAt, data.startTime, data.audioSpeed);
            loader.speed = data.playSpeed;
            loader.LoadJson(File.ReadAllText(data.jsonPath),data.startTime);
            GameObject.Find("Notes").GetComponent<PlayAllPerfect>().enabled = false;

            bgManager.LoadBGFromPath(new FileInfo(data.jsonPath).DirectoryName,data.audioSpeed);
            bgCover.color = new Color(0f, 0f, 0f, data.backgroundCover);
        }
        if (data.control == EditorControlMethod.OpStart)
        {
            request = "";
            timeProvider.SetStartTime(data.startAt, data.startTime, data.audioSpeed);
            loader.speed = data.playSpeed;
            loader.LoadJson(File.ReadAllText(data.jsonPath), data.startTime);

            bgManager.LoadBGFromPath(new FileInfo(data.jsonPath).DirectoryName, data.audioSpeed);
            bgCover.color = new Color(0f, 0f, 0f, data.backgroundCover);
            bgManager.PlaySongDetail();
        }
        if(data.control == EditorControlMethod.Pause)
        {
            timeProvider.isStart = false;
            bgManager.PauseVideo();
        }
        if (data.control == EditorControlMethod.Stop)
        {
            timeProvider.ResetStartTime();
            SceneManager.LoadScene(1);
        }
        if(data.control == EditorControlMethod.Continue)
        {
            timeProvider.SetStartTime(data.startAt, data.startTime, data.audioSpeed);
            bgManager.ContinueVideo(data.audioSpeed);
        }
        request = "";
    }
}
