using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideEffectManager : MonoBehaviour
{
    GameObject[] slideEffects = new GameObject[17];
    Animator[] animators = new Animator[17];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            slideEffects[i] = transform.GetChild(i).GetChild(0).gameObject;
            animators[i] = slideEffects[i].GetComponent<Animator>();
            slideEffects[i].SetActive(false);
        }
    }

    void Update()
    {

    }

    public void PlayEffect(string area)
    {
        if (area[0] == 'C')
        {
            PlayEffect(16);
        }else if(area[0] == 'A')
        {
            PlayEffect(area[1] - '0' - 1);
        }else if(area[0] == 'B')
        {
            PlayEffect(area[1] - '0' - 1 + 8);
        }
    }

    public void PlayEffect(int index)
    {
        slideEffects[index].SetActive(true);
        animators[index].SetTrigger("playing");
    }
}
