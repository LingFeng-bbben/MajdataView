using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHoldDrop : MonoBehaviour
{
    public float time;
    public float lastFor = 1f;
    public float speed = 1;
    public bool isFirework;

    public GameObject tapEffect;
    public GameObject holdEffect;

    AudioTimeProvider timeProvider;

    GameObject firework;
    Animator fireworkEffect;

    public Sprite[] TouchHoldSprite = new Sprite[5];
    public Sprite TouchPointSprite;

    public GameObject[] fans;
    SpriteRenderer[] fansSprite = new SpriteRenderer[6];
    public SpriteMask mask;

    private float wholeDuration;
    private float moveDuration;
    private float displayDuration;

    // Start is called before the first frame update
    void Start()
    {
        wholeDuration = 3.209385682f * Mathf.Pow(speed, -0.9549621752f);
        moveDuration = 0.8f * wholeDuration;
        displayDuration = 0.2f * wholeDuration;

        var notes = GameObject.Find("Notes").transform;
        holdEffect = Instantiate(holdEffect, notes);
        holdEffect.SetActive(false);

        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();

        firework = GameObject.Find("Firework");
        fireworkEffect = firework.GetComponent<Animator>();

        for (int i = 0; i < 6; i++)
        {
            fansSprite[i] = fans[i].GetComponent<SpriteRenderer>();
        }

        for (int i = 0; i < 4; i++)
        {
            fansSprite[i].sprite = TouchHoldSprite[i];
        }
        fansSprite[5].sprite = TouchHoldSprite[4];      // TouchHold Border
        fansSprite[4].sprite = TouchPointSprite;

        SetfanColor(new Color(1f, 1f, 1f, 0f));
        mask.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        var timing = timeProvider.AudioTime - time;
        //var pow = Mathf.Pow(-timing * speed, 0.1f) - 0.4f;
        var pow = -Mathf.Exp(8 * (timing * 0.4f / moveDuration) - 0.85f) + 0.42f;
        var distance = Mathf.Clamp(pow, 0f, 0.4f);

        if (timing > lastFor)
        {
            Instantiate(tapEffect, transform.position, transform.rotation);
            GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>().holdCount++;
            if (isFirework)
            {
                fireworkEffect.SetTrigger("Fire");
                firework.transform.position = transform.position;
            }
            Destroy(holdEffect);
            Destroy(gameObject);
        }

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
            mask.alphaCutoff = Mathf.Clamp(0.91f * (1 - ((lastFor - timing) / lastFor)), 0f, 1f);
        }
        
        if (float.IsNaN(distance)) distance = 0f;
        if(distance==0f) holdEffect.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            var pos = (0.226f + distance) * GetAngle(i);
            fans[i].transform.position = pos;
        }

        

    }

    Vector3 GetAngle(int index)
    {
        var angle = (Mathf.PI / 4) + (index * (Mathf.PI / 2));
        return new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
    }

    void SetfanColor(Color color)
    {
        foreach (var fan in fansSprite)
        {
            fan.color =color;
        }
    }
}
