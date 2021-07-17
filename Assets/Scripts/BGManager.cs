using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using System.IO;
using System.Drawing;

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
        if (File.Exists(path + "/Cover.jpg"))
        {
            StartCoroutine(loadPic(path + "/Cover.jpg"));
            return;
        }
        if (File.Exists(path + "/Cover.png"))
        {
            StartCoroutine(loadPic(path + "/Cover.png"));
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
        videoPlayer.time = provider.offset;
        videoPlayer.Play();
    }

    IEnumerator loadPic(string path)
    {

        UnityWebRequest req = UnityWebRequest.Get("file://"+path);
        yield return req.SendWebRequest();
        Image image = Image.FromFile(path);
        Texture2D texture = new Texture2D(image.Width, image.Height);
        texture.LoadImage(req.downloadHandler.data);
        spriteRender.sprite = Sprite.Create(texture, new Rect(0, 0, image.Width, image.Width),new Vector2(0.5f,0.5f));
        var scale = 1080f/(float)image.Width;
        gameObject.transform.localScale = new Vector3(scale, scale, scale);
        image.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
