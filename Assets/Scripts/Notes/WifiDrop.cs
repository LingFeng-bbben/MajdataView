using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WifiDrop : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject star_slidePrefab;
    GameObject[] star_slide = new GameObject[3];

    SpriteRenderer[] spriteRenderer_star = new SpriteRenderer[3];

    public Sprite[] eachSlide = new Sprite[11];
    public Sprite eachStar;

    public float time;
    public float timeStar;
    public float LastFor = 1f;
    public float speed;
    public bool isEach;

    public int startPosition = 1;


    AudioTimeProvider timeProvider;

    List<GameObject> slideBars = new List<GameObject>();
    List<SpriteRenderer> sbRender = new List<SpriteRenderer>();

    Vector3 SlidePositionStart = new Vector3();
    Vector3[] SlidePositionEnd = new Vector3[3];


    void Start()
    {
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        var notes = GameObject.Find("Notes").transform;
        for (int i = 0; i < star_slide.Length; i++)
        {
            star_slide[i] = Instantiate(star_slidePrefab, notes);
            spriteRenderer_star[i] = star_slide[i].GetComponent<SpriteRenderer>();
            if (isEach) spriteRenderer_star[i].sprite = eachStar;
            star_slide[i].transform.rotation = Quaternion.Euler(0, 0, -22.5f + (-45f * (i + 3 + startPosition)));
            SlidePositionEnd[i] = getPositionFromDistance(4.8f, i + 3 + startPosition);
            star_slide[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, -45f * (startPosition - 1));
        slideBars.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            slideBars.Add(transform.GetChild(i).gameObject);
        }
        SlidePositionStart = getPositionFromDistance(4.8f);

        for (int i = 0; i < slideBars.Count; i++)
        {
            var sr = slideBars[i].GetComponent<SpriteRenderer>();
            if (isEach) sr.sprite = eachSlide[i];
            sbRender.Add(sr);
            sr.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var startiming = timeProvider.AudioTime - timeStar;
        if (startiming <= 0f)
        {
            var alpha = startiming * (speed / 3) + 1f;
            alpha = alpha > 1f ? 1f : alpha;
            alpha = alpha < 0f ? 0f : alpha;
            foreach (var sr in sbRender)
            {
               sr.color = new Color(1f, 1f, 1f, alpha);
            }
            return;
        }

        foreach (var star in star_slide)
            star.SetActive(true);

        var timing = timeProvider.AudioTime - time;
        if (timing <= 0f)
        {
            var alpha = 1f - (-timing / (time - timeStar)) + 0.8f;
            alpha = alpha > 1f ? 1f : alpha;
            alpha = alpha < 0f ? 0f : alpha;
            for (int i = 0; i < star_slide.Length; i++)
            {
                spriteRenderer_star[i].color = new Color(1, 1, 1, alpha);
                star_slide[i].transform.localScale = new Vector3(alpha, alpha, alpha);
                star_slide[i].transform.position = SlidePositionStart;
            }
        }
        if (timing > 0f)
        {
            
            var process = (LastFor - timing) / LastFor;
            process = 1f - process;
            if (process > 1)
            {
                GameObject.Find("ObjectCount").GetComponent<ObjectCount>().slideCount++;
                for (int i = 0; i < star_slide.Length; i++)
                    Destroy(star_slide[i]);
                Destroy(gameObject);
            }
            var pos = (slideBars.Count - 1) * process;
            for (int i = 0; i < star_slide.Length; i++)
            {
                spriteRenderer_star[i].color = Color.white;
                star_slide[i].transform.position = (SlidePositionEnd[i] - SlidePositionStart) * process + SlidePositionStart; //TODO add some runhua
            }
            for (int i = 0; i < pos; i++)
            {
                slideBars[i].SetActive(false);
            }
        }
    }

    Vector3 getPositionFromDistance(float distance)
    {
        return new Vector3(
            distance * Mathf.Cos((startPosition * -2f + 5f) * 0.125f * Mathf.PI),
            distance * Mathf.Sin((startPosition * -2f + 5f) * 0.125f * Mathf.PI));
    }
    Vector3 getPositionFromDistance(float distance, int position)
    {
        return new Vector3(
            distance * Mathf.Cos((position * -2f + 5f) * 0.125f * Mathf.PI),
            distance * Mathf.Sin((position * -2f + 5f) * 0.125f * Mathf.PI));
    }
}
