using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Un4seen.Bass;
using System.IO;

public class SoundEffectManager : MonoBehaviour
{
    public int bgmStream = -114514;
    public int answerStream = -114514;
    public int judgeStream = -114514;
    public int judgeBreakStream = -114514;   // 这个是break的判定音效 不是欢呼声
    public int breakStream = -114514;        // 这个才是欢呼声
    public int judgeExStream = -114514;
    public int hanabiStream = -114514;
    public int holdRiserStream = -114514;
    public int trackStartStream = -114514;
    public int slideStream = -114514;
    public int touchStream = -114514;
    public int allperfectStream = -114514;
    public int fanfareStream = -114514;
    public int clockStream = -114514;
    public int breakSlideStartStream = -114514;     // break-slide启动音效
    public int breakSlideStream = -114514;          // break-slide欢呼声（critical perfect音效）
    public int judgeBreakSlideStream = -114514;     // break-slide判定音效

    // Start is called before the first frame update
    void Start()
    {
        var path = new DirectoryInfo(Application.dataPath).Parent.FullName + "/SFX/";
        trackStartStream = Bass.BASS_StreamCreateFile(path + "track_start.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
        allperfectStream = Bass.BASS_StreamCreateFile(path + "all_perfect.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
        fanfareStream = Bass.BASS_StreamCreateFile(path + "fanfare.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
        clockStream = Bass.BASS_StreamCreateFile(path + "clock.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);

        answerStream = Bass.BASS_StreamCreateFile(path + "answer.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
        judgeStream = Bass.BASS_StreamCreateFile(path + "judge.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);

        breakStream = Bass.BASS_StreamCreateFile(path + "break.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
        judgeBreakStream = Bass.BASS_StreamCreateFile(path + "judge_break.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
        breakSlideStream = Bass.BASS_StreamCreateFile(path + "break_slide.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
        judgeBreakSlideStream = Bass.BASS_StreamCreateFile(path + "judge_break_slide.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);

        judgeExStream = Bass.BASS_StreamCreateFile(path + "judge_ex.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);

        slideStream = Bass.BASS_StreamCreateFile(path + "slide.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
        breakSlideStartStream = Bass.BASS_StreamCreateFile(path + "break_slide_start.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);

        touchStream = Bass.BASS_StreamCreateFile(path + "touch.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
        holdRiserStream = Bass.BASS_StreamCreateFile(path + "touchHold_riser.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
        hanabiStream = Bass.BASS_StreamCreateFile(path + "hanabi.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);


        SetVolume(trackStartStream, 0.7f);
        SetVolume(allperfectStream, 0.7f);
        SetVolume(fanfareStream, 0.7f);
        SetVolume(clockStream, 0.7f);

        SetVolume(answerStream, 0.7f);
        SetVolume(judgeStream, 0.5f);

        SetVolume(breakStream, 0.375f);
        SetVolume(judgeBreakStream, 0.5f);
        SetVolume(breakSlideStream, 0.5f);
        SetVolume(judgeBreakSlideStream, 0.5f);

        SetVolume(judgeExStream, 0.5f);

        SetVolume(slideStream, 0.4f);
        SetVolume(breakSlideStartStream, 0.4f);

        SetVolume(touchStream, 0.4f);
        SetVolume(holdRiserStream, 0.35f);
        SetVolume(hanabiStream, 0.35f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetVolume(int channel, float value)
    {
        Bass.BASS_ChannelSetAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL, value);
    }

    public void PlayTap(bool isBreak = false, bool isEx = false)
    {
        if (isBreak)
        {
            Bass.BASS_ChannelPlay(breakStream, true);
            Bass.BASS_ChannelPlay(judgeBreakStream, true);
            if (isEx)
            {
                Bass.BASS_ChannelPlay(judgeExStream, true);
            }
        }
        else
        {
            Bass.BASS_ChannelPlay(answerStream, true);
            if (isEx)
            {
                Bass.BASS_ChannelPlay(judgeExStream, true);
            }
            else
            {
                Bass.BASS_ChannelPlay(judgeStream, true);
            }
        }
    }
}
