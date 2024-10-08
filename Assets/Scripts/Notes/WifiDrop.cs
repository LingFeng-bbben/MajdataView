﻿using Assets.Scripts.Interfaces;
using Assets.Scripts.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
#nullable enable
public class WifiDrop : NoteLongDrop,IFlasher
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
    public RuntimeAnimatorController judgeBreakShine;

    public bool isJustR;

    public float timeStart;
    public bool isBreak;
    public bool isGroupPart;
    public bool isGroupPartEnd;

    public int endPosition;
    public int sortIndex;

    public float fadeInTime;
    public float slideConst;
    float arriveTime = -1;
    public float fullFadeInTime;

    public Material breakMaterial;

    bool canShine = false;

    public List<int> areaStep = new List<int>();
    public bool smoothSlideAnime = false;

    Animator fadeInAnimator = null;

    private readonly List<SpriteRenderer> sbRender = new();

    private readonly List<GameObject> slideBars = new();
    private readonly Vector3[] SlidePositionEnd = new Vector3[3];

    private readonly SpriteRenderer[] spriteRenderer_star = new SpriteRenderer[3];
    private readonly GameObject[] star_slide = new GameObject[3];
    private GameObject slideOK;

    private Vector3 SlidePositionStart;

    private bool isDestroying = false;

    bool isChecking = false;
    bool isFinished { get => _judgeQueues.All(x => x.Count == 0); }
    bool canCheck = false;
    Dictionary<GameObject, Guid> guids = new();
    SensorManager sManager;
    List<GameObject> sensors = new();
    List<SensorType> boundSensors = new();
    public List<List<JudgeArea>> _judgeQueues = new();
    public List<List<JudgeArea>> judgeQueues = new();
    public Dictionary<GameObject, List<Sensor>> triggerSensors = new();

    private void Start()
    {
        // 计算Slide淡入时机
        // 在8.0速时应当提前300ms显示Slide
        fadeInTime = -3.926913f / speed;
        // Slide完全淡入时机
        // 正常情况下应为负值；速度过高将忽略淡入
        fullFadeInTime = Math.Min(fadeInTime + 0.2f, 0);
        var interval = fullFadeInTime - fadeInTime;
        fadeInAnimator = this.GetComponent<Animator>();
        fadeInAnimator.speed = 0.2f / interval; //淡入时机与正解帧间隔小于200ms时，加快淡入动画的播放速度; interval永不为0
        fadeInAnimator.SetTrigger("wifi");

        objectCounter = GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>();
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        var notes = GameObject.Find("Notes").transform;
        for (var i = 0; i < star_slide.Length; i++)
        {
            star_slide[i] = Instantiate(star_slidePrefab, notes);
            spriteRenderer_star[i] = star_slide[i].GetComponent<SpriteRenderer>();
            
            if (isBreak) spriteRenderer_star[i].sprite = breakStar;
            else if (isEach) spriteRenderer_star[i].sprite = eachStar;
            else spriteRenderer_star[i].sprite = normalStar;
            star_slide[i].transform.rotation = Quaternion.Euler(0, 0, -22.5f * (8 + i + 2 * (startPosition - 1)));
            //SlidePositionEnd[i] = getPositionFromDistance(4.8f, i + 3 + startPosition);
            star_slide[i].SetActive(false);
        }

        SlidePositionEnd[0] = GameObject.Find("NoteEffects").transform.GetChild(0).GetChild(endPosition - 2 < 0 ? 7 : endPosition - 2).position;// R
        SlidePositionEnd[1] = GameObject.Find("NoteEffects").transform.GetChild(0).GetChild(endPosition - 1).position;// Center
        SlidePositionEnd[2] = GameObject.Find("NoteEffects").transform.GetChild(0).GetChild(endPosition >= 8 ? 0 : endPosition).position; // L


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

        if (isBreak)
        {
            foreach(var star in star_slide)
            {
                var renderer = star.GetComponent<SpriteRenderer>();
                renderer.material = breakMaterial;
                renderer.material.SetFloat("_Brightness", 0.95f);
                var controller = star.AddComponent<BreakShineController>();
                controller.enabled = true;
                controller.parent = this;
            }
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
                sr.material = breakMaterial;
                sr.material.SetFloat("_Brightness", 0.95f);
                var controller = slideBars[i].AddComponent<BreakShineController>();
                controller.parent = this;
                controller.enabled = true;
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
            sr.sortingOrder = sortIndex--;
            sr.sortingLayerName = "Slide";
        }
        var sManagerObj = GameObject.Find("Sensors");
        sManager = sManagerObj.GetComponent<SensorManager>();

        
        var count = GameObject.Find("Sensors").transform.childCount;
        
        for (int i = 0; i < count; i++)
            sensors.Add(sManagerObj.transform.GetChild(i).gameObject);
        triggerSensors.Clear();
        guids.Clear();
        foreach (var star in star_slide)
        {
            triggerSensors.Add(star, new());
            guids.Add(star, Guid.NewGuid());
        }
        _judgeQueues = new (judgeQueues);
        foreach(var queue in _judgeQueues)
        {
            foreach (var area in queue)
                area.Reset();
        }
        //for(int i =0; i< 4; i++)
        //{
        //_judgeQueues.Add(new JudgeAreaGroup(new() { judgeQueues[0][i], judgeQueues[1][i], judgeQueues[2][i] }, judgeQueues[0][i].SlideIndex));
        //}
        //foreach(var sensor in sensors)
        //{
        //    var s = sensor.GetComponent<Sensor>();
        //    if (s != null)
        //        s.OnSensorStatusChange += Check;
        //}
        var allSensors = judgeQueues.SelectMany(x => x.SelectMany(y => y.GetSensorTypes()))
                                    .GroupBy(x => x)
                                    .Select(x => x.Key);
        inputManager = GameObject.Find("Input").GetComponent<InputManager>();
        boundSensors.AddRange(allSensors);
        foreach (var sensor in allSensors)
            inputManager.BindSensor(Check, sensor);
    }
    private void FixedUpdate()
    {
        /// time      是Slide启动的时间点
        /// timeStart 是Slide完全显示但未启动
        /// LastFor   是Slide的时值
        var timing = timeProvider.AudioTime - time;
        var startTiming = timeProvider.AudioTime - timeStart;
        var forceJudgeTiming = time + LastFor + 0.6;

        if (startTiming >= -0.05f)
            canCheck = true;
        else if (timing > 0)
            Running();        
        
        if (isFinished)
        {
            HideBar(areaStep.LastOrDefault());
            Judge();
        }
        else if (timeProvider.AudioTime - forceJudgeTiming >= 0)
            TooLateJudge();
    }
    int GetLastIndex()
    {
        if(_judgeQueues.All(x => x.Count == 0))
            return areaStep.LastOrDefault();
        else
        {
            IEnumerable<int>[] queues = new IEnumerable<int>[]
            {
                _judgeQueues[0].Select(x => x.SlideIndex),
                _judgeQueues[1].Select(x => x.SlideIndex),
                _judgeQueues[2].Select(x => x.SlideIndex),
            };
            var _ = queues.SelectMany(x => x)
                          .GroupBy(x => x)
                          .Select(x => x.Key);
            return areaStep[areaStep.FindIndex(x => x == _.Min())];
        }
    }
    void TooLateJudge()
    {
        if (_judgeQueues.Count == 1)
            slideOK.GetComponent<LoadJustSprite>().setLateGd();
        else
            slideOK.GetComponent<LoadJustSprite>().setMiss();
        isJudged = true;
        DestroySelf();
    }
    public void Check(object sender, InputEventArgs arg) => CheckAll();
    void CheckAll()
    {
        if (isFinished || !canCheck)
            return;
        else if (isChecking)
            return;
        else if (InputManager.Mode is AutoPlayMode.Enable or AutoPlayMode.Random)
            return;
        isChecking = true;
        for (int i = 0; i < 3; i++)
        {
            var queue = _judgeQueues[i];
            Check(ref queue);
            _judgeQueues[i] = queue;
        }
        isChecking = false;
    }
    void Check(ref List<JudgeArea> judgeQueue)
    {
        if (judgeQueue.Count == 0)
            return;

        var first = judgeQueue.First();
        JudgeArea second = null;

        if (judgeQueue.Count >= 2)
            second = judgeQueue[1];
        var fType = first.GetSensorTypes();
        foreach (var t in fType)
        {
            var sensor = sManager.GetSensor(t);
            first.Judge(t, sensor.Status);
        }

        if (second is not null && (first.CanSkip || first.On))
        {
            var sType = second.GetSensorTypes();
            foreach (var t in sType)
            {
                var sensor = sManager.GetSensor(t);
                second.Judge(t, sensor.Status);
            }

            if (second.IsFinished)
            {
                //HideBar(first.SlideIndex);
                judgeQueue = judgeQueue.Skip(2).ToList();
                return;
            }
            else if (second.On)
            {
                //HideBar(first.SlideIndex);
                judgeQueue = judgeQueue.Skip(1).ToList();
                return;
            }
        }

        if (first.IsFinished)
        {
            //HideBar(first.SlideIndex);
            judgeQueue = judgeQueue.Skip(1).ToList();
            return;
        }
        if (!isFinished)
            HideBar(GetLastIndex());

    }
    void Judge()
    {
        var timing = timeProvider.AudioTime - time;
        var starTiming = timeStart + (time - timeStart) * 0.667;
        var pTime = LastFor / areaStep.Last();
        var judgeTime = time + pTime * (areaStep.LastOrDefault() - 2.1f);// 正解帧
        var stayTime = (time + LastFor) - judgeTime; // 停留时间
        if (!isJudged)
        {
            arriveTime = timeProvider.AudioTime;
            var triggerTime = timeProvider.AudioTime;

            const float totalInterval = 1.2f; // 秒
            const float nPInterval = 0.4666667f; // Perfect基础区间

            float extInterval = MathF.Min(stayTime / 4, 0.733333f);           // Perfect额外区间
            float pInterval = MathF.Min(nPInterval + extInterval, totalInterval);// Perfect总区间
            var ext = MathF.Max(extInterval - 0.4f, 0);
            float grInterval = MathF.Max(0.4f - extInterval, 0);        // Great总区间
            float gdInterval = MathF.Max(0.3333334f - ext, 0); // Good总区间

            var diff = judgeTime - triggerTime; // 大于0为Fast，小于为Late
            bool isFast = false;
            JudgeType? judge = null;

            if (diff > 0)
                isFast = true;

            var p = pInterval / 2;
            var gr = grInterval / 2;
            var gd = gdInterval / 2;
            diff = MathF.Abs(diff);

            if (gr == 0)
            {
                if (diff >= p)
                    judge = isFast ? JudgeType.FastGood : JudgeType.LateGood;
                else
                    judge = JudgeType.Perfect;
            }
            else
            {
                if (diff >= gr + p || diff >= totalInterval / 2)
                    judge = isFast ? JudgeType.FastGood : JudgeType.LateGood;
                else if (diff >= p)
                    judge = isFast ? JudgeType.FastGreat : JudgeType.LateGreat;
                else
                    judge = JudgeType.Perfect;
            }

            print($"diff : {diff} ms");
            judgeResult = (JudgeType)judge;
            SetJust();
            isJudged = true;
        }
        else if (arriveTime < starTiming && timeProvider.AudioTime >= starTiming + stayTime * 0.667)
            DestroySelf();
        else if (arriveTime >= starTiming && timeProvider.AudioTime >= arriveTime + stayTime * 0.667)
            DestroySelf();
    }
    void HideBar(int endIndex)
    {
        endIndex = Math.Min(endIndex, slideBars.Count - 1);
        for (int i = 0; i <= endIndex; i++)
            slideBars[i].SetActive(false);
    }
    void Running()
    {
        if (InputManager.Mode is AutoPlayMode.Enable or AutoPlayMode.Random or AutoPlayMode.Disable)
            return;
        foreach (var star in star_slide)
        {
            var starRadius = 0.763736616f;
            var starPos = star.transform.position;
            var oldList = new List<Sensor>(triggerSensors[star]);
            triggerSensors[star].Clear();
            foreach (var s in sensors.Select(x => x.GetComponent<RectTransform>()))
            {
                var sensor = s.GetComponent<Sensor>();
                if (sensor.Group == SensorGroup.E || sensor.Group == SensorGroup.D)
                    continue;

                var rCenter = s.position;
                var rWidth = s.rect.width * s.lossyScale.x;
                var rHeight = s.rect.height * s.lossyScale.y;

                var radius = Math.Max(rWidth, rHeight) / 2;

                if ((starPos - rCenter).sqrMagnitude <= (radius * radius + starRadius * starRadius))
                    triggerSensors[star].Add(sensor);
            }
            var untriggerSensors = oldList.Where(x => !triggerSensors[star].Contains(x));

            foreach (var s in untriggerSensors)
                sManager.SetSensorOff(s.Type, guids[star]);
            foreach (var s in triggerSensors[star])
                sManager.SetSensorOn(s.Type, guids[star]);
        }
    }
    // Update is called once per frame
    private void Update()
    {
        // Wifi Slide淡入期间，不透明度从0到1耗时200ms
        var startiming = timeProvider.AudioTime - timeStart;
        if (startiming <= 0f)
        {
            if (startiming >= -0.05f)
            {
                fadeInAnimator.enabled = false;
                setSlideBarAlpha(1f);
            }
            else if (!fadeInAnimator.enabled && startiming >= fadeInTime)
                fadeInAnimator.enabled = true;
            return;
        }

        fadeInAnimator.enabled = false;
        setSlideBarAlpha(1f);
        foreach (var star in star_slide)
            star.SetActive(true);

        var timing = timeProvider.AudioTime - time;
        if (timing <= 0f)
        {
            canShine = true;
            float alpha;
            alpha = 1f - -timing / (time - timeStart);
            alpha = alpha > 1f ? 1f : alpha;
            alpha = alpha < 0f ? 0f : alpha;

            for (var i = 0; i < star_slide.Length; i++)
            {
                spriteRenderer_star[i].color = new Color(1, 1, 1, alpha);
                star_slide[i].transform.localScale = new Vector3(alpha + 0.5f, alpha + 0.5f, alpha + 0.5f);
                star_slide[i].transform.position = SlidePositionStart;
            }
        }
        else
        {
            UpdateStar();
            Running();
        }
        CheckAll();
    }
    void UpdateStar()
    {
        var timing = timeProvider.AudioTime - time;
        var process = (LastFor - timing) / LastFor;
        process = 1f - process;

        if (process >= 1)
        {
            for (var i = 0; i < star_slide.Length; i++)
            {
                spriteRenderer_star[i].color = Color.white;
                star_slide[i].transform.position = SlidePositionEnd[i];
                star_slide[i].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
            switch (InputManager.Mode)
            {
                case AutoPlayMode.Enable:
                case AutoPlayMode.Random:
                    var barIndex = areaStep[(int)(process * (areaStep.Count - 1))];
                    HideBar(barIndex);
                    DestroySelf();
                    judgeQueues.Clear();
                    return;
            }
            if (isFinished && isJudged)
                DestroySelf();
        }
        else
        {
            for (var i = 0; i < star_slide.Length; i++)
            {
                spriteRenderer_star[i].color = Color.white;
                star_slide[i].transform.position =
                    (SlidePositionEnd[i] - SlidePositionStart) * process + SlidePositionStart; //TODO add some runhua
                star_slide[i].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
        }
        switch (InputManager.Mode)
        {
            case AutoPlayMode.Enable:
            case AutoPlayMode.Random:
                var barIndex = areaStep[(int)(process * (areaStep.Count - 1))];
                judgeQueues = judgeQueues.Skip((int)(process * (judgeQueues.Count - 1))).ToList();
                HideBar(barIndex);
                break;
        }
    }
    void SetJust()
    {
        switch (judgeResult)
        {
            case JudgeType.FastGreat2:
            case JudgeType.FastGreat1:
            case JudgeType.FastGreat:
                slideOK.GetComponent<LoadJustSprite>().setFastGr();
                break;
            case JudgeType.FastGood:
                slideOK.GetComponent<LoadJustSprite>().setFastGd();
                break;
            case JudgeType.LateGood:
                slideOK.GetComponent<LoadJustSprite>().setLateGd();
                break;
            case JudgeType.LateGreat1:
            case JudgeType.LateGreat2:
            case JudgeType.LateGreat:
                slideOK.GetComponent<LoadJustSprite>().setLateGr();
                break;

        }
    }
    public bool CanShine() => canShine;
    void DestroySelf()
    {
        foreach (GameObject obj in slideBars)
            obj.SetActive(false);

        for (var i = 0; i < star_slide.Length; i++)
            Destroy(star_slide[i]);
        Destroy(gameObject);
    }
    void OnDestroy()
    {
        if (isDestroying || HttpHandler.IsReloding)
            return;

        switch (InputManager.Mode)
        {
            case AutoPlayMode.Enable:
                judgeResult = JudgeType.Perfect;
                SetJust();
                break;
            case AutoPlayMode.Random:
                judgeResult = (JudgeType)UnityEngine.Random.Range(1, 14);
                SetJust();
                break;
        }
        objectCounter.ReportResult(this, judgeResult, isBreak);
        if (isBreak && judgeResult == JudgeType.Perfect)
            slideOK.GetComponent<Animator>().runtimeAnimatorController = judgeBreakShine;
        slideOK.SetActive(true);

        
        foreach (var sensor in boundSensors)
            inputManager.UnbindSensor(Check, sensor);
        ClearTriggeredSensor();
        isDestroying = true;
    }
    void ClearTriggeredSensor()
    {
        foreach (var sensor in sensors)
        {
            var s = sensor.GetComponent<Sensor>();
            if (s != null)
            {
                foreach (var id in guids.Values)
                    s.SetOff(id);
            }
        }
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
}