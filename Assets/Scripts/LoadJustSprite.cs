using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadJustSprite : MonoBehaviour
{
    public int _0curv1str2wifi;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = GameObject.Find("Outline").GetComponent<CustomSkin>().Just[_0curv1str2wifi];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
