using Assets.Scripts.Interfaces;
using Assets.Scripts.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static NoteEffectManager;

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

    private readonly List<Animator> animators = new();
    private readonly List<SpriteRenderer> sbRender = new();

    private readonly List<GameObject> slideBars = new();
    private readonly Vector3[] SlidePositionEnd = new Vector3[3];

    private readonly SpriteRenderer[] spriteRenderer_star = new SpriteRenderer[3];
    private readonly GameObject[] star_slide = new GameObject[3];
    private GameObject slideOK;

    private Vector3 SlidePositionStart;

    private bool isDestroying = false;

    public ConnSlideInfo ConnectInfo { get; set; }
    bool isFinished { get => _judgeQueues.All(x => x.Count == 0); }
    public GameObject parent;
    bool canCheck = false;
    Dictionary<GameObject, Guid> guids = new();
    SensorManager sManager;
    List<GameObject> sensors = new();
    //public List<JudgeAreaGroup> _judgeQueues = new();
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
            slideOK.GetComponent<Animator>().runtimeAnimatorController = judgeBreakShine;

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
    }
    private void FixedUpdate()
    {
        /// time      是Slide启动的时间点
        /// timeStart 是Slide完全显示但未启动
        /// LastFor   是Slide的时值
        var timing = timeProvider.AudioTime - time;
        var startTiming = timeProvider.AudioTime - timeStart;
        var forceJudgeTiming = time + LastFor + 0.6;

        if (ConnectInfo.IsGroupPart)
        {
            if (ConnectInfo.IsGroupPartHead && startTiming >= -0.040f)
                canCheck = true;
            else if (!ConnectInfo.IsGroupPartHead)
                canCheck = ConnectInfo.ParentFinished;
        }
        else if (startTiming >= -0.050f)
            canCheck = true;

        if (timing > 0)
            Running();        

        if (ConnectInfo.IsConnSlide)
        {
            if (ConnectInfo.IsGroupPartEnd && isFinished)
            {
                HideBar(areaStep.LastOrDefault());
                Judge();
            }
            else if (ConnectInfo.IsGroupPartEnd && timeProvider.AudioTime - forceJudgeTiming >= 0)
                TooLateJudge();
        }
        else if (isFinished)
        {
            HideBar(areaStep.LastOrDefault());
            Judge();
        }
        else if (timeProvider.AudioTime - forceJudgeTiming >= 0)
            TooLateJudge();
    }
    int GetLastIndex()
    {
        var queue1 = _judgeQueues[0];
        var queue2 = _judgeQueues[1];
        var queue3 = _judgeQueues[2];
        var _ = new List<int>();
        _.AddRange(queue1.Select(x => x.SlideIndex));
        _.AddRange(queue2.Select(x => x.SlideIndex));
        _.AddRange(queue3.Select(x => x.SlideIndex));
        var min = _.Min();
        return areaStep[areaStep.FindIndex(x => x == min) - 1];
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
    void CheckAll()
    {
        if (isFinished || !canCheck)
            return;
        for(int i = 0; i < 3; i++)
        {
            var queue = _judgeQueues[i];
            Check(ref queue);
            _judgeQueues[i] = queue;
        }

    }
    public void Check(ref List<JudgeArea> judgeQueue)
    {
        try
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
        catch (Exception e)
        {
            print(e);
        }

    }
    void Judge()
    {
        if (!isGroupPartEnd)
            return;
        var timing = timeProvider.AudioTime - time;
        var starTiming = timeStart + (time - timeStart) * 0.667;
        var pTime = LastFor / areaStep.Last();
        var judgeTime = time + pTime * (areaStep.LastOrDefault() - 3.5f);// 正解帧
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

            switch (judge)
            {
                case JudgeType.FastGreat:
                    slideOK.GetComponent<LoadJustSprite>().setFastGr();
                    break;
                case JudgeType.FastGood:
                    slideOK.GetComponent<LoadJustSprite>().setFastGd();
                    break;
                case JudgeType.LateGood:
                    slideOK.GetComponent<LoadJustSprite>().setLateGd();
                    break;
                case JudgeType.LateGreat:
                    slideOK.GetComponent<LoadJustSprite>().setLateGr();
                    break;
            }
            print($"diff : {diff} ms");
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
        if (!GameObject.Find("Input").GetComponent<InputManager>().AutoPlay)
            return;
        foreach(var star in star_slide)
        {
            var starRadius = 0.763736616f;
            var starPos = star.transform.position;
            var oldList = new List<Sensor>(triggerSensors[star]);
            triggerSensors[star].Clear();
            foreach (var s in sensors.Select(x => x.GetComponent<RectTransform>()))
            {
                var sensor = s.GetComponent<Sensor>();
                if (sensor.Group == Sensor.SensorGroup.E || sensor.Group == Sensor.SensorGroup.D)
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
        CheckAll();
        if (star_slide.Any(x => x == null))
        {
            if (isFinished)
                DestroySelf();
            return;
        }
        // Wifi Slide淡入期间，不透明度从0到1耗时200ms
        var startiming = timeProvider.AudioTime - timeStart;
        if (startiming <= 0f)
        {
            if (!fadeInAnimator.enabled && startiming >= fadeInTime)
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
            if (ConnectInfo.IsConnSlide && !ConnectInfo.IsGroupPartHead)
                alpha = 0;
            else
            {
                alpha = 1f - -timing / (time - timeStart);
                alpha = alpha > 1f ? 1f : alpha;
                alpha = alpha < 0f ? 0f : alpha;
            }

            for (var i = 0; i < star_slide.Length; i++)
            {
                spriteRenderer_star[i].color = new Color(1, 1, 1, alpha);
                star_slide[i].transform.localScale = new Vector3(alpha + 0.5f, alpha + 0.5f, alpha + 0.5f);
                star_slide[i].transform.position = SlidePositionStart;
            }
        }
        else
        {
            
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
                if (isFinished)
                    DestroySelf();
                else if (ConnectInfo.IsConnSlide && !ConnectInfo.IsGroupPartEnd)
                    DestroySelf(true);
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
            Running();
        }
        
        CheckAll();
    }
    public bool CanShine() => canShine;
    void DestroySelf(bool onlyStar = false)
    {
        if (onlyStar)
        {
            foreach (var obj in star_slide)
                Destroy(obj);
        }
        else
        {
            if (ConnectInfo.Parent != null)
                Destroy(ConnectInfo.Parent);

            foreach (GameObject obj in slideBars)
                obj.SetActive(false);

            for (var i = 0; i < star_slide.Length; i++)
                Destroy(star_slide[i]);
            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        if (isDestroying || GameObject.Find("Server").GetComponent<HttpHandler>().IsReloding)
            return;
        if (ConnectInfo.Parent != null)
            Destroy(ConnectInfo.Parent);

        if (ConnectInfo.IsGroupPartEnd)
        {
            // 只有组内最后一个Slide完成 才会显示判定条并增加总数
            if (isBreak)
                GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>().breakCount++;
            else
                GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>().slideCount++;
            slideOK.SetActive(true);
        }
        foreach (var sensor in sensors)
        {
            var s = sensor.GetComponent<Sensor>();
            if (s != null)
            { 
                foreach(var id in guids.Values)
                    s.SetOff(id);
            }
            
        }

        isDestroying = true;
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