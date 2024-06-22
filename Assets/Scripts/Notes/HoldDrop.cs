using System;
using UnityEngine;
using static NoteEffectManager;
using static Sensor;

public class HoldDrop : NoteLongDrop
{
    public bool isEX;
    public bool isBreak;

    public Sprite tapSpr;
    public Sprite holdOnSpr;
    public Sprite holdOffSpr;
    public Sprite eachSpr;
    public Sprite eachHoldOnSpr;
    public Sprite exSpr;
    public Sprite breakSpr;
    public Sprite breakHoldOnSpr;

    public Sprite eachLine;
    public Sprite breakLine;

    public Sprite holdEachEnd;
    public Sprite holdBreakEnd;

    public RuntimeAnimatorController HoldShine;
    public RuntimeAnimatorController BreakShine;

    public GameObject tapLine;

    public Color exEffectTap;
    public Color exEffectEach;
    public Color exEffectBreak;
    private Animator animator;

    public Material breakMaterial;

    private SpriteRenderer exSpriteRender;
    private bool holdAnimStart;
    private SpriteRenderer holdEndRender;
    private SpriteRenderer lineSpriteRender;

    private SpriteRenderer spriteRenderer;

    InputManager inputManager;

    private void Start()
    {
        var notes = GameObject.Find("Notes").transform;
        noteManager = notes.GetComponent<NoteManager>();
        holdEffect = Instantiate(holdEffect, notes);
        holdEffect.SetActive(false);

        tapLine = Instantiate(tapLine, notes);
        tapLine.SetActive(false);
        lineSpriteRender = tapLine.GetComponent<SpriteRenderer>();

        exSpriteRender = transform.GetChild(0).GetComponent<SpriteRenderer>();

        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        holdEndRender = transform.GetChild(1).GetComponent<SpriteRenderer>();

        spriteRenderer.sortingOrder += noteSortOrder;
        exSpriteRender.sortingOrder += noteSortOrder;
        holdEndRender.sortingOrder += noteSortOrder;

        spriteRenderer.sprite = tapSpr;
        exSpriteRender.sprite = exSpr;

        var anim = gameObject.AddComponent<Animator>();
        anim.enabled = false;
        animator = anim;

        if (isEX) exSpriteRender.color = exEffectTap;
        if (isEach)
        {
            spriteRenderer.sprite = eachSpr;
            lineSpriteRender.sprite = eachLine;
            holdEndRender.sprite = holdEachEnd;
            if (isEX) exSpriteRender.color = exEffectEach;
        }

        if (isBreak)
        {
            spriteRenderer.sprite = breakSpr;
            lineSpriteRender.sprite = breakLine;
            holdEndRender.sprite = holdBreakEnd;
            if (isEX) exSpriteRender.color = exEffectBreak;
            spriteRenderer.material = breakMaterial;
        }

        spriteRenderer.forceRenderingOff = true;
        exSpriteRender.forceRenderingOff = true;
        holdEndRender.enabled = false;

        sensor = GameObject.Find("Sensors")
                                   .transform.GetChild(startPosition - 1)
                                   .GetComponent<Sensor>();
        manager = GameObject.Find("Sensors")
                                .GetComponent<SensorManager>();
        inputManager = GameObject.Find("Input")
                                 .GetComponent<InputManager>();
        sensor.OnSensorStatusChange += Check;
        inputManager.OnSensorStatusChange += Check;
    }
    private void FixedUpdate()
    {
        var autoPlay = GameObject.Find("Input").GetComponent<InputManager>().AutoPlay;

        if (GetRemainingTime() == 0 && isJudged)
        {
            userHold.Stop();
            Destroy(tapLine);
            Destroy(holdEffect);
            Destroy(gameObject);
        }
        else if (isJudged)
        {
            if (sensor.Status == SensorStatus.On)
                PlayHoldEffect();
            if (GetJudgeTiming() < 0.1f || (GetRemainingTime() < 0.2f && GetRemainingTime() != 0 ))
                return;
            if(sensor.Status == SensorStatus.On)
            {
                if(!userHold.IsRunning)
                    userHold.Start();
            }
            else if(sensor.Status == SensorStatus.Off)
            {
                if (userHold.IsRunning)
                    userHold.Stop();
                StopHoldEffect();
            }
        }
        else if (GetJudgeTiming() > 0.15f)
        {
            judgeDiff = 150;
            judgeResult = JudgeType.Miss;
            sensor.OnSensorStatusChange -= Check;
            inputManager.OnSensorStatusChange -= Check;
            isJudged = true;
            GameObject.Find("Notes").GetComponent<NoteManager>().noteCount[startPosition]++;
        }

        if(autoPlay)
        {
            if(GetJudgeTiming() > 0 && !isJudged ||
                isJudged && GetRemainingTime() > 0)
            {
                manager.SetSensorOn(sensor.Type, guid);
                isAutoTrigger = true;
            }
            else if(GetRemainingTime() == 0)
                manager.SetSensorOff(sensor.Type, guid);
        }
        else if(isAutoTrigger)
        {
            manager.SetSensorOff(sensor.Type, guid);
            isAutoTrigger = false;
        }
    }
    void Check(SensorType s, SensorStatus oStatus, SensorStatus nStatus)
    {
        if (s != sensor.Type)
            return;
        if (isJudged || !noteManager.CanJudge(gameObject, startPosition))
            return;
        if (oStatus == SensorStatus.Off && nStatus == SensorStatus.On)
        {
            if (sensor.IsJudging)
                return;
            else
                sensor.IsJudging = true;
            Judge(); 
        }

        if(isJudged)
        {
            sensor.OnSensorStatusChange -= Check;
            inputManager.OnSensorStatusChange -= Check;
            GameObject.Find("Notes").GetComponent<NoteManager>().noteCount[startPosition]++;
        }
    }
    void Judge()
    {

        const int JUDGE_GOOD_AREA = 150;
        const int JUDGE_GREAT_AREA = 100;
        const int JUDGE_PERFECT_AREA = 50;

        const float JUDGE_SEG_PERFECT1 = 16.66667f;
        const float JUDGE_SEG_PERFECT2 = 33.33334f;
        const float JUDGE_SEG_GREAT1 = 66.66667f;
        const float JUDGE_SEG_GREAT2 = 83.33334f;

        if (isJudged)
            return;

        var timing = timeProvider.AudioTime - time;
        var isFast = timing < 0;
        var diff = MathF.Abs(timing * 1000);
        JudgeType result;
        if (diff > JUDGE_GOOD_AREA && isFast)
            return;
        else if (diff < JUDGE_SEG_PERFECT1)
            result = JudgeType.Perfect;
        else if (diff < JUDGE_SEG_PERFECT2)
            result = JudgeType.LatePerfect1;
        else if (diff < JUDGE_PERFECT_AREA)
            result = JudgeType.LatePerfect2;
        else if (diff < JUDGE_SEG_GREAT1)
            result = JudgeType.LateGreat;
        else if (diff < JUDGE_SEG_GREAT2)
            result = JudgeType.LateGreat1;
        else if (diff < JUDGE_GREAT_AREA)
            result = JudgeType.LateGreat;
        else if (diff < JUDGE_GOOD_AREA)
            result = JudgeType.LateGood;
        else
            result = JudgeType.Miss;

        if (result != JudgeType.Miss && isFast)
            result = 14 - result;
        if (result != JudgeType.Miss && isEX)
            result = JudgeType.Perfect;
        if (isFast)
            judgeDiff = 0;
        else
            judgeDiff = diff;
        if (!userHold.IsRunning)
            userHold.Start();
        PlayHoldEffect();
        judgeResult = result;
        isJudged = true;
    }
    // Update is called once per frame
    private void Update()
    {
        var timing = timeProvider.AudioTime - time;
        var distance = timing * speed + 4.8f;
        var destScale = distance * 0.4f + 0.51f;
        if (destScale < 0f)
        {
            destScale = 0f;
            return;
        }

        spriteRenderer.forceRenderingOff = false;
        if (isEX) exSpriteRender.forceRenderingOff = false;

        spriteRenderer.size = new Vector2(1.22f, 1.4f);

        var holdTime = timing - LastFor;
        var holdDistance = holdTime * speed + 4.8f;
        if (holdTime >= 0 || 
            holdTime >= 0 && LastFor <= 0.15f)
        {
            return;
        }


        transform.rotation = Quaternion.Euler(0, 0, -22.5f + -45f * (startPosition - 1));
        tapLine.transform.rotation = transform.rotation;
        holdEffect.transform.position = getPositionFromDistance(4.8f);

        if (isBreak &&
            !holdAnimStart && 
            !isJudged)
        {
            var extra = Math.Max(Mathf.Sin(timeProvider.GetFrame() * 0.17f) * 0.5f, 0);
            spriteRenderer.material.SetFloat("_Brightness", 0.95f + extra);
        }


        if (destScale > 0.3f) tapLine.SetActive(true);

        if (distance < 1.225f)
        {
            transform.localScale = new Vector3(destScale, destScale);
            spriteRenderer.size = new Vector2(1.22f, 1.42f);
            distance = 1.225f;
            var pos = getPositionFromDistance(distance);
            transform.position = pos;            
        }
        else
        {
            if (holdDistance < 1.225f && distance >= 4.8f) // 头到达 尾未出现
            {
                holdDistance = 1.225f;
                distance = 4.8f;
            }
            else if (holdDistance < 1.225f && distance < 4.8f) // 头未到达 尾未出现
            {
                holdDistance = 1.225f;
            }
            else if (holdDistance >= 1.225f && distance >= 4.8f) // 头到达 尾出现
            {
                distance = 4.8f;

                holdEndRender.enabled = true;
            }
            else if (holdDistance >= 1.225f && distance < 4.8f) // 头未到达 尾出现
            {
                holdEndRender.enabled = true;
            }

            var dis = (distance - holdDistance) / 2 + holdDistance;
            transform.position = getPositionFromDistance(dis); //0.325
            var size = distance - holdDistance + 1.4f;
            spriteRenderer.size = new Vector2(1.22f, size);
            holdEndRender.transform.localPosition = new Vector3(0f, 0.6825f - size / 2);
            transform.localScale = new Vector3(1f, 1f);
        }

        var lineScale = Mathf.Abs(distance / 4.8f);
        lineScale = lineScale >= 1f ? 1f : lineScale;
        tapLine.transform.localScale = new Vector3(lineScale, lineScale, 1f);
        exSpriteRender.size = spriteRenderer.size;
    }
    private void OnDestroy()
    {
        if (GameObject.Find("Server").GetComponent<HttpHandler>().IsReloding)
            return;
        var realityHT = LastFor - 0.3f - (judgeDiff / 1000f);
        var percent = MathF.Min(1, (userHold.ElapsedMilliseconds  / 1000f) / realityHT);
        JudgeType result = judgeResult;
        if(realityHT > 0)
        {
            if (percent >= 0.95f)
            {
                if(judgeResult == JudgeType.Miss)
                    result = JudgeType.LateGood;
                else if (MathF.Abs((int)judgeResult - 7) == 6)
                    result = (int)judgeResult < 7 ? JudgeType.LateGreat : JudgeType.FastGreat;
                else
                    result = judgeResult;
            }
            else if (percent >= 0.67f)
            {
                if (judgeResult == JudgeType.Miss)
                    result = JudgeType.LateGood;
                else if (MathF.Abs((int)judgeResult - 7) == 6)
                    result = (int)judgeResult < 7 ? JudgeType.LateGreat : JudgeType.FastGreat;
                else if (judgeResult == JudgeType.Perfect)
                    result = (int)judgeResult < 7 ? JudgeType.LatePerfect1 : JudgeType.FastPerfect1;
            }
            else if (percent >= 0.33f)
            {
                if (MathF.Abs((int)judgeResult - 7) >= 6)
                    result = (int)judgeResult < 7 ? JudgeType.LateGood : JudgeType.FastGood;
                else
                    result = (int)judgeResult < 7 ? JudgeType.LateGreat : JudgeType.FastGreat;
            }
            else if (percent >= 0.05f)
                result = (int)judgeResult < 7 ? JudgeType.LateGood : JudgeType.FastGood;
            else if (percent >= 0)
            {
                if (judgeResult == JudgeType.Miss)
                    result = JudgeType.Miss;
                else
                    result = (int)judgeResult < 7 ? JudgeType.LateGood : JudgeType.FastGood;
            }
        }
        var effectManager = GameObject.Find("NoteEffects").GetComponent<NoteEffectManager>();
        effectManager.PlayEffect(startPosition, isBreak, result);
        effectManager.PlayFastLate(startPosition, result);
        print($"Hold: {MathF.Round(percent * 100,2)}%\nTotal Len : {MathF.Round(realityHT * 1000,2)}ms");
        if (isBreak)
            GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>().breakCount++;
        else
            GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>().holdCount++;
        if (GameObject.Find("Input").GetComponent<InputManager>().AutoPlay)
            manager.SetSensorOff(sensor.Type, guid);
        if(!isJudged)
            GameObject.Find("Notes").GetComponent<NoteManager>().noteCount[startPosition]++;
        sensor.OnSensorStatusChange -= Check;
        inputManager.OnSensorStatusChange -= Check;
    }
    protected override void PlayHoldEffect()
    {
        base.PlayHoldEffect();
        GameObject.Find("NoteEffects").GetComponent<NoteEffectManager>().ResetEffect(startPosition);
        if (LastFor <= 0.3)
            return;
        else if (!holdAnimStart && timeProvider.AudioTime - time > 0.1)//忽略开头6帧与结尾12帧
        {
            holdAnimStart = true;
            animator.runtimeAnimatorController = HoldShine;
            animator.enabled = true;
            var sprRenderer = GetComponent<SpriteRenderer>();
            if (isBreak)
                sprRenderer.sprite = breakHoldOnSpr;
            else if (isEach)
                sprRenderer.sprite = eachHoldOnSpr;
            else
                sprRenderer.sprite = holdOnSpr;
        }
    }
    protected override void StopHoldEffect()
    {
        base.StopHoldEffect();
        holdAnimStart = false;
        animator.runtimeAnimatorController = HoldShine;
        animator.enabled = false;
        var sprRenderer = GetComponent<SpriteRenderer>();
        sprRenderer.sprite = holdOffSpr;
    }

}