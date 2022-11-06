using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffectManager : MonoBehaviour
{
    public Sprite hex;
    public Sprite star;

    GameObject[] tapEffects = new GameObject[8];
    Animator[] tapAnimators = new Animator[8];
    GameObject[] judgeEffects = new GameObject[8];
    Animator[] judgeAnimators = new Animator[8];

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            tapEffects[i] = transform.GetChild(i).gameObject;
            tapAnimators[i] = tapEffects[i].GetComponent<Animator>();
            tapEffects[i].SetActive(false);
        }
        GameObject judgeObject = GameObject.Find("JudgeEffects");
        for (int i = 0; i < judgeObject.transform.childCount; i++)
        {
            judgeEffects[i] = judgeObject.transform.GetChild(i).gameObject;
            judgeAnimators[i] = judgeEffects[i].GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    public void PlayEffect(int position,bool isBreak)
    {
        var pos = position - 1;
        tapEffects[pos].SetActive(true);
        if (isBreak)
        {
            tapAnimators[pos].SetTrigger("break");
            judgeAnimators[pos].SetTrigger("break");
        }
        else
        {
            tapAnimators[pos].SetTrigger("tap");
            judgeAnimators[pos].SetTrigger("perfect");
        }
    }
}
