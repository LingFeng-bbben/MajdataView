using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{

    public bool ifDestroy;
    public bool ifStopRecording;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ifStopRecording) GameObject.Find("ScreenRecorder").GetComponent<ScreenRecorder>().StopRecording();
        if (ifDestroy) Destroy(gameObject);
    }
}
