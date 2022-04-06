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
    public bool isEX = false;

    public Sprite tapSpr;
    public Sprite eachSpr;
    public Sprite exSpr;

    public Sprite eachLine;

    public GameObject holdEffect;

    public GameObject tapLine;

    public Color exEffectTap;
    public Color exEffectEach;

    AudioTimeProvider timeProvider;

    SpriteRenderer spriteRenderer;
    SpriteRenderer lineSpriteRender;
    SpriteRenderer exSpriteRender;
    void Start()
    {
        var notes = GameObject.Find("Notes").transform;
        holdEffect = Instantiate(holdEffect,notes);
        holdEffect.SetActive(false);
        
        tapLine = Instantiate(tapLine,notes);
        tapLine.SetActive(false);
        lineSpriteRender = tapLine.GetComponent<SpriteRenderer>();
        
        exSpriteRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
        
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        int sortOrder = (int)(time * -100);
        spriteRenderer.sortingOrder = sortOrder;
        exSpriteRender.sortingOrder = sortOrder;

        spriteRenderer.sprite = tapSpr;
        exSpriteRender.sprite = exSpr;

        if (isEX)
        {
            exSpriteRender.color = exEffectTap;
        }
        if (isEach)
        {
            spriteRenderer.sprite = eachSpr;
            lineSpriteRender.sprite = eachLine;
            if (isEX)
            {
                exSpriteRender.color = exEffectEach;
            }
        }
        spriteRenderer.forceRenderingOff = true;
        exSpriteRender.forceRenderingOff = true;
    }

    // Update is called once per frame
    void Update()
    {
        var timing = timeProvider.AudioTime - time;
        var distance = timing * speed + 4.8f;
        var destScale = distance * 0.4f + 0.51f;
        if (destScale < 0f) { 
            destScale = 0f;
            return;
        }
        spriteRenderer.forceRenderingOff = false;
        if (isEX) exSpriteRender.forceRenderingOff = false;

        spriteRenderer.size = new Vector2(1.22f, 1.4f);

        var holdTime = timing - lastFor;
        var holdDistance = holdTime * speed + 4.8f;
        if (holdTime > 0) {
            GameObject.Find("TapEffects").GetComponent<TapEffectManager>().PlayEffect(startPosition, false);
            GameObject.Find("ObjectCount").GetComponent<ObjectCount>().holdCount++;
            Destroy(tapLine);
            Destroy(holdEffect);
            Destroy(gameObject); 
        }


        transform.rotation = Quaternion.Euler(0, 0, -22.5f + (-45f * (startPosition - 1)));
        tapLine.transform.rotation = transform.rotation;
        holdEffect.transform.position = getPositionFromDistance(4.8f);

        if (distance < 1.225f)
        {


            transform.localScale = new Vector3(destScale, destScale);
            spriteRenderer.size = new Vector2(1.22f, 1.42f);
            distance = 1.225f;
            Vector3 pos = getPositionFromDistance(distance);
            transform.position = pos;

            if (destScale > 0.3f) tapLine.SetActive(true);
        }
        else
        {
            if (holdDistance < 1.225f && distance >= 4.8f) //两头都出界
            {
                holdDistance = 1.225f;
                distance = 4.8f;
                holdEffect.SetActive(true);
            }
            if (holdDistance < 1.225f && distance < 4.8f)
            {
                holdDistance = 1.225f;
            }
            if(holdDistance >= 1.225f && distance >= 4.8f)
            {
                distance = 4.8f;
                holdEffect.SetActive(true);
            }
            if (holdDistance >= 1.225f && distance < 4.8f)
            {

            }
            var dis = (distance - holdDistance) / 2 + holdDistance;
            transform.position = getPositionFromDistance(dis);//0.325
            var size = distance - holdDistance + 1.4f;
            spriteRenderer.size = new Vector2(1.22f, size);
            transform.localScale = new Vector3(1f, 1f);
        }
        var lineScale = Mathf.Abs(distance / 4.8f);
        lineScale = lineScale >= 1f ? 1f : lineScale;
        tapLine.transform.localScale = new Vector3(lineScale, lineScale, 1f);
        exSpriteRender.size = spriteRenderer.size;
        //lineSpriteRender.color = new Color(1f, 1f, 1f, lineScale);
    }

    Vector3 getPositionFromDistance(float distance)
    {
        return new Vector3(
            distance * Mathf.Cos((startPosition * -2f + 5f) * 0.125f * Mathf.PI),
            distance * Mathf.Sin((startPosition * -2f + 5f) * 0.125f * Mathf.PI));
    }
}
