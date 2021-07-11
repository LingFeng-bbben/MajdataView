using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using System.IO;

public class BGManager : MonoBehaviour
{
    SpriteRenderer spriteRender;
    VideoPlayer videoPlayer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        videoPlayer = GetComponent<VideoPlayer>();
    }

    public void LoadBGFromPath(string path)
    {
        if (File.Exists(path + "/bg.mp4"))
        {
            videoPlayer.url = "file://" + path + "/bg.mp4";
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
            StartCoroutine(waitFumenStart());
            return;
        }
        if (File.Exists(path + "/bg.jpg"))
        {
            StartCoroutine(loadPic(path + "/bg.jpg"));
            return;
        }
        if (File.Exists(path + "/bg.png"))
        {
            StartCoroutine(loadPic(path + "/bg.png"));
        }
    }

    IEnumerator waitFumenStart()
    {
        var provider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        while (provider.AudioTime <= 0) yield return new WaitForEndOfFrame();
        videoPlayer.Play();
    }

    IEnumerator loadPic(string path)
    {
        UnityWebRequest req = UnityWebRequest.Get("file://"+path);
        yield return req.SendWebRequest();
        
        Texture2D texture = new Texture2D(1080,1080);
        texture.LoadImage(req.downloadHandler.data);
        spriteRender.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),new Vector2(0.5f,0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
