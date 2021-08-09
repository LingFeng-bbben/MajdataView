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
    public float offset = 0f;
    public void SetStartTime(long _ticks, float _offset,float _speed)
    {
        ticks = _ticks;
        offset = _offset;
        AudioTime = offset;
        var dateTime = new DateTime(ticks);
        var seconds = (dateTime - DateTime.Now).TotalSeconds;
        startTime = Time.realtimeSinceStartup + (float)seconds;
        speed = _speed;
        isStart = true;
    }

    public void ResetStartTime()
    {
        offset = 0f;
        isStart = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
            AudioTime = (Time.realtimeSinceStartup - startTime) * speed + offset;
    }
}
