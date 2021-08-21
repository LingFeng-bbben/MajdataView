using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffectManager : MonoBehaviour
{
    public Sprite hex;
    public Sprite star;

    GameObject[] tapEffects = new GameObject[8];
    Animator[] animators = new Animator[8];

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            tapEffects[i] = transform.GetChild(i).gameObject;
            animators[i] = tapEffects[i].GetComponent<Animator>();
            tapEffects[i].SetActive(false);
        }
    }

    // Update is called once per frame
    public void PlayEffect(int position,bool isBreak)
    {
        var pos = position - 1;
        tapEffects[pos].SetActive(true);
        if (isBreak) animators[pos].SetTrigger("break");
        else animators[pos].SetTrigger("tap");
    }
}
