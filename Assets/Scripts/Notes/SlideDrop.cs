﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDrop : NoteLongDrop
{
    // Start is called before the first frame update
    public GameObject star_slide;

    SpriteRenderer spriteRenderer_star;

    public Sprite spriteNormal;
    public Sprite spriteEach;
    public Sprite spriteBreak;
    public RuntimeAnimatorController slideShine;

    public bool isMirror;
    public bool isJustR;
    public bool isSpecialFlip; // fixes known star problem
    public bool isEach;
    public bool isBreak;
    public bool isGroupPart;
    public bool isGroupPartEnd;
    // public float time;
    public float timeStar;
    // public float LastFor = 1f;
    public float speed;

    public int startPosition =1;

    public int sortIndex = 0;

    AudioTimeProvider timeProvider;

    List<GameObject> slideBars = new List<GameObject>();
    GameObject slideOK;

    List<Vector3> slidePositions = new List<Vector3>();
    List<Quaternion> slideRotations = new List<Quaternion>();

    bool startShining = false;
    List<Animator> animators = new List<Animator>();

    void Start()
    {
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
    }

    private void OnEnable()
    {
        slideOK = transform.GetChild(transform.childCount - 1).gameObject;//slideok is the last one
        if (isMirror)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            transform.rotation = Quaternion.Euler(0f, 0f, -45f * (startPosition ));
            slideOK.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -45f * (startPosition - 1));
        }

        if (isJustR) {
            if (slideOK.GetComponent<LoadJustSprite>().setR() == 1 && isMirror)
            {
                slideOK.transform.Rotate(new Vector3(0f, 0f, 180f));
                var angel = slideOK.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                slideOK.transform.position += new Vector3(Mathf.Sin(angel) * 0.27f, Mathf.Cos(angel) * -0.27f);
            }
        } else {
            if (slideOK.GetComponent<LoadJustSprite>().setL() == 1 && !isMirror)
            {
                slideOK.transform.Rotate(new Vector3(0f, 0f, 180f));
                var angel = slideOK.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                slideOK.transform.position += new Vector3(Mathf.Sin(angel) * 0.27f, Mathf.Cos(angel) * -0.27f);
            }
        }

        
        spriteRenderer_star = star_slide.GetComponent<SpriteRenderer>();
        for (int i = 0; i < transform.childCount-1; i++)
        {
            slideBars.Add(transform.GetChild(i).gameObject);
        }

        slideOK.SetActive(false);
        slideOK.transform.SetParent(transform.parent);
        slidePositions.Add(getPositionFromDistance(4.8f));
        foreach (var bars in slideBars)
        {
            slidePositions.Add(bars.transform.position);
            slideRotations.Add(Quaternion.Euler(bars.transform.rotation.eulerAngles + new Vector3(0f, 0f, 18f)));
        }
        foreach (var gm in slideBars)
        {
            var sr = gm.GetComponent<SpriteRenderer>();
            sr.color = new Color(1f, 1f, 1f, 0f);
            sr.sortingOrder += sortIndex;
            sr.sortingLayerName = "Slide";
            if (isBreak)
            {
                sr.sprite = spriteBreak;
                Animator anim = gm.AddComponent<Animator>();
                anim.runtimeAnimatorController = slideShine;
                anim.enabled = false;
                animators.Add(anim);
            }
            else if (isEach) sr.sprite = spriteEach;
            else sr.sprite = spriteNormal;
        }
    }

    // Update is called once per frame
    void Update()
    {
        

        var startiming = timeProvider.AudioTime - timeStar;
        if (startiming <= 0f)
        {
            var alpha = startiming * (speed / 3) + 1f;
            alpha = alpha > 1f ? 1f : alpha;
            alpha = alpha < 0f ? 0f : alpha;
            setSlideBarAlpha(alpha);
            return;
        }
        setSlideBarAlpha(1f);

        if (isBreak && !startShining)
        {
            startShining = true;
            foreach (Animator anim in animators)
            {
                anim.enabled = true;
                anim.Play("BreakShine", -1, 0.9f);
            }
        }

        star_slide.SetActive(true);
        var timing = timeProvider.AudioTime - time;
        if (timing <= 0f)
        {
            float alpha;
            if (isGroupPart)
            {
                alpha = 0;
            }
            else
            {
                // 只有当它是一个起点Slide（而非Slide Group中的子部分）的时候，才会有开始的星星渐入动画
                alpha = 1f - (-timing / (time - timeStar));
                alpha = alpha > 1f ? 1f : alpha;
                alpha = alpha < 0.5f ? 0.5f : alpha;
            }
            spriteRenderer_star.color = new Color(1, 1, 1, alpha);
            star_slide.transform.localScale = new Vector3(alpha + 0.5f, alpha + 0.5f, alpha + 0.5f);
            star_slide.transform.position = slidePositions[0];
            applyStarRotation(slideRotations[0]);
        }
        if (timing > 0f)
        {
            spriteRenderer_star.color = Color.white;
            star_slide.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            var process = (LastFor -timing) / LastFor;
            process = 1f - process;
            if (process > 1) {
                if (isGroupPartEnd)
                {
                    // 只有组内最后一个Slide完成 才会显示判定条并增加总数
                    if (isBreak)
                    {
                        GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>().breakCount++;
                    }
                    else
                    {
                        GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>().slideCount++;
                    }
                    slideOK.SetActive(true);
                }
                else
                {
                    // 如果不是组内最后一个 那么也要将判定条删掉
                    Destroy(slideOK);
                }
                Destroy(star_slide);
                Destroy(gameObject);
            }
            //print(process);
            var pos = (slidePositions.Count-1) * process;
            int index = (int)pos;
            try
            {
                star_slide.transform.position = (slidePositions[index + 1] - slidePositions[index]) * (pos - index) + slidePositions[index]; 
                //star_slide.transform.rotation = slideRotations[index];
                var delta = Mathf.DeltaAngle(slideRotations[index + 1].eulerAngles.z , slideRotations[index].eulerAngles.z) * (pos - index);
                delta = Mathf.Abs(delta);
                applyStarRotation(
                     Quaternion.Euler(0f, 0f,
                         Mathf.MoveTowardsAngle(slideRotations[index].eulerAngles.z, slideRotations[index + 1].eulerAngles.z, delta)
                     )
                );
                for (int i = 0; i < pos; i++)
                {
                    slideBars[i].SetActive(false);
                }
            }
            catch { }

        }
    }
    void setSlideBarAlpha(float alpha)
    {
        foreach (var gm in slideBars)
        {
            gm.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);
        }
    }
    Vector3 getPositionFromDistance(float distance)
    {
        return new Vector3(
            distance * Mathf.Cos((startPosition * -2f + 5f) * 0.125f * Mathf.PI),
            distance * Mathf.Sin((startPosition * -2f + 5f) * 0.125f * Mathf.PI));
    }
    void applyStarRotation(Quaternion newRotation)
    {
        var halfFlip = newRotation.eulerAngles;
        halfFlip.z += 180f;
        if (isSpecialFlip)
            star_slide.transform.rotation = Quaternion.Euler(halfFlip);
        else
            star_slide.transform.rotation = newRotation;
    }
}
