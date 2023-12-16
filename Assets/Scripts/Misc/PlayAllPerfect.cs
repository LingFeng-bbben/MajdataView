using UnityEngine;

public class PlayAllPerfect : MonoBehaviour
{
    private GameObject Allperfect;
    private AudioTimeProvider timeProvider;

    private void Start()
    {
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        Allperfect = GameObject.Find("CanvasAllPerfect");
        Allperfect.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (timeProvider.isStart && transform.childCount == 0 && Allperfect) Allperfect.SetActive(true);
    }
}