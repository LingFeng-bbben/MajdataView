using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldDrop : MonoBehaviour
{
    public float time;
    public float lastFor = 1f;
    public int startPosition = 1;
    public float speed = 1;

    public bool isEach = false;

    public Sprite tapSpr;
    public Sprite eachSpr;


    public AudioSource audioSource;

    SpriteRenderer spriteRenderer;
    void Start()
    {
        audioSource = GameObject.Find("Player").GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isEach) spriteRenderer.sprite = eachSpr;

        spriteRenderer.size = new Vector2(1.22f, 1.4f);

        var timing = audioSource.time - time;
        //var timing = time;
        var holdTime = timing - lastFor;

        var distance = timing * speed + 4.8f;
        var holdDistance = holdTime * speed + 4.8f;
        if (holdTime > 0) Destroy(gameObject);

        print(distance + "  " + holdDistance);

        transform.rotation = Quaternion.Euler(0, 0, -22.5f + (-45f * (startPosition - 1)));
        if (distance < 1.225f)
        {

            var destScale = distance * 0.4f + 0.51f;
            if (destScale < 0f) destScale = 0f;
            transform.localScale = new Vector3(destScale, destScale);
            spriteRenderer.size = new Vector2(1.22f, 1.42f);
            distance = 1.225f;
            Vector3 pos = getPositionFromDistance(distance);
            transform.position = pos;
        }
        else
        {
            if (holdDistance < 1.225f && distance > 4.8f)
            {
                holdDistance = 1.225f;
                distance = 4.8f;
            }
            if (holdDistance < 1.225f && distance < 4.8f)
            {
                holdDistance = 1.225f;
            }
            if(holdDistance > 1.225f && distance > 4.8f)
            {
                distance = 4.8f;
            }
            if (holdDistance > 1.225f && distance < 4.8f)
            {

            }
            var dis = (distance - holdDistance) / 2 + holdDistance;
            transform.position = getPositionFromDistance(dis);//0.325
            var size = distance - holdDistance + 1.4f;
            spriteRenderer.size = new Vector2(1.22f, size);
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
