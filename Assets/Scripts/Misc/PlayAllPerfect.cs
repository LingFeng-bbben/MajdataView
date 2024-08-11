using Assets.Scripts.Types;
using UnityEngine;
#nullable enable
public class PlayAllPerfect : MonoBehaviour
{
    private GameObject Allperfect;
    private AudioTimeProvider timeProvider;
    JsonDataLoader loader;

    private void Start()
    {
        loader = GameObject.FindAnyObjectByType<JsonDataLoader>();
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        Allperfect = GameObject.Find("CanvasAllPerfect");
        Allperfect.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (loader is null)
            return;
        else if (loader.State is not (NoteLoaderStatus.Idle or NoteLoaderStatus.Finished))
            return;
        else if (timeProvider.isStart && transform.childCount == 0 && Allperfect) Allperfect.SetActive(true);
    }
}