using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarDrop : MonoBehaviour
{
    // Start is called before the first frame update
    public float time;
    public int startPosition = 1;
    public float speed = 1;

    public bool isEach = false;
    public bool isBreak = false;
    public bool isDouble = false;

    public Sprite tapSpr;
    public Sprite eachSpr;
    public Sprite breakSpr;

    public Sprite tapSpr_Double;
    public Sprite eachSpr_Double;
    public Sprite breakSpr_Double;

    public GameObject slide;

    public AudioSource audioSource;

    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GameObject.Find("Player").GetComponent<AudioSource>();
        //TODO: do something to instnate the slide bar
    }

    // Update is called once per frame
    void Update()
    {
        if (isDouble)
        {
            spriteRenderer.sprite = tapSpr_Double;
            if (isEach) spriteRenderer.sprite = eachSpr_Double;
            if (isBreak) spriteRenderer.sprite = breakSpr_Double;
        }
        else
        {
            if (isEach) spriteRenderer.sprite = eachSpr;
            if (isBreak) spriteRenderer.sprite = breakSpr;
        }
        var timing = audioSource.time - time;
        var distance = timing * speed + 4.8f;

        if (timing > 0) Destroy(gameObject);

        transform.rotation = Quaternion.Euler(0, 0, -22.5f + (-45f * (startPosition - 1))); //TODO:add some rotation for the star
        if (distance < 1.225f)
        {

            var destScale = distance * 0.4f + 0.51f;
            if (destScale < 0f) destScale = 0f;
            transform.localScale = new Vector3(destScale, destScale);

            distance = 1.225f;
            Vector3 pos = getPositionFromDistance(distance);
            transform.position = pos;
        }
        else
        {
            if (!slide.activeSelf) slide.SetActive(true);
            Vector3 pos = getPositionFromDistance(distance);
            transform.position = pos;
            transform.localScale = new Vector3(1f, 1f);
        }

    }

    Vector3 getPositionFromDistance(float distance)
    {
        return new Vector3(
            distance * Mathf.Cos((startPosition * -2f + 5f) * 0.125f * Mathf.PI),
            distance * Mathf.Sin((startPosition * -2f + 5f) * 0.125f * Mathf.PI));
    }
}
