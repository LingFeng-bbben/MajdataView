using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class SlideDrop : NoteLongDrop,IFlasher
{
    // Start is called before the first frame update
    public GameObject star_slide;

    public Sprite spriteNormal;
    public Sprite spriteEach;
    public Sprite spriteBreak;
    public RuntimeAnimatorController slideShine;
    public RuntimeAnimatorController judgeBreakShine;

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

    public int startPosition = 1;

    public int sortIndex;

    public float fadeInTime;

    public float fullFadeInTime;

    public float slideConst;
    float arriveTime = -1;

    public List<int> areaStep = new List<int>();
    public bool smoothSlideAnime = false;

    public Material breakMaterial;

    bool canShine = false;

    Animator fadeInAnimator = null;

    private readonly List<Animator> animators = new();

    private readonly List<GameObject> slideBars = new();

    private readonly List<Vector3> slidePositions = new();
    private readonly List<Quaternion> slideRotations = new();
    private GameObject slideOK;

    private SpriteRenderer spriteRenderer_star;

    private bool startShining;

    private AudioTimeProvider timeProvider;

    public int endPosition;

    private void Start()
    {
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();        
        // 计算Slide淡入时机
        // 在8.0速时应当提前300ms显示Slide
        fadeInTime = -3.926913f / speed ;
        // Slide完全淡入时机
        // 正常情况下应为负值；速度过高将忽略淡入
        fullFadeInTime = Math.Min(fadeInTime + 0.2f,0);
        var interval = fullFadeInTime - fadeInTime;
        fadeInAnimator = this.GetComponent<Animator>();
        fadeInAnimator.speed = 0.2f / interval; //淡入时机与正解帧间隔小于200ms时，加快淡入动画的播放速度; interval永不为0
        fadeInAnimator.SetTrigger("slide");

        slidePositions.Add(GameObject.Find("NoteEffects").transform.GetChild(0).GetChild(endPosition - 1).position);
        slideRotations.Add(Quaternion.Euler(
            slideBars.Last().transform
            .rotation
            .eulerAngles + new Vector3(0f, 0f, 18f)));


    }

    // Update is called once per frame
    private void Update()
    {
        // Slide淡入期间，不透明度从0到0.55耗时200ms
        var startiming = timeProvider.AudioTime - timeStar;
        if (startiming <= 0f)
        {
            if (!fadeInAnimator.enabled && startiming >= fadeInTime)
                fadeInAnimator.enabled = true;
            
            return;
        }
        fadeInAnimator.enabled = false;
        setSlideBarAlpha(1f);

        //if (isBreak && !startShining)
        //{
        //    startShining = true;
        //    foreach (var anim in animators)
        //    {
        //        anim.enabled = true;
        //        anim.Play("BreakShine", -1, 0.9f);
        //    }
        //}

        star_slide.SetActive(true);
        var timing = timeProvider.AudioTime - time;
        if (timing <= 0f)
        {
            canShine = true;
            float alpha;
            if (isGroupPart)
            {
                alpha = 0;
            }
            else
            {
                // 只有当它是一个起点Slide（而非Slide Group中的子部分）的时候，才会有开始的星星渐入动画
                alpha = 1f - -timing / (time - timeStar);
                alpha = alpha > 1f ? 1f : alpha;
                alpha = alpha < 0f ? 0f : alpha;
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

            var process = (LastFor - timing) / LastFor;
            process = 1f - process;
            if (process > 1)
                DestroySelf();

            //print(process);
            var pos = (slidePositions.Count - 1) * process;
            var index = Math.Min((int)pos,slidePositions.Count);

            // Slide的箭头消失到哪里
            int slideAreaIndex;
            if (smoothSlideAnime)
            {
                slideAreaIndex = index + 1;
            } else
            {
                try
                {
                    slideAreaIndex = areaStep[(int)(process * (areaStep.Count - 1))];
                }
                catch
                {
                    slideAreaIndex = areaStep.Last() ;
                }
            }
            
            try
            {
                var lastIndex = (areaStep[areaStep.Count - 1] + areaStep[areaStep.Count - 2]) / 2;
                if (isGroupPartEnd && !smoothSlideAnime && pos >= lastIndex)
                {
                    var waitTime = LastFor * slideConst / 1.3;
                    if (arriveTime == -1)
                        arriveTime = timeProvider.AudioTime;
                    else if (timeProvider.AudioTime >= arriveTime +  waitTime)
                        DestroySelf();
                    else
                        foreach (var bar in slideBars)
                            bar.SetActive(false);
                }
                star_slide.transform.position = (slidePositions[index + 1] - slidePositions[index]) * (pos - index) +
                                                slidePositions[index];
                //star_slide.transform.rotation = slideRotations[index];
                var delta = Mathf.DeltaAngle(slideRotations[index + 1].eulerAngles.z,
                    slideRotations[index].eulerAngles.z) * (pos - index);
                delta = Mathf.Abs(delta);
                applyStarRotation(
                    Quaternion.Euler(0f, 0f,
                        Mathf.MoveTowardsAngle(slideRotations[index].eulerAngles.z,
                            slideRotations[index + 1].eulerAngles.z, delta)
                    )
                );
                for (var i = 0; i < slideAreaIndex; i++) slideBars[i].SetActive(false);
            }
            catch
            {
            }
        }
    }
    void DestroySelf()
    {
        // TODO: FES星星最后一个判定区箭头的消失效果
        foreach (GameObject obj in slideBars)
        {
            obj.SetActive(false);
        }

        if (isGroupPartEnd)
        {
            // 只有组内最后一个Slide完成 才会显示判定条并增加总数
            if (isBreak)
                GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>().breakCount++;
            else
                GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>().slideCount++;
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
    public bool CanShine() => canShine;
    private void OnEnable()
    {
        slideOK = transform.GetChild(transform.childCount - 1).gameObject; //slideok is the last one        

        if(isBreak)
            slideOK.GetComponent<Animator>().runtimeAnimatorController = judgeBreakShine;

        if (isMirror)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            transform.rotation = Quaternion.Euler(0f, 0f, -45f * startPosition);
            slideOK.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -45f * (startPosition - 1));
        }

        if (isJustR)
        {
            if (slideOK.GetComponent<LoadJustSprite>().setR() == 1 && isMirror)
            {
                slideOK.transform.Rotate(new Vector3(0f, 0f, 180f));
                var angel = slideOK.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                slideOK.transform.position += new Vector3(Mathf.Sin(angel) * 0.27f, Mathf.Cos(angel) * -0.27f);
            }
        }
        else
        {
            if (slideOK.GetComponent<LoadJustSprite>().setL() == 1 && !isMirror)
            {
                slideOK.transform.Rotate(new Vector3(0f, 0f, 180f));
                var angel = slideOK.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                slideOK.transform.position += new Vector3(Mathf.Sin(angel) * 0.27f, Mathf.Cos(angel) * -0.27f);
            }
        }


        spriteRenderer_star = star_slide.GetComponent<SpriteRenderer>();

        if(isBreak)
        {
            spriteRenderer_star.material = breakMaterial;
            spriteRenderer_star.material.SetFloat("_Brightness", 0.95f);
            var controller = star_slide.AddComponent<BreakShineController>();
            controller.enabled = true;
            controller.parent = this;
        }

        for (var i = 0; i < transform.childCount - 1; i++) slideBars.Add(transform.GetChild(i).gameObject);

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
                sr.material = breakMaterial;
                sr.material.SetFloat("_Brightness", 0.95f);
                var controller = gm.AddComponent<BreakShineController>();
                controller.parent = this;
                controller.enabled = true;
                //anim.runtimeAnimatorController = slideShine;
                //anim.enabled = false;
                //animators.Add(anim);
            }
            else if (isEach)
            {
                sr.sprite = spriteEach;
            }
            else
            {
                sr.sprite = spriteNormal;
            }
        }
    }

    private void setSlideBarAlpha(float alpha)
    {
        foreach (var gm in slideBars) gm.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);
    }

    private Vector3 getPositionFromDistance(float distance)
    {
        return new Vector3(
            distance * Mathf.Cos((startPosition * -2f + 5f) * 0.125f * Mathf.PI),
            distance * Mathf.Sin((startPosition * -2f + 5f) * 0.125f * Mathf.PI));
    }

    private void applyStarRotation(Quaternion newRotation)
    {
        var halfFlip = newRotation.eulerAngles;
        halfFlip.z += 180f;
        if (isSpecialFlip)
            star_slide.transform.rotation = Quaternion.Euler(halfFlip);
        else
            star_slide.transform.rotation = newRotation;
    }
}