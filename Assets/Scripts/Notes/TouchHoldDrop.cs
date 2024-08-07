using Assets.Scripts.Types;
using System;
using UnityEngine;
#nullable enable
public class TouchHoldDrop : NoteLongDrop
{
    public bool isFirework;
    public GameObject tapEffect;
    public GameObject judgeEffect;

    public Sprite touchHoldBoard;
    public Sprite touchHoldBoard_Miss;
    public SpriteRenderer boarder;
    public Sprite[] TouchHoldSprite = new Sprite[5];
    public Sprite TouchPointSprite;

    public GameObject[] fans;
    public SpriteMask mask;
    private readonly SpriteRenderer[] fansSprite = new SpriteRenderer[6];
    private float displayDuration;

    private GameObject firework;
    private Animator fireworkEffect;
    private float moveDuration;

    private float wholeDuration;

    Sprite[] judgeText;

    // Start is called before the first frame update
    private void Start()
    {
        wholeDuration = 3.209385682f * Mathf.Pow(speed, -0.9549621752f);
        moveDuration = 0.8f * wholeDuration;
        displayDuration = 0.2f * wholeDuration;

        objectCounter = GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>();
        var notes = GameObject.Find("Notes").transform;
        noteManager = notes.GetComponent<NoteManager>();
        holdEffect = Instantiate(holdEffect, notes);
        holdEffect.SetActive(false);

        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();

        firework = GameObject.Find("FireworkEffect");
        fireworkEffect = firework.GetComponent<Animator>();

        for (var i = 0; i < 6; i++)
        {
            fansSprite[i] = fans[i].GetComponent<SpriteRenderer>();
            fansSprite[i].sortingOrder += noteSortOrder;
        }

        for (var i = 0; i < 4; i++) fansSprite[i].sprite = TouchHoldSprite[i];
        fansSprite[5].sprite = TouchHoldSprite[4]; // TouchHold Border
        fansSprite[4].sprite = TouchPointSprite;

        SetfanColor(new Color(1f, 1f, 1f, 0f));
        mask.enabled = false;

        sensor = GameObject.Find("Sensors")
                                   .transform.GetChild(16)
                                   .GetComponent<Sensor>();
        manager = GameObject.Find("Sensors")
                                .GetComponent<SensorManager>();
        var customSkin = GameObject.Find("Outline").GetComponent<CustomSkin>();
        judgeText = customSkin.JudgeText;
        sensor.OnStatusChanged += Check;
    }
    void Check(object sender, InputEventArgs arg)
    {
        if (isJudged || !noteManager.CanJudge(gameObject, sensor.Type))
            return;
        else if (arg.IsClick)
        {
            if (sensor.IsJudging)
                return;
            else
                sensor.IsJudging = true;
            Judge();
            if (isJudged)
            {
                sensor.OnStatusChanged -= Check;
                GameObject.Find("Notes").GetComponent<NoteManager>().touchIndex[SensorType.C]++;
            }
        }
    }
    void Judge()
    {

        const float JUDGE_GOOD_AREA = 316.667f;
        const int JUDGE_GREAT_AREA = 250;
        const int JUDGE_PERFECT_AREA = 200;

        const float JUDGE_SEG_PERFECT = 150f;

        if (isJudged)
            return;

        var timing = timeProvider.AudioTime - time;
        var isFast = timing < 0;
        var diff = MathF.Abs(timing * 1000);
        JudgeType result;
        if (diff > JUDGE_SEG_PERFECT && isFast)
            return;
        else if (diff < JUDGE_SEG_PERFECT)
            result = JudgeType.Perfect;
        else if (diff < JUDGE_PERFECT_AREA)
            result = JudgeType.LatePerfect2;
        else if (diff < JUDGE_GREAT_AREA)
            result = JudgeType.LateGreat;
        else if (diff < JUDGE_GOOD_AREA)
            result = JudgeType.LateGood;
        else
            result = JudgeType.Miss;
        if (isFast)
            judgeDiff = 0;
        else
            judgeDiff = diff;

        judgeResult = result;
        isJudged = true;
        PlayHoldEffect();
    }
    private void FixedUpdate()
    {
        var autoPlay = InputManager.AutoPlay;
        var remainingTime = GetRemainingTime();
        var timing = GetJudgeTiming();
        var holdTime = timing - LastFor;
        var on = sensor.Status == SensorStatus.On;

        if (remainingTime == 0 && isJudged)
        {
            Destroy(holdEffect);
            Destroy(gameObject);
        }
        else if (isJudged)
        {
            if (remainingTime < 0.2f && remainingTime != 0)
                return;
            else if (timing > 0.25f)
            {
                if(on)
                {
                    userHoldTime += Time.fixedDeltaTime;
                    PlayHoldEffect();
                }
                else
                    StopHoldEffect();
            }
        }
        else if (timing > 0.316667f)
        {
            judgeDiff = 316.667f;
            judgeResult = JudgeType.Miss;
            sensor.OnStatusChanged -= Check;
            isJudged = true;
            objectCounter.NextTouch(SensorType.C);
        }

        if (autoPlay)
        {
            if (timing > 0 && !isJudged ||
                isJudged && remainingTime > 0)
            {
                manager.SetSensorOn(sensor.Type, guid);
                isAutoTrigger = true;
            }
            else if (timing == 0)
                manager.SetSensorOff(sensor.Type, guid);
        }
        else if (isAutoTrigger)
        {
            manager.SetSensorOff(sensor.Type, guid);
            isAutoTrigger = false;
        }

    }
    // Update is called once per frame
    private void Update()
    {
        var timing = GetJudgeTiming();
        var pow = -Mathf.Exp(8 * (timing * 0.4f / moveDuration) - 0.85f) + 0.42f;
        var distance = Mathf.Clamp(pow, 0f, 0.4f);

        if (-timing <= wholeDuration && -timing > moveDuration)
        {
            SetfanColor(new Color(1f, 1f, 1f, Mathf.Clamp((wholeDuration + timing) / displayDuration, 0f, 1f)));
            fans[5].SetActive(false);
            mask.enabled = false;
        }
        else if (-timing < moveDuration)
        {
            fans[5].SetActive(true);
            mask.enabled = true;
            SetfanColor(Color.white);
            mask.alphaCutoff = Mathf.Clamp(0.91f * (1 - (LastFor - timing) / LastFor), 0f, 1f);
        }

        if (float.IsNaN(distance)) distance = 0f;

        for (var i = 0; i < 4; i++)
        {
            var pos = (0.226f + distance) * GetAngle(i);
            fans[i].transform.position = pos;
        }
    }
    private void OnDestroy()
    {
        if (HttpHandler.IsReloding)
            return;
        var realityHT = LastFor - 0.45f - (judgeDiff / 1000f);
        var percent = MathF.Min(1, userHoldTime / realityHT);
        JudgeType result = judgeResult;
        if (realityHT > 0)
        {
            if (percent >= 0.95f)
                result = judgeResult;
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
        
        print($"TouchHold: {MathF.Round(percent * 100, 2)}%\nTotal Len : {MathF.Round(realityHT * 1000, 2)}ms");
        objectCounter.ReportResult(this, result);
        if (!isJudged)
            objectCounter.NextTouch(SensorType.C);
        if (isFirework && result != JudgeType.Miss)
        {
            fireworkEffect.SetTrigger("Fire");
            firework.transform.position = transform.position;
        }
        manager.SetSensorOff(sensor.Type, guid);
        PlayJudgeEffect(result);
    }

    protected override void PlayHoldEffect()
    {
        base.PlayHoldEffect();
        boarder.sprite = touchHoldBoard;
    }
    void PlayJudgeEffect(JudgeType judgeResult)
    {
        var obj = Instantiate(judgeEffect, Vector3.zero, transform.rotation);
        var _obj = Instantiate(judgeEffect, Vector3.zero, transform.rotation);
        var judgeObj = obj.transform.GetChild(0);
        var flObj = _obj.transform.GetChild(0);

        judgeObj.transform.position = new Vector3(0, -0.6f, 0);
        flObj.transform.position = new Vector3(0, -1.08f, 0);
        flObj.GetChild(0).transform.rotation = Quaternion.Euler(Vector3.zero);
        judgeObj.GetChild(0).transform.rotation = Quaternion.Euler(Vector3.zero);
        var anim = obj.GetComponent<Animator>();

        var effects = GameObject.Find("NoteEffects");
        var flAnim = _obj.GetComponent<Animator>();
        GameObject effect;
        switch (judgeResult)
        {
            case JudgeType.LateGood:
            case JudgeType.FastGood:
                judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[1];
                effect = Instantiate(effects.transform.GetChild(3).GetChild(0), transform.position, transform.rotation).gameObject;
                effect.SetActive(true);
                break;
            case JudgeType.LateGreat:
            case JudgeType.LateGreat1:
            case JudgeType.LateGreat2:
            case JudgeType.FastGreat2:
            case JudgeType.FastGreat1:
            case JudgeType.FastGreat:
                judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[2];
                //transform.Rotate(0, 0f, 30f);
                effect = Instantiate(effects.transform.GetChild(2).GetChild(0), transform.position, transform.rotation).gameObject;
                effect.SetActive(true);
                effect.gameObject.GetComponent<Animator>().SetTrigger("great");
                break;
            case JudgeType.LatePerfect2:
            case JudgeType.FastPerfect2:
            case JudgeType.LatePerfect1:
            case JudgeType.FastPerfect1:
                judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[3];
                transform.Rotate(0, 180f, 90f);
                Instantiate(tapEffect, transform.position, transform.rotation);
                break;
            case JudgeType.Perfect:
                judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[4];
                transform.Rotate(0, 180f, 90f);
                Instantiate(tapEffect, transform.position, transform.rotation);
                break;
            case JudgeType.Miss:
                judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[0];
                break;
            default:
                break;
        }
        //judgeEffect.transform.position = new Vector3(0, -0.6f, 0);
        GameObject.Find("NoteEffects").GetComponent<NoteEffectManager>().PlayFastLate(_obj, flAnim, judgeResult);
        anim.SetTrigger("touch");
    }
    protected override void StopHoldEffect()
    {
        base.StopHoldEffect();
        boarder.sprite = touchHoldBoard_Miss;
    }
    private Vector3 GetAngle(int index)
    {
        var angle = Mathf.PI / 4 + index * (Mathf.PI / 2);
        return new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
    }

    private void SetfanColor(Color color)
    {
        foreach (var fan in fansSprite) fan.color = color;
    }
}