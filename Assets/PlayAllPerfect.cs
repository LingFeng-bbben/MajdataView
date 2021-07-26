using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAllPerfect : MonoBehaviour
{
    GameObject Allperfect;
    AudioTimeProvider timeProvider;
    void Start()
    {
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        Allperfect = GameObject.Find("CanvasAllPerfect");
        Allperfect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(timeProvider.isStart&&transform.childCount==0&&Allperfect) Allperfect.SetActive(true);
    }
}
