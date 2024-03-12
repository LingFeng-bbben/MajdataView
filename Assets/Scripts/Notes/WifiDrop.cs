using System;
using System.Collections.Generic;
using UnityEngine;

public class WifiDrop : NoteLongDrop
{
    // Start is called before the first frame update
    public GameObject star_slidePrefab;

    public Sprite[] normalSlide = new Sprite[11];
    public Sprite[] eachSlide = new Sprite[11];
    public Sprite[] breakSlide = new Sprite[11];
    public Sprite normalStar;
    public Sprite eachStar;
    public Sprite breakStar;

    public RuntimeAnimatorController slideShine;

    public bool isJustR;

    // public float time;
    public float timeStart;

    // public float LastFor = 1f;
    public float speed;
    public bool isEach;
    public bool isBreak;
    public bool isGroupPart;
    public bool isGroupPartEnd;

    public int startPosition = 1;

    public int sortIndex;

    public List<int> areaStep = new List<int>();
    public bool smoothSlideAnime = false;

    private readonly List<Animator> animators = new();
    private readonly List<SpriteRenderer> sbRender = new();

    private readonly List<GameObject> slideBars = new();
    private readonly Vector3[] SlidePositionEnd = new Vector3[3];

    private readonly SpriteRenderer[] spriteRenderer_star = new SpriteRenderer[3];
    private readonly GameObject[] star_slide = new GameObject[3];
    private GameObject slideOK;

    private Vector3 SlidePositionStart;

    private bool startShining;

    private AudioTimeProvider timeProvider;

    private void Start()
    {
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        var notes = GameObject.Find("Notes").transform;
        for (var i = 0; i < star_slide.Length; i++)
        {
            star_slide[i] = Instantiate(star_slidePrefab, notes);
            spriteRenderer_star[i] = star_slide[i].GetComponent<SpriteRenderer>();
            if (isEach) spriteRenderer_star[i].sprite = eachStar;
            else if (isBreak) spriteRenderer_star[i].sprite = breakStar;
            else spriteRenderer_star[i].sprite = normalStar;
            star_slide[i].transform.rotation = Quaternion.Euler(0, 0, -22.5f * (8 + i + 2 * (startPosition - 1)));
            SlidePositionEnd[i] = getPositionFromDistance(4.8f, i + 3 + startPosition);
            star_slide[i].SetActive(false);
        }

        transform.rotation = Quaternion.Euler(0f, 0f, -45f * (startPosition - 1));
        slideBars.Clear();
        for (var i = 0; i < transform.childCount - 1; i++) slideBars.Add(transform.GetChild(i).gameObject);
        slideOK = transform.GetChild(transform.childCount - 1).gameObject; //slideok is the last one
        if (isJustR)
        {
            slideOK.GetComponent<LoadJustSprite>().setR();
        }
        else
        {
            slideOK.GetComponent<LoadJustSprite>().setL();
            slideOK.transform.Rotate(new Vector3(0f, 0f, 180f));
        }

        slideOK.SetActive(false);
        slideOK.transform.SetParent(transform.parent);
        SlidePositionStart = getPositionFromDistance(4.8f);

        for (var i = 0; i < slideBars.Count; i++)
        {
            var sr = slideBars[i].GetComponent<SpriteRenderer>();

            if (isBreak)
            {
                sr.sprite = breakSlide[i];
                var anim = slideBars[i].AddComponent<Animator>();
                anim.runtimeAnimatorController = slideShine;
                anim.enabled = false;
                animators.Add(anim);
            }
            else if (isEach)
            {
                sr.sprite = eachSlide[i];
            }
            else
            {
                sr.sprite = normalSlide[i];
            }

            sbRender.Add(sr);
            sr.color = new Color(1f, 1f, 1f, 0f);
            sr.sortingOrder += sortIndex;
            sr.sortingLayerName = "Slide";
        }
    }

    // Update is called once per frame
    private void Update()
    {
        var startiming = timeProvider.AudioTime - timeStart;
        if (startiming <= 0f)
        {
            var alpha = startiming * (speed / 3.9269f) + 1f;
            alpha = alpha > 1f ? 1f : alpha;
            alpha = alpha < 0f ? 0f : alpha;
            setSlideBarAlpha(alpha);
            return;
        }

        setSlideBarAlpha(1f);

        if (isBreak && !startShining)
        {
            startShining = true;
            foreach (var anim in animators)
            {
                anim.enabled = true;
                anim.Play("BreakShine", -1, 0f);
            }
        }

        foreach (var star in star_slide)
            star.SetActive(true);

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
                alpha = 1f - -timing / (time - timeStart);
                alpha = alpha > 1f ? 1f : alpha;
                alpha = alpha < 0.5f ? 0.5f : alpha;
            }

            for (var i = 0; i < star_slide.Length; i++)
            {
                spriteRenderer_star[i].color = new Color(1, 1, 1, alpha);
                star_slide[i].transform.localScale = new Vector3(alpha + 0.5f, alpha + 0.5f, alpha + 0.5f);
                star_slide[i].transform.position = SlidePositionStart;
            }
        }

        if (timing > 0f)
        {
            var process = (LastFor - timing) / LastFor;
            process = 1f - process;
            if (process > 1)
            {
                foreach (GameObject obj in slideBars)
                {
                    obj.SetActive(false);
                }

                if (isGroupPartEnd)
                {
                    if (isBreak)
                        GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>().breakCount++;
                    else
                        GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>().slideCount++;
                    slideOK.SetActive(true);
                }

                for (var i = 0; i < star_slide.Length; i++)
                    Destroy(star_slide[i]);
                Destroy(gameObject);
            }

            var pos = (slideBars.Count - 1) * process;
            // Slide的箭头消失到哪里
            int slideAreaIndex;
            if (smoothSlideAnime)
            {
                slideAreaIndex = (int)pos + 1;
            }
            else
            {
                slideAreaIndex = areaStep[(int)(process * (areaStep.Count - 1))];
            }

            for (var i = 0; i < star_slide.Length; i++)
            {
                spriteRenderer_star[i].color = Color.white;
                star_slide[i].transform.position =
                    (SlidePositionEnd[i] - SlidePositionStart) * process + SlidePositionStart; //TODO add some runhua
                star_slide[i].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

            for (var i = 0; i < slideAreaIndex; i++) slideBars[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
    }

    private void setSlideBarAlpha(float alpha)
    {
        foreach (var sr in sbRender)
        {
            var oldColor = sr.color;
            oldColor.a = alpha;
            sr.color = oldColor;
        }
    }

    private Vector3 getPositionFromDistance(float distance)
    {
        return new Vector3(
            distance * Mathf.Cos((startPosition * -2f + 5f) * 0.125f * Mathf.PI),
            distance * Mathf.Sin((startPosition * -2f + 5f) * 0.125f * Mathf.PI));
    }

    private Vector3 getPositionFromDistance(float distance, int position)
    {
        return new Vector3(
            distance * Mathf.Cos((position * -2f + 5f) * 0.125f * Mathf.PI),
            distance * Mathf.Sin((position * -2f + 5f) * 0.125f * Mathf.PI));
    }
}