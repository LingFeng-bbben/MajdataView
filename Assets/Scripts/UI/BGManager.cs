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
    AudioTimeProvider provider;
    float playSpeed;
    // Start is called before the first frame update
    void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        videoPlayer = GetComponent<VideoPlayer>();
        rawImage = GameObject.Find("Jacket").GetComponent<RawImage>();
        provider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
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
    public void ContinueVideo(float speed)
    {
        videoPlayer.playbackSpeed = speed;
        playSpeed = speed;
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
            loadVideo(path + "/bg.mp4", speed);
            return;
        }
        if (File.Exists(path + "/mv.mp4"))
        {
            loadVideo(path + "/mv.mp4", speed);
            return;
        }
        if (File.Exists(path + "/bg.wmv"))
        {
            loadVideo(path + "/bg.wmv", speed);
            return;
        }
    }

    IEnumerator loadPic(string path)
    {
        Sprite sprite;
        yield return sprite = SpriteLoader.LoadSpriteFromFile(path);
        rawImage.texture = sprite.texture;
        spriteRender.sprite = sprite;
        var scale = 1080f/(float)sprite.texture.width;
        gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }

    void loadVideo(string path, float speed)
    {
        videoPlayer.url = "file://" + path;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.playbackSpeed = speed;
        playSpeed = speed;
        StartCoroutine(waitFumenStart());
    }

    IEnumerator waitFumenStart()
    {
        videoPlayer.Prepare();
        //videoPlayer.timeReference = VideoTimeReference.ExternalTime;
        while (provider.AudioTime <= 0) yield return new WaitForEndOfFrame();
        while (!videoPlayer.isPrepared) yield return new WaitForEndOfFrame();
        videoPlayer.Play();
        videoPlayer.time = provider.AudioTime;

        var scale = (float)videoPlayer.height/(float)videoPlayer.width;
        spriteRender.sprite = Sprite.Create(new Texture2D(1080, 1080), new Rect(0, 0, 1080, 1080), new Vector2(0.5f, 0.5f));
        gameObject.transform.localScale = new Vector3(1f, scale);
    }
    // Update is called once per frame
    void Update()
    {
        //videoPlayer.externalReferenceTime = provider.AudioTime;
        var delta = videoPlayer.clockTime - provider.AudioTime;
        //print(delta);
        if (delta < -0.01f)
        {
            videoPlayer.playbackSpeed = playSpeed + 0.2f;
        }else if(delta > 0.01f)
        {
            videoPlayer.playbackSpeed = playSpeed - 0.2f;
        }
        else
        {
            videoPlayer.playbackSpeed = playSpeed;
        }
    }
}
