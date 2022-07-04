using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDrop : MonoBehaviour
{
    public float time;
    public float speed = 7;
    public char areaPosition;
    public bool isEach;
    public bool isFirework;

    public int startPosition;

    public GameObject tapEffect;
    public GameObject justEffect;
    public GameObject fireworkEffect;
    public GameObject multTouchEffect2;
    public GameObject multTouchEffect3;

    public Sprite faneachSprite;
    public Sprite pointEachSprite;
    public Sprite multTouch2EachSprite;
    public Sprite multTouch3EachSprite;
    AudioTimeProvider timeProvider;
    MultTouchHandler multTouchHandler;

    public GameObject[] fans;
    SpriteRenderer[] fansSprite = new SpriteRenderer[7];

    private float wholeDuration;
    private float moveDuration;
    private float displayDuration;
    private int layer = 0;
    private bool isStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        wholeDuration = 3.209385682f * Mathf.Pow(speed, -0.9549621752f);
        moveDuration = 0.8f * wholeDuration;
        displayDuration = 0.2f * wholeDuration;

        var notes = GameObject.Find("Notes").transform;
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        multTouchHandler = GameObject.Find("MultTouchHandler").GetComponent<MultTouchHandler>();
        for (int i = 0; i < 7; i++)
        {
            fansSprite[i] = fans[i].GetComponent<SpriteRenderer>();
        }
        if (isEach) { 
            SetfanSprite(faneachSprite);
            fansSprite[4].sprite = pointEachSprite;
            fansSprite[5].sprite = multTouch2EachSprite;
            fansSprite[6].sprite = multTouch3EachSprite;
        }
        if (isFirework) { 
            fireworkEffect = Instantiate(fireworkEffect, transform.position, transform.rotation);
            fireworkEffect.SetActive(false);
        }
        justEffect.SetActive(false);
        transform.position = GetAreaPos(startPosition, areaPosition);
        SetfanColor(new Color(1f, 1f, 1f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        var timing = timeProvider.AudioTime - time;
        
        //var timing = time;
        //var pow = Mathf.Pow(-timing * speed, 0.1f)-0.4f;
        var pow = -Mathf.Exp(8 * (timing*0.4f/moveDuration) - 0.85f) + 0.42f;
        var distance = Mathf.Clamp(pow, 0f, 0.4f);

        if (timing > 0.05f)
        {
            multTouchHandler.cancelTouch(this);
            Instantiate(tapEffect, transform.position, transform.rotation);
            GameObject.Find("ObjectCount").GetComponent<ObjectCount>().touchCount++;
            Destroy(gameObject);
        }
        if (timing > 0f)
        {
            fireworkEffect.SetActive(true);
            justEffect.SetActive(true);
        }

        if (-timing <= wholeDuration && -timing > moveDuration)
        {
            if (!isStarted)
            {
                isStarted = true;
                multTouchHandler.registerTouch(this);
            }
            SetfanColor(new Color(1f, 1f, 1f, Mathf.Clamp((wholeDuration+timing)/displayDuration, 0f, 1f)));
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
        for (int i = 0; i < 4; i++)
        {
            var pos = (0.226f + distance) * GetAngle(i);
            fans[i].transform.localPosition = pos;
        }

    }

    public void setLayer(int newLayer)
    {
        layer = newLayer;
        if (layer == 1)
        {
            multTouchEffect2.SetActive(true);
            multTouchEffect3.SetActive(false);
        } else if (layer == 2)
        {
            multTouchEffect2.SetActive(false);
            multTouchEffect3.SetActive(true);
        } else
        {
            multTouchEffect2.SetActive(false);
            multTouchEffect3.SetActive(false);
        }
    }

    public void layerDown()
    {
        setLayer(layer - 1);
    }

    Vector3 GetAngle(int index)
    {
        var angle = (index * (Mathf.PI / 2));
        return new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
    }
    Vector3 GetAreaPos(int index,char area)
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
            var angle = (-index * (Mathf.PI / 4)) + ((Mathf.PI * 5) / 8);
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle))*2.3f;
        }
        if (area == 'A')
        {
            var angle = (-index * (Mathf.PI / 4)) + ((Mathf.PI * 5) / 8);
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 4.8f;
        }
        if (area == 'E')
        {
            var angle = (-index * (Mathf.PI / 4)) + ((Mathf.PI * 6) / 8);
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 3.0f;
        }
        if (area == 'D')
        {
            var angle = (-index * (Mathf.PI / 4)) + ((Mathf.PI * 6) / 8);
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 4.8f;
        }
        return Vector3.zero;
    }

    void SetfanColor(Color color)
    {
        foreach (var fan in fansSprite)
        {
            fan.color = color;
        }
    }
    void SetfanSprite(Sprite sprite)
    {
        for (int i = 0; i < 4; i++)
        {
            fansSprite[i].sprite = sprite;
        }
    }
}
