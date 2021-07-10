using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioTimeProvider : MonoBehaviour
{
    public float AudioTime = 0f; //notes get this value

    float startTime;
    long ticks = 0;
    bool isStart = false;
    float offset = 0f;
    public void SetStartTime(long _ticks, float _offset)
    {
        ticks = _ticks;
        offset = _offset;
        StartCoroutine(waitToStart());
    }

    public void ResetStartTime()
    {
        offset = 0f;
        isStart = false;
    }

    IEnumerator waitToStart()
    {
        while (DateTime.Now.Ticks < ticks)
        {
            yield return new WaitForEndOfFrame();
        }
        startTime = Time.realtimeSinceStartup;
        isStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(isStart)
        AudioTime = Time.realtimeSinceStartup-startTime +offset;
    }
}
