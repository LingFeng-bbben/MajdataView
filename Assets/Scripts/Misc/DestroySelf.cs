using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public bool ifDestroy;
    public bool ifStopRecording;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (ifStopRecording) GameObject.Find("ScreenRecorder").GetComponent<ScreenRecorder>().StopRecording();
        if (ifDestroy) Destroy(gameObject);
    }
}