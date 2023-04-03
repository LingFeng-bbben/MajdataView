﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapDrop : NoteDrop
{
    // Start is called before the first frame update
    // public float time;
    public int startPosition = 1;
    public float speed = 1;

    public bool isEach = false;
    public bool isBreak = false;
    public bool isEX = false;
    public bool isFakeStarRotate = false;

    public Sprite normalSpr;
    public Sprite eachSpr;
    public Sprite breakSpr;
    public Sprite exSpr;

    public Sprite eachLine;
    public Sprite breakLine;

    public RuntimeAnimatorController BreakShine;

    public GameObject tapLine;

    public Color exEffectTap;
    public Color exEffectEach;
    public Color exEffectBreak;

    AudioTimeProvider timeProvider;

    SpriteRenderer spriteRenderer;
    SpriteRenderer lineSpriteRender;
    SpriteRenderer exSpriteRender;

    ObjectCounter ObjectCounter;

    bool breakAnimStart = false;
    Animator animator;

    SoundEffectManager seManager;

    void Start()
    {
        var notes = GameObject.Find("Notes").transform;
        tapLine = Instantiate(tapLine,notes);
        tapLine.SetActive(false);
        lineSpriteRender = tapLine.GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        exSpriteRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        ObjectCounter = GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>();
        seManager = GameObject.Find("SoundEffectManager").GetComponent<SoundEffectManager>();
        
        int sortOrder = (int)(time * -100);
        spriteRenderer.sortingOrder = sortOrder;
        exSpriteRender.sortingOrder = sortOrder;

        spriteRenderer.sprite = normalSpr;
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
        if (isBreak)
        {
            spriteRenderer.sprite = breakSpr;
            lineSpriteRender.sprite = breakLine;
            if (isEX)
            {
                exSpriteRender.color = exEffectBreak;
            }
            Animator anim = gameObject.AddComponent<Animator>();  // break tap闪烁
            anim.runtimeAnimatorController = BreakShine;
            anim.enabled = false;
            animator = anim;
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
        if(isEX) exSpriteRender.forceRenderingOff = false;
        if (isBreak && !breakAnimStart)
        {
            breakAnimStart = true;
            animator.enabled = true;
            animator.Play("BreakShine", -1, 0.5f);
        }

        if (timing > 0)
        {
            seManager.PlayTap(isBreak, isEX);

            GameObject.Find("NoteEffects").GetComponent<NoteEffectManager>().PlayEffect(startPosition, isBreak);
            if (isBreak) ObjectCounter.breakCount++;
            else ObjectCounter.tapCount++;

            Destroy(tapLine);
            Destroy(gameObject);
        }
        if (isFakeStarRotate) {
            transform.Rotate(0f, 0f, 400f*Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, -22.5f + (-45f * (startPosition - 1)));
        }
        tapLine.transform.rotation = Quaternion.Euler(0, 0, -22.5f + (-45f * (startPosition - 1)));

        if (distance < 1.225f)
        {
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
        //lineSpriteRender.color = new Color(1f, 1f, 1f, lineScale);
    }

    Vector3 getPositionFromDistance(float distance)
    {
        return new Vector3(
            distance * Mathf.Cos((startPosition * -2f + 5f)*0.125f * Mathf.PI), 
            distance * Mathf.Sin((startPosition * -2f + 5f) * 0.125f * Mathf.PI));
    }
}
