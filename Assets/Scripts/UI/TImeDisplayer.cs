using System;
using UnityEngine;
using UnityEngine.UI;

public class TImeDisplayer : MonoBehaviour
{
    private Text text;

    private AudioTimeProvider timeProvider;

    // Start is called before the first frame update
    private void Start()
    {
        text = GetComponent<Text>();
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
    }

    // Update is called once per frame

    private void Update()
    {
        // Lock AudioTime variable for real
        var ctime = timeProvider.AudioTime;
        var timenowInt = (int)ctime;
        var minute = timenowInt / 60;
        var second = timenowInt - 60 * minute;
        double mili = (ctime - timenowInt) * 10000;

        // Make timing display "cleaner" on negative timing.
        if (ctime < 0)
        {
            minute = Math.Abs(minute);
            second = Math.Abs(second);
            mili = Math.Abs(mili);
            text.text = string.Format("-{0}:{1:00}.{2:000}", minute, second, mili / 10);
        }
        else
        {
            text.text = string.Format("{0}:{1:00}.{2:0000}", minute, second, mili);
        }
    }
}