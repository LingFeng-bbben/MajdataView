using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDrop : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject star_slide;

    SpriteRenderer spriteRenderer_star;

    public Sprite spriteNormal;
    public Sprite spriteEach;

    public bool isMirror;
    public bool isEach;
    public float time;
    public float timeStar;
    public float LastFor = 1f;
    public float speed;

    public int startPosition =1;

    public int sortIndex = 0;

    AudioTimeProvider timeProvider;

    List<GameObject> slideBars = new List<GameObject>();

    List<Vector3> slidePositions = new List<Vector3>();
    List<Quaternion> slideRotations = new List<Quaternion>();

    void Start()
    {
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
    }

    private void OnEnable()
    {
        if (isMirror)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            transform.rotation = Quaternion.Euler(0f, 0f, -45f * (startPosition ));
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -45f * (startPosition - 1));
        }

        
        spriteRenderer_star = star_slide.GetComponent<SpriteRenderer>();
        for (int i = 0; i < transform.childCount; i++)
        {
            slideBars.Add(transform.GetChild(i).gameObject);
        }
        slidePositions.Add(getPositionFromDistance(4.8f));
        foreach (var bars in slideBars)
        {
            slidePositions.Add(bars.transform.position);
            slideRotations.Add( Quaternion.Euler(bars.transform.rotation.eulerAngles+new Vector3(0f,0f,15f)));
        }
        foreach (var gm in slideBars)
        {
            var sr = gm.GetComponent<SpriteRenderer>();
            sr.color = new Color(1f, 1f, 1f, 0f);
            sr.sortingOrder += sortIndex;
            sr.sortingLayerName = "Slide";
            if (isEach) sr.sprite = spriteEach;
            else sr.sprite = spriteNormal;
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
            setSlideBarAlpha(alpha);
            return;
        }
        setSlideBarAlpha(1f);
        star_slide.SetActive(true);
        var timing = timeProvider.AudioTime - time;
        if (timing <= 0f)
        {
            var alpha = 1f-( -timing / (time - timeStar));
            alpha = alpha > 1f ? 1f : alpha;
            alpha = alpha < 0.5f ? 0.5f : alpha;
            spriteRenderer_star.color = new Color(1, 1, 1,alpha);
            star_slide.transform.localScale = new Vector3(alpha+0.5f, alpha + 0.5f, alpha + 0.5f);
            star_slide.transform.position = slidePositions[0];
            star_slide.transform.rotation = slideRotations[0];
        }
        if (timing > 0f)
        {
            spriteRenderer_star.color = Color.white;
            var process = (LastFor -timing) / LastFor;
            process = 1f - process;
            if (process > 1) {
                GameObject.Find("ObjectCount").GetComponent<ObjectCount>().slideCount++;
                Destroy(star_slide);
                Destroy(gameObject);
            }
            //print(process);
            var pos = (slidePositions.Count-1) * process;
            int index = (int)pos;
            try
            {
                star_slide.transform.position = (slidePositions[index + 1] - slidePositions[index]) * (pos - index) + slidePositions[index]; 
                                                                                                                                             //star_slide.transform.rotation = slideRotations[index];
                star_slide.transform.rotation = Quaternion.Euler(
                    (slideRotations[index + 1].eulerAngles - slideRotations[index].eulerAngles) * (pos - index) + slideRotations[index].eulerAngles
                );
                for (int i = 0; i < pos; i++)
                {
                    slideBars[i].SetActive(false);
                }
            }
            catch { }

        }
    }
    void setSlideBarAlpha(float alpha)
    {
        foreach (var gm in slideBars)
        {
            gm.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);
        }
    }
    Vector3 getPositionFromDistance(float distance)
    {
        return new Vector3(
            distance * Mathf.Cos((startPosition * -2f + 5f) * 0.125f * Mathf.PI),
            distance * Mathf.Sin((startPosition * -2f + 5f) * 0.125f * Mathf.PI));
    }
}
