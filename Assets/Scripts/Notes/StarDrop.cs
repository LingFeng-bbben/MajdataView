using Assets.Scripts.Notes;
using UnityEngine;
#nullable enable
public class StarDrop : TapBase
{
    public float rotateSpeed = 1f;

    public bool isDouble;
    public bool isNoHead;
    public bool isFakeStar = false;
    public bool isFakeStarRotate = false;

    public Sprite tapSpr_Double;
    public Sprite eachSpr_Double;
    public Sprite breakSpr_Double;
    public Sprite exSpr_Double;

    public GameObject slide;
    private void Start()
    {
        PreLoad();

        if (isDouble)
        {
            exSpriteRender.sprite = exSpr_Double;
            spriteRenderer.sprite = tapSpr_Double;
            if (isEX) exSpriteRender.color = exEffectTap;
            if (isEach)
            {
                lineSpriteRender.sprite = eachLine;
                spriteRenderer.sprite = eachSpr_Double;
                if (isEX) exSpriteRender.color = exEffectEach;
            }

            if (isBreak)
            {
                lineSpriteRender.sprite = breakLine;
                spriteRenderer.sprite = breakSpr_Double;
                if (isEX) exSpriteRender.color = exEffectBreak;
                spriteRenderer.material = breakMaterial;
            }
        }
        else
        {
            exSpriteRender.sprite = exSpr;
            spriteRenderer.sprite = tapSpr;
            if (isEX) exSpriteRender.color = exEffectTap;
            if (isEach)
            {
                lineSpriteRender.sprite = eachLine;
                spriteRenderer.sprite = eachSpr;
                if (isEX) exSpriteRender.color = exEffectEach;
            }

            if (isBreak)
            {
                lineSpriteRender.sprite = breakLine;
                spriteRenderer.sprite = breakSpr;
                if (isEX) exSpriteRender.color = exEffectBreak;
                spriteRenderer.material = breakMaterial;
            }
        }

        spriteRenderer.forceRenderingOff = true;
        exSpriteRender.forceRenderingOff = true;

        if(!isNoHead)
        {
            sensor = GameObject.Find("Sensors")
                                   .transform.GetChild(startPosition - 1)
                                   .GetComponent<Sensor>();
            manager = GameObject.Find("Sensors")
                                    .GetComponent<SensorManager>();
            inputManager = GameObject.Find("Input")
                                 .GetComponent<InputManager>();
            sensor.OnStatusChanged += Check;
            inputManager.OnButtonStatusChanged += Check;
        }
    }
    // Update is called once per frame
    protected override void Update()
    {
        var songSpeed = timeProvider.CurrentSpeed;
        var judgeTiming = GetJudgeTiming();
        var distance = judgeTiming * speed + 4.8f;

        if (judgeTiming > 0 && isNoHead)
        {
            Destroy(tapLine);
            Destroy(gameObject);
            return;
        }

        if (timeProvider.isStart && !isFakeStar)
            transform.Rotate(0f, 0f, -180f * Time.deltaTime * songSpeed / rotateSpeed);
        else if (isFakeStarRotate)
            transform.Rotate(0f, 0f, 400f * Time.deltaTime);  
        
        base.Update();
        if (isNoHead)
        {
            spriteRenderer.forceRenderingOff = true;
            if (isEX) exSpriteRender.forceRenderingOff = true;
            tapLine.SetActive(false);
        }
        if (distance >= 1.225f && !isFakeStar)
            if (!slide.activeSelf) slide.SetActive(true);
        if (judgeTiming > 0 && GameObject.Find("Input").GetComponent<InputManager>().AutoPlay)
            manager.SetSensorOn(sensor.Type, guid);
    }
    protected override void OnDestroy()
    {
        if(!isNoHead || isFakeStar)
            base.OnDestroy();
    }
}