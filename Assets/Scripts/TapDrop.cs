using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapDrop : MonoBehaviour
{
    // Start is called before the first frame update
    public float time;
    public int startPosition = 1;
    public float speed = 1;

    public bool isEach = false;
    public bool isBreak = false;

    public Sprite eachSpr;
    public Sprite breakSpr;

    public Sprite eachLine;
    public Sprite breakLine;

    public GameObject tapEffect;
    public GameObject tapLine;

    AudioTimeProvider timeProvider;

    SpriteRenderer spriteRenderer;
    SpriteRenderer lineSpriteRender;
    void Start()
    {
        var notes = GameObject.Find("Notes").transform;
        tapLine = Instantiate(tapLine,notes);
        tapLine.SetActive(false);
        lineSpriteRender = tapLine.GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isEach) { 
            spriteRenderer.sprite = eachSpr;
            lineSpriteRender.sprite = eachLine;
        }
        if (isBreak) { 
            spriteRenderer.sprite = breakSpr;
            lineSpriteRender.sprite = breakLine;
        }

        var timing = timeProvider.AudioTime - time;
        var distance = timing * speed + 4.8f;

        if (timing > 0)
        {
            Instantiate(tapEffect, getPositionFromDistance(4.8f), transform.rotation);
            Destroy(tapLine);
            Destroy(gameObject);
        }
        transform.rotation = Quaternion.Euler(0, 0, -22.5f + (-45f * (startPosition - 1)));
        tapLine.transform.rotation = transform.rotation;

        if (distance < 1.225f)
        {

            var destScale = distance*0.4f +0.51f;
            if (destScale < 0f) destScale = 0f;
            transform.localScale = new Vector3(destScale, destScale);

            distance = 1.225f;
            Vector3 pos = getPositionFromDistance(distance);
            transform.position = pos;
            if (destScale > 0.3f) tapLine.SetActive(true);
        }
        else
        {
            Vector3 pos = getPositionFromDistance(distance);
            transform.position = pos;
            transform.localScale = new Vector3(1f, 1f);
        }
        var lineScale = Mathf.Abs(distance / 4.8f);
        tapLine.transform.localScale = new Vector3(lineScale, lineScale, 1f);
        lineSpriteRender.color = new Color(1f, 1f, 1f, lineScale);
    }

    Vector3 getPositionFromDistance(float distance)
    {
        return new Vector3(
            distance * Mathf.Cos((startPosition * -2f + 5f)*0.125f * Mathf.PI), 
            distance * Mathf.Sin((startPosition * -2f + 5f) * 0.125f * Mathf.PI));
    }
}
