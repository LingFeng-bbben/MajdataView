using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TImeDisplayer : MonoBehaviour
{
    Text text;
    AudioTimeProvider timeProvider;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
    }

    // Update is called once per frame

    void Update()
    {
        // Lock AudioTime variable for real
        float ctime = timeProvider.AudioTime;
        int timenowInt = (int)ctime;
        int minute = timenowInt / 60;
        int second = timenowInt - (60 * minute);
        double mili = (ctime - timenowInt) * 10000;

        // Make timing display "cleaner" on negative timing.
        if (ctime < 0) {
            minute = Math.Abs(minute);
            second = Math.Abs(second);
            mili   = Math.Abs(mili);
            text.text = string.Format("-{0}:{1:00}.{2:000}", minute, second, mili / 10);
        } else {
            text.text = string.Format("{0}:{1:00}.{2:0000}", minute, second, mili);
        }
    }
}
