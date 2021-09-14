using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EachLineDrop : MonoBehaviour
{
    public float time;
    public int startPosition = 1;
    public int curvLength = 1;
    public float speed = 1;

    public Sprite[] curvSprites;

    AudioTimeProvider timeProvider;
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();

        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sprite = curvSprites[curvLength - 1];
        sr.forceRenderingOff = true;
    }

    // Update is called once per frame
    void Update()
    {
        var timing = timeProvider.AudioTime - time;
        var distance = timing * speed + 4.8f;
        var destScale = distance * 0.4f + 0.51f;
        if (timing > 0) {
            Destroy(gameObject);
        }
        if (distance < 1.225f)
        {
            distance = 1.225f;
            if (destScale > 0.3f) sr.forceRenderingOff = false;
        }
        var lineScale = Mathf.Abs(distance / 4.8f);
        transform.localScale = new Vector3(lineScale, lineScale, 1f);
        transform.rotation = Quaternion.Euler(0, 0, (-45f * (startPosition - 1)));
    }
}
