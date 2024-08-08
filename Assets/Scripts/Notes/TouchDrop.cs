using Assets.Scripts;
using Assets.Scripts.Types;
using System;
using Unity.Burst.Intrinsics;
using UnityEngine;
#nullable enable
public class TouchDrop : TouchBase
{
    public GameObject justEffect;

    public GameObject multTouchEffect2;
    public GameObject multTouchEffect3;

    public Sprite fanNormalSprite;
    public Sprite fanEachSprite;

    public Sprite pointNormalSprite;
    public Sprite pointEachSprite;

    public Sprite justSprite;

    public Sprite[] multTouchNormalSprite = new Sprite[2];
    public Sprite[] multTouchEachSprite = new Sprite[2];

    public GameObject[] fans;
    private readonly SpriteRenderer[] fansSprite = new SpriteRenderer[7];
    private float displayDuration;

    private GameObject firework;
    private Animator fireworkEffect;
    private bool isStarted;
    private int layer;
    private float moveDuration;
    bool isTriggered = false;
    private MultTouchHandler multTouchHandler;

    private float wholeDuration;

    // Start is called before the first frame update
    void Start()
    {
        wholeDuration = 3.209385682f * Mathf.Pow(speed, -0.9549621752f);
        moveDuration = 0.8f * wholeDuration;
        displayDuration = 0.2f * wholeDuration;

        var notes = GameObject.Find("Notes").transform;
        noteManager = notes.GetComponent<NoteManager>();
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        multTouchHandler = GameObject.Find("MultTouchHandler").GetComponent<MultTouchHandler>();
        objectCounter = GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>();
        firework = GameObject.Find("FireworkEffect");
        fireworkEffect = firework.GetComponent<Animator>();

        for (var i = 0; i < 7; i++)
        {
            fansSprite[i] = fans[i].GetComponent<SpriteRenderer>();
            fansSprite[i].sortingOrder += noteSortOrder;
        }

        if (isEach)
        {
            SetfanSprite(fanEachSprite);
            fansSprite[4].sprite = pointEachSprite;
            fansSprite[5].sprite = multTouchEachSprite[0];
            fansSprite[6].sprite = multTouchEachSprite[1];
        }
        else
        {
            SetfanSprite(fanNormalSprite);
            fansSprite[4].sprite = pointNormalSprite;
            fansSprite[5].sprite = multTouchNormalSprite[0];
            fansSprite[6].sprite = multTouchNormalSprite[1];
        }

        justEffect.GetComponent<SpriteRenderer>().sprite = justSprite;

        transform.position = GetAreaPos(startPosition, areaPosition);
        justEffect.SetActive(false);
        SetfanColor(new Color(1f, 1f, 1f, 0f));
        sensor = GameObject.Find("Sensors")
                                   .transform.GetChild((int)GetSensor())
                                   .GetComponent<Sensor>();
        manager = GameObject.Find("Sensors")
                                .GetComponent<SensorManager>();
        inputManager = GameObject.Find("Input")
                                 .GetComponent<InputManager>();
        var customSkin = GameObject.Find("Outline").GetComponent<CustomSkin>();
        judgeText = customSkin.JudgeText;
        inputManager.BindSensor(Check, GetSensor());
    }
    void Check(object sender,InputEventArgs arg)
    {
        var type = GetSensor();
        if (arg.Type != type)
            return;
        if (isJudged || !noteManager.CanJudge(gameObject, type))
            return;
        else if (arg.IsClick)
        {
            if (!inputManager.IsIdle(arg))
                return;
            else
                inputManager.SetBusy(arg);
            Judge();
            if (isJudged)
            {
                Destroy(gameObject);
            }
        }
    }
    private void FixedUpdate()
    {
        if (!isJudged && GetJudgeTiming() <= 0.316667f)
        {
            if (GroupInfo is not null)
            {
                if (GroupInfo.Percent > 0.5f && GroupInfo.JudgeResult != null)
                {
                    isJudged = true;
                    judgeResult = (JudgeType)GroupInfo.JudgeResult;
                    Destroy(gameObject);
                }
            }
        }
        else if (!isJudged)
        {
            judgeResult = JudgeType.Miss;
            isJudged = true;
            Destroy(gameObject);
        }
        else if (isJudged)
            Destroy(gameObject);

        if (GetJudgeTiming() >= 0 && InputManager.AutoPlay)
        {
            if (isTriggered)
                return;
            inputManager.ClickSensor(GetSensor());
            isTriggered = true;
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

        judgeResult = result;
        isJudged = true;
    }
    // Update is called once per frame
    private void Update()
    {
        var timing = timeProvider.AudioTime - time;

        //var timing = time;
        //var pow = Mathf.Pow(-timing * speed, 0.1f)-0.4f;
        var pow = -Mathf.Exp(8 * (timing * 0.4f / moveDuration) - 0.85f) + 0.42f;
        var distance = Mathf.Clamp(pow, 0f, 0.4f);

        if (timing >= 0)
        {
            var _pow = -Mathf.Exp(- 0.85f) + 0.42f;
            var _distance = Mathf.Clamp(_pow, 0f, 0.4f);
            for (var i = 0; i < 4; i++)
            {
                var pos = (0.226f + _distance) * GetAngle(i);
                fans[i].transform.localPosition = pos;
            }
            return;
        }

        if (timing > -0.02f) justEffect.SetActive(true);

        if (-timing <= wholeDuration && -timing > moveDuration)
        {
            if (!isStarted)
            {
                isStarted = true;
                multTouchHandler.registerTouch(this);
            }

            SetfanColor(new Color(1f, 1f, 1f, Mathf.Clamp((wholeDuration + timing) / displayDuration, 0f, 1f)));
        }
        else if (-timing < moveDuration)
        {
            if (!isStarted)
            {
                isStarted = true;
                multTouchHandler.registerTouch(this);
            }

            SetfanColor(Color.white);
        }

        if (float.IsNaN(distance)) distance = 0f;
        for (var i = 0; i < 4; i++)
        {
            var pos = (0.226f + distance) * GetAngle(i);
            fans[i].transform.localPosition = pos;
        }
    }
    private void OnDestroy()
    {
        if (HttpHandler.IsReloding)
            return;
        multTouchHandler.cancelTouch(this);
        PlayJudgeEffect();
        if (GroupInfo is not null && judgeResult != JudgeType.Miss)
            GroupInfo.JudgeResult = judgeResult;
        objectCounter.ReportResult(this, judgeResult);
        objectCounter.NextTouch(sensor.Type);

        if (isFirework && judgeResult != JudgeType.Miss)
        {
            fireworkEffect.SetTrigger("Fire");
            firework.transform.position = transform.position;
        }
        inputManager.UnbindSensor(Check, GetSensor());
    }
    void PlayJudgeEffect()
    {
        var obj = Instantiate(judgeEffect, Vector3.zero,transform.rotation);
        var _obj = Instantiate(judgeEffect, Vector3.zero, transform.rotation);
        var judgeObj = obj.transform.GetChild(0);
        var flObj = _obj.transform.GetChild(0);

        if (sensor.Group != SensorGroup.C)
        {
            judgeObj.transform.position = GetPosition(-0.46f);
            flObj.transform.position = GetPosition(-0.92f);
        }
        else
        {
            judgeObj.transform.position = new Vector3(0, -0.6f, 0);
            flObj.transform.position = new Vector3(0, -1.08f, 0);
        }
        judgeObj.GetChild(0).transform.rotation = GetRoation();
        flObj.GetChild(0).transform.rotation = GetRoation();
        var anim = obj.GetComponent<Animator>();
        var flAnim = _obj.GetComponent<Animator>();
        switch(judgeResult)
        {
            case JudgeType.LateGood:
            case JudgeType.FastGood:
                judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[1];
                break;
            case JudgeType.LateGreat:
            case JudgeType.LateGreat1:
            case JudgeType.LateGreat2:
            case JudgeType.FastGreat2:
            case JudgeType.FastGreat1:
            case JudgeType.FastGreat:
                judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[2];
                break;
            case JudgeType.LatePerfect2:
            case JudgeType.FastPerfect2:
            case JudgeType.LatePerfect1:
            case JudgeType.FastPerfect1:
                judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[3];
                break;
            case JudgeType.Perfect:
                judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[4];
                break;
            case JudgeType.Miss:
                judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[0];
                break;
            default:
                break;
        }
        if(judgeResult != JudgeType.Miss)
            Instantiate(tapEffect, transform.position, transform.rotation);

        GameObject.Find("NoteEffects").GetComponent<NoteEffectManager>().PlayFastLate(_obj,flAnim,judgeResult);

        anim.SetTrigger("touch");
    }
    /// <summary>
    /// 获取当前坐标指定距离的坐标
    /// <para>方向：原点</para>
    /// </summary>
    /// <param name="magnitude"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    Vector3 GetPosition(float distance)
    {
        var d = transform.position.magnitude;
        var ratio = MathF.Max(0, d + distance) / d;
        return transform.position * ratio;
    }
    public void setLayer(int newLayer)
    {
        layer = newLayer;
        if (layer == 1)
        {
            multTouchEffect2.SetActive(true);
            multTouchEffect3.SetActive(false);
        }
        else if (layer == 2)
        {
            multTouchEffect2.SetActive(false);
            multTouchEffect3.SetActive(true);
        }
        else
        {
            multTouchEffect2.SetActive(false);
            multTouchEffect3.SetActive(false);
        }
    }
    public void layerDown()
    {
        setLayer(layer - 1);
    }

    private Vector3 GetAngle(int index)
    {
        var angle = index * (Mathf.PI / 2);
        return new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
    }

    private Vector3 GetAreaPos(int index, char area)
    {
        /// <summary>
        /// AreaDistance: 
        /// C:   0
        /// E:   3.1
        /// B:   2.21
        /// A,D: 4.8
        /// </summary>
        if (area == 'C') return Vector3.zero;
        if (area == 'B')
        {
            var angle = -index * (Mathf.PI / 4) + Mathf.PI * 5 / 8;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 2.3f;
        }

        if (area == 'A')
        {
            var angle = -index * (Mathf.PI / 4) + Mathf.PI * 5 / 8;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 4.1f;
        }

        if (area == 'E')
        {
            var angle = -index * (Mathf.PI / 4) + Mathf.PI * 6 / 8;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 3.0f;
        }

        if (area == 'D')
        {
            var angle = -index * (Mathf.PI / 4) + Mathf.PI * 6 / 8;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 4.1f;
        }

        return Vector3.zero;
    }

    private void SetfanColor(Color color)
    {
        foreach (var fan in fansSprite) fan.color = color;
    }

    private void SetfanSprite(Sprite sprite)
    {
        for (var i = 0; i < 4; i++) fansSprite[i].sprite = sprite;
    }
}