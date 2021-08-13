using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDrop : MonoBehaviour
{
    public float time;
    public float speed = 1;
    public char areaPosition;
    public bool isEach;
    public bool isFirework;

    public int startPosition;

    public GameObject tapEffect;
    public GameObject justEffect;
    public GameObject fireworkEffect;

    public Sprite faneachSprite;
    public Sprite pointEachSprite;
    AudioTimeProvider timeProvider;

    public GameObject[] fans;
    SpriteRenderer[] fansSprite = new SpriteRenderer[5];

    // Start is called before the first frame update
    void Start()
    {
        var notes = GameObject.Find("Notes").transform;
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        for (int i = 0; i < 5; i++)
        {
            fansSprite[i] = fans[i].GetComponent<SpriteRenderer>();
        }
        if (isEach) { 
            SetfanSprite(faneachSprite);
            fansSprite[4].sprite = pointEachSprite;
        }
        if (isFirework) { 
            fireworkEffect = Instantiate(fireworkEffect, transform.position, transform.rotation);
            fireworkEffect.SetActive(false);
        }
        justEffect.SetActive(false);
        transform.position = GetAreaPos(startPosition, areaPosition);
    }

    // Update is called once per frame
    void Update()
    {
        var timing = timeProvider.AudioTime - time;
        //var timing = time;
        //var pow = Mathf.Pow(-timing * speed, 0.1f)-0.4f;
        var pow = -Mathf.Exp(8 * timing - 0.6f) + 0.5f;
        var distance = Mathf.Clamp(pow, 0f, 0.4f);

        if (timing > 0.05f)
        {
            Instantiate(tapEffect, transform.position, transform.rotation);
            GameObject.Find("ObjectCount").GetComponent<ObjectCount>().touchCount++;
            Destroy(gameObject);
        }
        if (timing > 0f)
        {
            fireworkEffect.SetActive(true);
            justEffect.SetActive(true);
        }

        if (pow > 0.4f)
        {
            SetfanColor(new Color(1f, 1f, 1f, Mathf.Clamp((timing * 6 + 2.5f), 0f, 1f)));
        }
        else
        {
            SetfanColor(Color.white);
        }

        if (float.IsNaN(distance)) distance = 0f;
        for (int i = 0; i < 4; i++)
        {
            var pos = (0.226f + distance) * GetAngle(i);
            fans[i].transform.localPosition = pos;
        }

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
