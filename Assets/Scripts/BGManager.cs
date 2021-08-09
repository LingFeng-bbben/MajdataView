using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using System.IO;
using System.Drawing;
using UnityEngine.UI;

public class BGManager : MonoBehaviour
{
    GameObject SongDetail;
    SpriteRenderer spriteRender;
    VideoPlayer videoPlayer;
    RawImage rawImage;
    // Start is called before the first frame update
    void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        videoPlayer = GetComponent<VideoPlayer>();
        rawImage = GameObject.Find("Jacket").GetComponent<RawImage>();
        SongDetail = GameObject.Find("CanvasSongDetail");
        SongDetail.SetActive(false);
    }

    public void PlaySongDetail()
    {
        SongDetail.SetActive(true);
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
    }
    public void ContinueVideo()
    {
        videoPlayer.Play();
    }


    public void LoadBGFromPath(string path,float speed)
    {
        if (File.Exists(path + "/Cover.jpg"))
        {
            StartCoroutine(loadPic(path + "/Cover.jpg"));
        }
        if (File.Exists(path + "/Cover.png"))
        {
            StartCoroutine(loadPic(path + "/Cover.png"));
        }
        if (File.Exists(path + "/bg.jpg"))
        {
            StartCoroutine(loadPic(path + "/bg.jpg"));
        }
        if (File.Exists(path + "/bg.png"))
        {
            StartCoroutine(loadPic(path + "/bg.png"));
        }
        if (File.Exists(path + "/bg.mp4"))
        {
            videoPlayer.url = "file://" + path + "/bg.mp4";
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
            videoPlayer.playbackSpeed = speed;
            StartCoroutine(waitFumenStart());
            return;
        }
        if (File.Exists(path + "/bg.wmv"))
        {
            videoPlayer.url = "file://" + path + "/bg.wmv";
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
            videoPlayer.playbackSpeed = speed;
            StartCoroutine(waitFumenStart());
            return;
        }
    }

    IEnumerator waitFumenStart()
    {
        var provider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        videoPlayer.time = provider.offset;
        videoPlayer.Play();
        yield return new WaitForEndOfFrame();// Load first frame
        videoPlayer.Pause();
        while (provider.AudioTime <= 0) yield return new WaitForEndOfFrame();
        videoPlayer.Play();
        videoPlayer.time = provider.offset;
    }

    IEnumerator loadPic(string path)
    {

        UnityWebRequest req = UnityWebRequest.Get("file://"+path);
        yield return req.SendWebRequest();
        System.Drawing.Image image = System.Drawing.Image.FromFile(path);
        Texture2D texture = new Texture2D(image.Width, image.Height);
        texture.LoadImage(req.downloadHandler.data);
        rawImage.texture = texture;
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
