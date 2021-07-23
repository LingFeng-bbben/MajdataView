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
        int minute = (int)timeProvider.AudioTime / 60;
        int second = (int)timeProvider.AudioTime - (60 * minute);
        double mili = (timeProvider.AudioTime - (int)timeProvider.AudioTime) * 10000;
        text.text = string.Format("{0}:{1:00}.{2:0000}", minute, second, mili);
    }
}
