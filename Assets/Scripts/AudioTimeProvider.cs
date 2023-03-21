using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioTimeProvider : MonoBehaviour
{
    public float AudioTime = 0f; //notes get this value

    float startTime;
    float speed;
    long ticks = 0;
    public bool isStart = false;
    public bool isRecord = false;
    public float offset = 0f;
    public void SetStartTime(long _ticks, float _offset, float _speed, bool _isRecord = false)
    {
        ticks = _ticks;
        offset = _offset;
        AudioTime = offset;
        var dateTime = new DateTime(ticks);
        var seconds = (dateTime - DateTime.Now).TotalSeconds;
        isRecord = _isRecord;
        if (_isRecord) {
            startTime = Time.time +5;
            Time.timeScale = _speed;
            Time.captureFramerate = 60;
        }
        else
        {
            startTime = Time.realtimeSinceStartup + (float)seconds;
            speed = _speed;
            Time.captureFramerate = 0;
        }
        isStart = true;
    }

    public void ResetStartTime()
    {
        offset = 0f;
        isStart = false;
    }

    public float CurrentSpeed
    {
        get {
            return isRecord ? Time.timeScale : speed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            if (isRecord)
            {
                AudioTime = (Time.time - startTime) + offset;
            }
            else
            {
                AudioTime = (Time.realtimeSinceStartup - startTime) * speed + offset;
            }
        }
    }

}
