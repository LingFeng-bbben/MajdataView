using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffectManager : MonoBehaviour
{
    public Sprite hex;
    public Sprite star;

    GameObject[] tapEffects = new GameObject[8];
    Animator[] animators = new Animator[8];
    GameObject Outline;
    Animator OutlineAnimator;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            tapEffects[i] = transform.GetChild(i).gameObject;
            animators[i] = tapEffects[i].transform.GetChild(0).gameObject.GetComponent<Animator>();
        }
        Outline = GameObject.Find("Outline");
        OutlineAnimator = Outline.transform.GetChild(0).gameObject.GetComponent<Animator>();
    }

    public void setOutlineColor(string diff)
    {
        if (diff == "EASY")
        {
            setOutlineColor(0, 80, 250);
        } else if (diff == "BASIC")
        {
            setOutlineColor(0, 250, 0);
        } else if (diff == "ADVANCED")
        {
            setOutlineColor(250, 240, 0);
        } else if (diff == "EXPERT")
        {
            setOutlineColor(250, 40, 40);
        } else if (diff == "MASTER")
        {
            setOutlineColor(200, 0, 250);
        } else if (diff == "Re:MASTER")
        {
            setOutlineColor(200, 0, 250);
        } else if (diff == "ORIGINAL")
        {
            setOutlineColor(250, 160, 0);
        } else
        {
            setOutlineColor(255, 255, 255);
        }
    }

    public void setOutlineColor(byte R, byte G, byte B)
    {
        Outline.GetComponent<SpriteRenderer>().color = new Color32(R, G, B, 255);
        Outline.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color32(R, G, B, 0);
    }

    // Update is called once per frame
    public void PlayEffect(int position,bool isBreak)
    {
        OutlineAnimator.SetTrigger("play");
        var pos = position - 1;
        if (isBreak)
        {
            tapEffects[pos].transform.localScale = new Vector3(1.4f, 1.4f);
            animators[pos].SetTrigger("break");
        }
        else {
            tapEffects[pos].transform.localScale = new Vector3(1.3f, 1.3f);
            animators[pos].SetTrigger("tap");
        }
    }
}
