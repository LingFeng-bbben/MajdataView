using UnityEngine;

public class StarDrop : NoteDrop
{
    // Start is called before the first frame update
    // public float time;
    public int startPosition = 1;
    public float speed = 1;
    public float rotateSpeed = 1f;

    public bool isEach;
    public bool isBreak;
    public bool isDouble;
    public bool isEX;
    public bool isNoHead;

    public Sprite tapSpr;
    public Sprite eachSpr;
    public Sprite breakSpr;
    public Sprite exSpr;

    public Sprite tapSpr_Double;
    public Sprite eachSpr_Double;
    public Sprite breakSpr_Double;
    public Sprite exSpr_Double;

    public Sprite eachLine;
    public Sprite breakLine;

    public RuntimeAnimatorController BreakShine;

    public GameObject slide;
    public GameObject tapLine;

    public Color exEffectTap;
    public Color exEffectEach;
    public Color exEffectBreak;
    private Animator animator;

    private bool breakAnimStart;
    private SpriteRenderer exSpriteRender;
    private SpriteRenderer lineSpriteRender;

    private ObjectCounter ObjectCounter;

    private SpriteRenderer spriteRenderer;

    private AudioTimeProvider timeProvider;

    private void Start()
    {
        var notes = GameObject.Find("Notes").transform;
        tapLine = Instantiate(tapLine, notes);
        tapLine.SetActive(false);
        lineSpriteRender = tapLine.GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        exSpriteRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        ObjectCounter = GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>();

        var sortOrder = (int)(time * -100);
        spriteRenderer.sortingOrder = sortOrder;
        exSpriteRender.sortingOrder = sortOrder;

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
                var anim = gameObject.AddComponent<Animator>(); // break star闪烁
                anim.runtimeAnimatorController = BreakShine;
                anim.enabled = false;
                animator = anim;
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
                var anim = gameObject.AddComponent<Animator>(); // break star闪烁
                anim.runtimeAnimatorController = BreakShine;
                anim.enabled = false;
                animator = anim;
            }
        }

        spriteRenderer.forceRenderingOff = true;
        exSpriteRender.forceRenderingOff = true;
    }

    // Update is called once per frame
    private void Update()
    {
        var timing = timeProvider.AudioTime - time;
        var distance = timing * speed + 4.8f;
        var destScale = distance * 0.4f + 0.51f;
        var songSpeed = timeProvider.CurrentSpeed;
        if (destScale < 0f)
        {
            destScale = 0f;
            return;
        }

        if (!isNoHead)
        {
            spriteRenderer.forceRenderingOff = false;
            if (isEX) exSpriteRender.forceRenderingOff = false;
        }

        if (isBreak && !breakAnimStart)
        {
            breakAnimStart = true;
            animator.enabled = true;
            animator.Play("BreakShine", -1, 0.5f);
        }

        if (timing > 0)
        {
            if (!isNoHead)
            {
                GameObject.Find("NoteEffects").GetComponent<NoteEffectManager>().PlayEffect(startPosition, isBreak);
                if (isBreak) ObjectCounter.breakCount++;
                else ObjectCounter.tapCount++;
            }

            Destroy(tapLine);
            Destroy(gameObject);
        }

        if (timeProvider.isStart)
            transform.Rotate(0f, 0f, -180f * Time.deltaTime * songSpeed / rotateSpeed);

        tapLine.transform.rotation = Quaternion.Euler(0, 0, -22.5f + -45f * (startPosition - 1));

        if (distance < 1.225f)
        {
            transform.localScale = new Vector3(destScale, destScale);

            distance = 1.225f;
            var pos = getPositionFromDistance(distance);
            transform.position = pos;
            if (destScale > 0.3f && !isNoHead) tapLine.SetActive(true);
        }
        else
        {
            if (!slide.activeSelf) slide.SetActive(true);
            var pos = getPositionFromDistance(distance);
            transform.position = pos;
            transform.localScale = new Vector3(1f, 1f);
        }

        var lineScale = Mathf.Abs(distance / 4.8f);
        tapLine.transform.localScale = new Vector3(lineScale, lineScale, 1f);
        //lineSpriteRender.color = new Color(1f, 1f, 1f, lineScale);
    }

    private Vector3 getPositionFromDistance(float distance)
    {
        return new Vector3(
            distance * Mathf.Cos((startPosition * -2f + 5f) * 0.125f * Mathf.PI),
            distance * Mathf.Sin((startPosition * -2f + 5f) * 0.125f * Mathf.PI));
    }
}