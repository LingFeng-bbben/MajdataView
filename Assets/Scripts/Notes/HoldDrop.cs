﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldDrop : NoteLongDrop
{
    // public float time;
    // public float LastFor = 1f;
    public int startPosition = 1;
    public float speed = 1;

    public bool isEach = false;
    public bool isEX = false;
    public bool isBreak = false;

    public Sprite tapSpr;
    public Sprite eachSpr;
    public Sprite exSpr;
    public Sprite breakSpr;

    public Sprite eachLine;
    public Sprite breakLine;

    public Sprite holdEachEnd;
    public Sprite holdBreakEnd;

    public RuntimeAnimatorController HoldShine;
    public RuntimeAnimatorController BreakShine;

    public GameObject holdEffect;

    public GameObject tapLine;

    public Color exEffectTap;
    public Color exEffectEach;
    public Color exEffectBreak;

    AudioTimeProvider timeProvider;

    SpriteRenderer spriteRenderer;
    SpriteRenderer lineSpriteRender;
    SpriteRenderer exSpriteRender;
    SpriteRenderer holdEndRender;

    bool breakAnimStart = false;
    bool holdAnimStart = false;
    Animator animator;

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

        holdEndRender = transform.GetChild(1).GetComponent<SpriteRenderer>();

        int sortOrder = (int)(time * -100);
        spriteRenderer.sortingOrder = sortOrder;
        exSpriteRender.sortingOrder = sortOrder;
        holdEndRender.sortingOrder = sortOrder;

        spriteRenderer.sprite = tapSpr;
        exSpriteRender.sprite = exSpr;

        Animator anim = gameObject.AddComponent<Animator>();
        anim.enabled = false;
        animator = anim;

        if (isEX)
        {
            exSpriteRender.color = exEffectTap;
        }
        if (isEach)
        {
            spriteRenderer.sprite = eachSpr;
            lineSpriteRender.sprite = eachLine;
            holdEndRender.sprite = holdEachEnd;
            if (isEX)
            {
                exSpriteRender.color = exEffectEach;
            }
        }
        if (isBreak)
        {
            spriteRenderer.sprite = breakSpr;
            lineSpriteRender.sprite = breakLine;
            holdEndRender.sprite = holdBreakEnd;
            if (isEX)
            {
                exSpriteRender.color = exEffectBreak;
            }
        }
        spriteRenderer.forceRenderingOff = true;
        exSpriteRender.forceRenderingOff = true;
        holdEndRender.enabled = false;
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

        if (isBreak && !breakAnimStart)
        {
            breakAnimStart = true;
            animator.runtimeAnimatorController = BreakShine;
            animator.enabled = true; // break hold闪烁
        }

        spriteRenderer.size = new Vector2(1.22f, 1.4f);

        var holdTime = timing - LastFor;
        var holdDistance = holdTime * speed + 4.8f;
        if (holdTime > 0) {
            GameObject.Find("NoteEffects").GetComponent<NoteEffectManager>().PlayEffect(startPosition, isBreak);
            if (isBreak)
            {
                GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>().breakCount++;
            }
            else
            {
                GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>().holdCount++;
            }
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
            if (holdDistance < 1.225f && distance >= 4.8f) // 头到达 尾未出现
            {
                holdDistance = 1.225f;
                distance = 4.8f;
                holdEffect.SetActive(true);
                startHoldShine();
            }
            else if (holdDistance < 1.225f && distance < 4.8f) // 头未到达 尾未出现
            {
                holdDistance = 1.225f;
            }
            else if (holdDistance >= 1.225f && distance >= 4.8f) // 头到达 尾出现
            {
                distance = 4.8f;
                holdEffect.SetActive(true);
                startHoldShine();

                holdEndRender.enabled = true;
            }
            else if (holdDistance >= 1.225f && distance < 4.8f) // 头未到达 尾出现
            {
                holdEndRender.enabled = true;
            }
            var dis = (distance - holdDistance) / 2 + holdDistance;
            transform.position = getPositionFromDistance(dis);//0.325
            var size = distance - holdDistance + 1.4f;
            spriteRenderer.size = new Vector2(1.22f, size);
            holdEndRender.transform.localPosition = new Vector3(0f, 0.6825f-size/2);
            transform.localScale = new Vector3(1f, 1f);
        }
        var lineScale = Mathf.Abs(distance / 4.8f);
        lineScale = lineScale >= 1f ? 1f : lineScale;
        tapLine.transform.localScale = new Vector3(lineScale, lineScale, 1f);
        exSpriteRender.size = spriteRenderer.size;
        //lineSpriteRender.color = new Color(1f, 1f, 1f, lineScale);
    }

    void startHoldShine()
    {
        if (!holdAnimStart)
        {
            holdAnimStart = true;
            animator.runtimeAnimatorController = HoldShine;
            animator.enabled = true;
        }
    }

    Vector3 getPositionFromDistance(float distance)
    {
        return new Vector3(
            distance * Mathf.Cos((startPosition * -2f + 5f) * 0.125f * Mathf.PI),
            distance * Mathf.Sin((startPosition * -2f + 5f) * 0.125f * Mathf.PI));
    }
}
