using Assets.Scripts.Notes;
using UnityEngine;

public class StarDrop : TapBase
{
    public float rotateSpeed = 1f;

    public bool isDouble;
    public bool isNoHead;

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
            sensor.OnSensorStatusChange += Check;
            inputManager.OnSensorStatusChange += Check;
        }
    }
    // Update is called once per frame
    protected override void Update()
    {
        var songSpeed = timeProvider.CurrentSpeed;
        var timing = timeProvider.AudioTime - time;
        var distance = timing * speed + 4.8f;

        if (timing > 0 && isNoHead)
        {
            Destroy(tapLine);
            Destroy(gameObject);
            return;
        }

        base.Update();
        if (isNoHead)
        {
            spriteRenderer.forceRenderingOff = true;
            if (isEX) exSpriteRender.forceRenderingOff = true;
            tapLine.SetActive(false);
        }
        if (distance >= 1.225f)
            if (!slide.activeSelf) slide.SetActive(true);
        if (timeProvider.isStart)
            transform.Rotate(0f, 0f, -180f * Time.deltaTime * songSpeed / rotateSpeed);
        if (timing > 0 && GameObject.Find("Input").GetComponent<InputManager>().AutoPlay)
        {
            manager.SetSensorOn(sensor.Type, guid);

            if (timing > 0.02)
            {
                Destroy(tapLine);
                Destroy(gameObject);
            }
        }
    }
    protected override void OnDestroy()
    {
        if(!isNoHead)
            base.OnDestroy();
    }
}