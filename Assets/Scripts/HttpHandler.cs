using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

public class HttpHandler : MonoBehaviour
{

    HttpListener http = new HttpListener();
    Task listen;
    string request = "";

    void Start()
    {
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

            context.Response.StatusCode = 200;
            StreamWriter stream = new StreamWriter(context.Response.OutputStream);
            print("request");
            stream.WriteLine("Hello!!!");
            stream.Close();
            context.Response.Close();


        }
        print("exit listen");
    }

    private void OnApplicationQuit()
    {
        http.Stop();
        print("server stoped");
    }

    private void Update()
    {
        if (request == "") return;
        var data = JsonConvert.DeserializeObject<EditRequestjson>(request);
        if (data.control == EditorControlMethod.Start)
        {
            var loader = GameObject.Find("DataLoader").GetComponent<JsonDataLoader>();
            loader.LoadJson(File.ReadAllText(data.jsonPath));
            var timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
            timeProvider.SetStartTime(data.startAt,data.startTime);
        }
        request = "";
    }
}
