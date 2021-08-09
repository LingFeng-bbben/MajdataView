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

    GameObject SongDetail;


    void Start()
    {
        http.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        http.Prefixes.Add("http://localhost:8013/");
        http.Start();
        listen = new Task(httpListen);
        listen.Start();
        print("server started");

        SongDetail = GameObject.Find("CanvasSongDetail");
        SongDetail.SetActive(false);
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

            context.Response.StatusCode = 200;
            StreamWriter stream = new StreamWriter(context.Response.OutputStream);
            print("request");
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

        if (data.control == EditorControlMethod.Start)
        {
            timeProvider.SetStartTime(data.startAt, data.startTime, data.audioSpeed);
            loader.speed = data.playSpeed;
            loader.LoadJson(File.ReadAllText(data.jsonPath),data.startTime);
            
            bgManager.LoadBGFromPath(new FileInfo(data.jsonPath).DirectoryName,data.audioSpeed);
            bgCover.color = new Color(0f, 0f, 0f, data.backgroundCover);
        }
        if (data.control == EditorControlMethod.OpStart)
        {
            timeProvider.SetStartTime(data.startAt, data.startTime, data.audioSpeed);
            loader.speed = data.playSpeed;
            loader.LoadJson(File.ReadAllText(data.jsonPath), data.startTime);

            bgManager.LoadBGFromPath(new FileInfo(data.jsonPath).DirectoryName, data.audioSpeed);
            bgCover.color = new Color(0f, 0f, 0f, data.backgroundCover);

            SongDetail.SetActive(true);
        }
        if(data.control == EditorControlMethod.Pause)
        {
            timeProvider.isStart = false;
        }
        if (data.control == EditorControlMethod.Stop)
        {
            timeProvider.ResetStartTime();
            SceneManager.LoadScene(0);
        }
        request = "";
    }
}
